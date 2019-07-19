using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using CustomUIControls.Graphing;
using System.Xml.Serialization;

namespace MemTracer
{
    public delegate void DelegateDoubleClickNode(object sender, System.EventArgs e);
    delegate void DelegateAddModuleInfo(string modName, ulong modBase, ulong modSize);
    public delegate void DelegateCompareSnapshots(int snapA, int snapB);
    public delegate void DelegateOverlapSnapshots(int snapA, int snapB);

    public partial class MemTracerForm : Form
    {
        delegate void DelegateOnDisconnect();
        delegate void DelegateAddSnapshot(SnapshotDesc desc);

        const ulong BUFFER_SIZE = 16384;

        Socket m_clientSocket = null;
        IAsyncResult m_result;
        AsyncCallback m_pfnCallBack = null;
        List<MemOperation> m_memOperations = new List<MemOperation>();
        MemSnapshot m_globalSnapshot = new MemSnapshot();
        int m_frame = 0;
        int m_lastSnapshotViewFrame = 0;
        int m_lastMemOpFrame = 0;
        List<SnapshotDesc> m_snapshots = new List<SnapshotDesc>();
        DelegateAddSnapshot m_delegateAddSnapshot;
        int[] m_mostAllocatedBlocks = new int[65536];
        DelegateAddModuleInfo m_delegateAddModuleInfo;
        bool m_dirty = true;
        bool m_initialized = false;
        bool m_64bit = false;
        int m_addressSize = 4;
        Dictionary<string, int> m_tracedVars = new Dictionary<string, int>();
        int m_selectedSnapshotIndex = -1;
        int m_numMemOpsPrevFrame = 0;
        int m_maxTagLen = 32;
        int m_maxSnapshotNameLen = 32;
        int m_maxTracedVarNameLen = 32;
        MsgReceiver m_msgReceiver = new MsgReceiver();
        int m_numFrames = 0;
        bool m_replayingStream = false;
        int m_ticksToNextFrame = 100;
        DelegateOnDisconnect m_delegateOnDisconnect;
        C2DPushGraph.LineHandle m_graphLineHandle;

        public IStackTracer StackTracer { get; set; }

        public static MemTracerForm ms_MainForm = null;

        public MemTracerForm()
        {
            InitializeComponent();

            m_delegateAddModuleInfo = new DelegateAddModuleInfo(this.AddModuleInfo);
            m_delegateOnDisconnect = new DelegateOnDisconnect(this.OnDisconnect);
            m_delegateAddSnapshot = new DelegateAddSnapshot(this.AddSnapshot);

            snapshotList1.OnCompareSnapshots = new DelegateCompareSnapshots(this.CompareSnapshots);
            snapshotList1.OnOverlapSnapshots = new DelegateOverlapSnapshots(this.OverlapSnapshots);
            snapshotList1.SetMainForm(this);
            snapshotInfo1.Init("Global");

            m_graphLineHandle = usageGraph.AddLine(0, Color.LightGreen);

            ms_MainForm = this;

            LoadConfig(kConfigFileName);
            StackTracer = new DiaStackTracer();

            EnableTabPage(tabPageFrameSnapshot, IsFrameAnalysisEnabled());
            EnableTabPage(tabPageFrameOps, IsFrameAnalysisEnabled());
        }

        [Serializable]
        public class SnapshotDesc
        {
            public String name;
            public int memOperationNr;
            public uint bytes;
            public int blocks;
        }
        struct SocketPacket
        {
            public enum CommandID
            {
                INITIAL_SETTINGS = 1,
                MODULE_INFO,
                ALLOC,
                FREE,
                FRAME_END,
                TAG_BLOCK,
                ADD_SNAPSHOT,
                TRACED_VAR,
                MAX
            };

            public byte[] dataBuffer;
        }
        struct ClientPlatform
        {
            public enum Platform
            {
                WINDOWS_32,
                WINDOWS_64,
                XENON,
                PS3
            };
        }

        public void WaitForData()
        {
            try
            {
                if (m_clientSocket == null)
                    return;

                if (m_pfnCallBack == null)
                {
                    m_pfnCallBack = new AsyncCallback(OnDataReceived);
                }
                SocketPacket theSocPkt = new SocketPacket();
                theSocPkt.dataBuffer = new byte[BUFFER_SIZE];
                // Start listening to the data asynchronously
                m_result = m_clientSocket.BeginReceive(theSocPkt.dataBuffer,
                    0, theSocPkt.dataBuffer.Length,
                    SocketFlags.None,
                    m_pfnCallBack,
                    theSocPkt);
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
        }


        void OnDataReceived(IAsyncResult asyn)
        {
            if (m_clientSocket == null)
                return;

            try
            {
                SocketPacket packet = (SocketPacket)asyn.AsyncState;
                int receivedBytes = m_clientSocket.EndReceive(asyn);

                List<byte[]> messages = m_msgReceiver.OnDataReceived(packet.dataBuffer, receivedBytes);
                for (int i = 0; i < messages.Count; ++i)
                {
                    HandleMessage(messages[i]);
                }
                if (m_clientSocket != null && m_clientSocket.Connected)
                    WaitForData();
            }
            catch (NullReferenceException)
            {
                // Socket closed (most probably)
            }
            catch (ObjectDisposedException)
            {
                // Socket closed
            }
            catch (SocketException exc)
            {
                MessageBox.Show(exc.Message);
                Invoke(m_delegateOnDisconnect);
            }
        }

        void SetControlButtonsState(bool enabled)
        {
            butNextFrame.Enabled = enabled;
            butPrevFrame.Enabled = enabled;
            butPlay.Enabled = enabled;
            butFFwd.Enabled = enabled;
            butRewind.Enabled = enabled;
        }

        void OnDisconnect()
        {
            CloseSocket();

            butConnect.Text = "Connect";
            EnableTabPage(tabPageFrameSnapshot, IsFrameAnalysisEnabled());
            EnableTabPage(tabPageFrameOps, IsFrameAnalysisEnabled());

            SetControlButtonsState(IsFrameAnalysisEnabled());
            GoToFrame(0);
        }

        void OnConnect()
        {
            butConnect.Text = "Disconnect";
            timer1.Enabled = true;

            EnableTabPage(tabPageFrameSnapshot, false);
            EnableTabPage(tabPageFrameOps, false);

            SetControlButtonsState(false);
        }

        static private ulong GetInt64(byte[] data, int index)
        {
            ulong result = 0;
            byte shift = 0;
            for (int i = 0; i < 8; ++i)
            {
                result += (ulong)data[i + index] << shift;
                shift += 8;
            }
            return (result);
        }
        static private uint GetInt(byte[] data, int i)
        {
            return (uint)((data[i + 3] << 24) + (data[i + 2] << 16) + (data[i + 1] << 8) + (data[i + 0]));
        }
        private ulong GetAddress(byte[] data, int index)
        {
            if (m_64bit)
            {
                return GetInt64(data, index);
            }
            else
            {
                return GetInt(data, index);
            }
        }
        private static string GetStringFromBuffer(byte[] data, int dataIdx, int maxLen)
        {
            System.Text.Decoder d = System.Text.Encoding.ASCII.GetDecoder();
            int charLen = d.GetCharCount(data, dataIdx, maxLen);
            char[] charBuf = new char[charLen];
            d.GetChars(data, dataIdx, charLen, charBuf, 0);
            string ret = new string(charBuf);
            char[] zeroChars = { '\0', '?' };
            return ret.TrimEnd(zeroChars);
        }

        private void HandleMessage(byte[] msgData)
        {
            SocketPacket.CommandID commandId = (SocketPacket.CommandID)(msgData[0]);
            switch (commandId)
            {
                case SocketPacket.CommandID.INITIAL_SETTINGS:
                    {
                        ClientPlatform.Platform platform = (ClientPlatform.Platform)msgData[1];

                        //if (platform == 2)
                        //{
                        //    PS3StackTracer ps3tracer = new PS3StackTracer(m_config.PS3BinPath,
                        //                                    m_config.Verbose, m_config.UseCr,
                        //                                    m_config.UseError);
                        //    StackTracer = ps3tracer;
                        //}

                        m_64bit = (platform == ClientPlatform.Platform.WINDOWS_64);
                        if (m_64bit)
                        {
                            m_addressSize = 8;
                        }

                        m_maxTagLen = (int)msgData[2];
                        m_maxSnapshotNameLen = (int)msgData[3];
                        m_maxTracedVarNameLen = (int)msgData[4];
                        System.Diagnostics.Debug.Assert(m_maxTagLen > 0);
                        System.Diagnostics.Debug.Assert(m_maxSnapshotNameLen > 0);
                        System.Diagnostics.Debug.Assert(m_maxTracedVarNameLen > 0);
                        break;
                    }

                case SocketPacket.CommandID.MODULE_INFO:
                    {
                        ulong modBase = GetAddress(msgData, 1);
                        int offset = 1 + m_addressSize;
                        
                        ulong modSize = GetInt(msgData, offset);
                        offset += 4;

                        string debugFileName = GetStringFromBuffer(msgData, offset, 128);
                        Invoke(m_delegateAddModuleInfo, new Object[] { debugFileName, modBase, modSize });
                        m_initialized = true;
                        break;
                    }

                case SocketPacket.CommandID.ALLOC:
                    {
                        HandleAlloc(msgData);
                        break;
                    }

                case SocketPacket.CommandID.FREE:
                    {
                        ulong addr = GetAddress(msgData, 1);
                        MemOperation op = new MemOperation(MemOperation.Type.Free);
                        op.UserData = addr;
                        m_memOperations.Add(op);
                        m_globalSnapshot.RemoveBlock(addr);
                        m_dirty = true;

//                        System.Console.Out.WriteLine("Free " + addr.ToString("X") + " " + (m_memOperations.Count - 1));

                        break;
                    }

                case SocketPacket.CommandID.TAG_BLOCK:
                    {
                        ulong addr = GetInt(msgData, 1);
                        string tag = GetStringFromBuffer(msgData, (1 + m_addressSize), m_maxTagLen);
                        ulong crc = TagDict.AddTag(tag);
                        m_globalSnapshot.TagBlock(addr, crc);
                        break;
                    }

                case SocketPacket.CommandID.FRAME_END:
                    {
                        // Only add frame end marker if something interesting happened in this frame.
                        // Otherwise we'd be just wasting memory.
                        if (m_numMemOpsPrevFrame < m_memOperations.Count)
                        {
                            MemOperation op = new MemOperation(MemOperation.Type.FrameEnd);
                            m_memOperations.Add(op);
                            ++m_frame;
                            ++m_numFrames;
                        }
                        m_numMemOpsPrevFrame = m_memOperations.Count;
                        break;
                    }

                case SocketPacket.CommandID.ADD_SNAPSHOT:
                    {
                        string snapshotName = GetStringFromBuffer(msgData, 1, m_maxSnapshotNameLen);

                        //System.Console.Out.WriteLine("Snapshot " + snapshotName);
                        
                        SnapshotDesc desc = CreateSnapshotDesc(snapshotName);
                        Invoke(m_delegateAddSnapshot, desc);
                        
                        break;
                    }

                case SocketPacket.CommandID.TRACED_VAR:
                    {
                        int tracedVarValue = (int)GetInt(msgData, 1);
                        string tracedVarName = GetStringFromBuffer(msgData, 5, m_maxTracedVarNameLen);

                        SetTracedVar(tracedVarName, tracedVarValue);

                        break;
                    }

                default:
                    {
                        System.Diagnostics.Debug.Fail("Unknown command ID: " + commandId);
                        break;
                    }
            }
        }
        private void HandleAlloc(byte[] msgData)
        {
            MemBlock memBlock = new MemBlock();
            int addressSize = 4;
            if (m_64bit)
            {
                addressSize = 8;
            }

            memBlock.m_address = GetAddress(msgData, 1);
            memBlock.m_size = GetInt(msgData, 1 + addressSize);
            memBlock.m_tag = GetInt(msgData, 1 + addressSize + 4);
            int stackDepth = (int)msgData[1 + addressSize + 8];
            System.Diagnostics.Debug.Assert(stackDepth > 0 && stackDepth < 100);
            ulong[] callStack = new ulong[stackDepth];
            int callstackStart = 1 + addressSize + 9;
            for (int i = 0; i < stackDepth; ++i)
            {
                callStack[i] = GetAddress(msgData, callstackStart + i * addressSize);
            }
            memBlock.m_callStackCRC = CallstackTab.CalcCRC(callStack);
            CallstackTab.AddCallStack(memBlock.m_callStackCRC, callStack);

            if (m_initialized)
            {
                MemOperation op = new MemOperation(MemOperation.Type.Alloc);
                op.UserData = memBlock;
                m_memOperations.Add(op);
                m_globalSnapshot.AddBlock(memBlock);
                if (memBlock.m_size < 65536)
                    ++m_mostAllocatedBlocks[memBlock.m_size];
            }
            m_dirty = true;
        }

        private void CloseSocket()
        {
            try
            {
                if (m_clientSocket != null)
                {
                    m_clientSocket.Shutdown(SocketShutdown.Both);
                    m_clientSocket.Close();
                    m_clientSocket = null;
                }
            }
            catch (SocketException)
            {
            }
        }

        private void OnPlayStateChange()
        {
            butConnect.Enabled = !m_replayingStream;
            SetControlButtonsState(!m_replayingStream);
            butPlay.Enabled = true;
        }

        private void toolBar1_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button == butNextFrame)
            {
                AdvanceFrame();
            }
            else if (e.Button == butPrevFrame)
            {
                RewindFrame();
            }
            else if (e.Button == butFFwd)
            {
                GoToFrame(m_numFrames);
            }
            else if (e.Button == butRewind)
            {
                GoToFrame(0);
            }
            else if (e.Button == butPlay)
            {
                m_replayingStream = !m_replayingStream;
                butPlay.Text = (m_replayingStream ? "Pause" : "Play");

                timer1.Enabled = true;
                m_ticksToNextFrame = 100 / (int)numericUpDownSpeed.Value;

                OnPlayStateChange();

                if (m_replayingStream)
                {
                    m_graphLineHandle.Clear();
                }
            }

            if (e.Button != butConnect)
            {
                return;
            }

            if (m_clientSocket != null)
            {
                OnDisconnect();
                return;
            }
            try
            {
                int iPortNo = 1000;
                ConnectDialog cd = new ConnectDialog(iPortNo);
                if (cd.ShowDialog(this) != DialogResult.OK)
                    return;

				// Create the socket instance
				m_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				
				// Cet the remote IP address
                String strIP = cd.GetServer();
				IPAddress ip = IPAddress.Parse(strIP);
				IPEndPoint ipEnd = new IPEndPoint(ip, cd.GetPort());
				m_clientSocket.Connect ( ipEnd );

				if (m_clientSocket.Connected) 
				{
                    OnConnect();
					WaitForData();
				}
			}
			catch (SocketException se)
			{
				string str;
				str = "\nConnection failed, is the server running?\n" + se.Message;
				MessageBox.Show (str);
				m_clientSocket = null;
			}		
        }

        private void LoadMemOperations(String fileName)
        {
            Stream s = File.Open(fileName, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            StackTracer.Deserialize(s, formatter);
            m_memOperations = formatter.Deserialize(s) as List<MemOperation>;
            m_snapshots = formatter.Deserialize(s) as List<SnapshotDesc>;
            m_numFrames = (int)formatter.Deserialize(s);
            m_mostAllocatedBlocks = formatter.Deserialize(s) as int[];
            CallstackTab.m_callStackMap = formatter.Deserialize(s) as Dictionary<ulong, ulong[]>;
            TagDict.m_tags = formatter.Deserialize(s) as Dictionary<ulong, string>;

            foreach (SnapshotDesc snapDesc in m_snapshots)
                AddSnapshot(snapDesc.name, snapDesc.bytes, snapDesc.blocks, snapDesc.memOperationNr);

            s.Close();

            m_selectedSnapshotIndex = -1;
            m_globalSnapshot = BuildEndSnapshot();
            UpdateFrameStats();

            SetControlButtonsState(m_numFrames > 0);
            m_lastMemOpFrame = -1;
            m_lastSnapshotViewFrame = -1;
            GoToFrame(0);
        }
        void SaveMemOperations(String fileName)
        {
            Stream s = File.Open(fileName, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            StackTracer.Serialize(s, formatter);
            formatter.Serialize(s, m_memOperations);
            formatter.Serialize(s, m_snapshots);
            formatter.Serialize(s, m_numFrames);
            formatter.Serialize(s, m_mostAllocatedBlocks);
            formatter.Serialize(s, CallstackTab.m_callStackMap);
            formatter.Serialize(s, TagDict.m_tags);

            s.Close();
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SaveMemOperations(saveFileDialog1.FileName);
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                LoadMemOperations(openFileDialog1.FileName);
            }
        }

        void UpdateFrameStats()
        {
            snapshotInfo1.Update(m_globalSnapshot);
            int pushVal = (int)m_globalSnapshot.NumAllocatedBytes >> 10;
            UpdateUsageGraph(pushVal);
        }

        private void UpdateUsageGraph(int pushVal)
        {
            if (pushVal > usageGraph.MaxPeekMagnitude)
            {
                usageGraph.MaxPeekMagnitude = pushVal + 10;
                usageGraph.MaxLabel = pushVal.ToString() + "kb";
            }
            usageGraph.Push(pushVal, 0);
            if (m_replayingStream)
            {
                usageGraph.UpdateGraph();
            }
        }

        MemSnapshot BuildSnapshotFromDesc(SnapshotDesc desc)
        {
            MemSnapshot snapshot = new MemSnapshot();
            int numOperations = desc.memOperationNr;
            if (numOperations > m_memOperations.Count)
                numOperations = m_memOperations.Count;
            for (int i = 0; i < numOperations; ++i)
            {
                MemOperation op = m_memOperations[i];
                if (op.OpType == MemOperation.Type.Alloc)
                    snapshot.AddBlock(op.UserData as MemBlock);
                else if (op.OpType == MemOperation.Type.Free)
                    snapshot.RemoveBlock((ulong)op.UserData);
            }

            return snapshot;
        }
        MemSnapshot BuildEndSnapshot()
        {
            MemSnapshot snapshot = new MemSnapshot();
            foreach (MemOperation op in m_memOperations)
            {
                if (op.OpType == MemOperation.Type.Alloc)
                    snapshot.AddBlock(op.UserData as MemBlock);
                else if (op.OpType == MemOperation.Type.Free)
                    snapshot.RemoveBlock((ulong)op.UserData);
                /*else
                {
                    int breakh = 1;
                }*/
            }
            return snapshot;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_dirty && !m_replayingStream)
                UpdateFrameStats();

            if (m_replayingStream)
            {
                --m_ticksToNextFrame;
                if (m_ticksToNextFrame <= 0)
                {
                    AdvanceFrame();
                    m_ticksToNextFrame = 100 / (int)numericUpDownSpeed.Value;
                }
            }
        }

        private SnapshotDesc CreateSnapshotDesc(string name)
        {
            SnapshotDesc desc = new SnapshotDesc();
            desc.memOperationNr = m_memOperations.Count;
            desc.name = name;
            desc.bytes = m_globalSnapshot.NumAllocatedBytes;
            desc.blocks = m_globalSnapshot.NumAllocatedBlocks;
            return desc;
        }
        private void AddSnapshot(SnapshotDesc desc)
        {
            m_snapshots.Add(desc);
            AddSnapshot(desc.name, desc.bytes, desc.blocks, desc.memOperationNr);
        }

        private void addSnapshotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewSnapshot dlg = new NewSnapshot();
            if (dlg.ShowDialog() == DialogResult.OK && dlg.GetName().Length != 0)
            {
                AddSnapshot(CreateSnapshotDesc(dlg.GetName()));
            }
        }

        int SmallestEntry(int[] mostAllocatedTab)
        {
            int smallestIdx = 0;
            for (int i = 0; i < mostAllocatedTab.Length; ++i)
            {
                if (mostAllocatedTab[i] < 0)
                    return i;

                int numBlocks = m_mostAllocatedBlocks[mostAllocatedTab[i]];
                if (numBlocks < m_mostAllocatedBlocks[mostAllocatedTab[smallestIdx]])
                    smallestIdx = i;
            }
            return smallestIdx;
        }
        void InitAllocationOverview(MostAllocatedForm frm, int maxEntries)
        {
            int[] mostAllocated = new int[maxEntries];
            for (int i = 0; i < maxEntries; ++i)
                mostAllocated[i] = -1;
            for (int i = 0; i < m_mostAllocatedBlocks.Length; ++i)
            {
                int numBlocks = m_mostAllocatedBlocks[i];
                int smallestIdx = SmallestEntry(mostAllocated);
                if (mostAllocated[smallestIdx] < 0 || numBlocks > m_mostAllocatedBlocks[mostAllocated[smallestIdx]])
                    mostAllocated[smallestIdx] = i;
            }
            for (int i = 0; i < maxEntries; ++i)
            {
                if (mostAllocated[i] >= 0)
                    frm.AddMostAllocatedEntry(mostAllocated[i], m_mostAllocatedBlocks[mostAllocated[i]]);
            }
        }

        private void mostAllocatedBlocksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MostAllocatedForm mostAllocated = new MostAllocatedForm();
            InitAllocationOverview(mostAllocated, 50);
            mostAllocated.InitLargestBlocks(m_memOperations, 50);
            mostAllocated.Sort();
            mostAllocated.ShowDialog();
        }

        void AddSnapshot(string name, uint bytes, int blocks, int memOpNr)
        {
            snapshotList1.AddSnapshot(name, bytes, blocks, memOpNr);
        }
        void AddModuleInfo(string modName, ulong modBase, ulong modSize)
        {
            StackTracer.AddModuleInfo(modName, modBase, modSize);
        }

        void CompareSnapshots(int snapAIdx, int snapBIdx)
        {
            MemSnapshot snapA = BuildSnapshotFromDesc(m_snapshots[snapAIdx]);
            MemSnapshot snapB = BuildSnapshotFromDesc(m_snapshots[snapBIdx]);
            CompareSnapshots dlg = new CompareSnapshots(snapA, snapB, m_snapshots[snapAIdx].name, m_snapshots[snapBIdx].name);
            dlg.ShowDialog();
        }
        void OverlapSnapshots(int snapAIdx, int snapBIdx)
        {
            MemSnapshot snapA = BuildSnapshotFromDesc(m_snapshots[snapAIdx]);
            MemSnapshot snapB = BuildSnapshotFromDesc(m_snapshots[snapBIdx]);
            MemSnapshot result = snapA.Overlap(snapB);
            ShowSnapshot dlg = new ShowSnapshot(result, "Overlap");
            dlg.ShowDialog();
        }

        private void snapshotInfoGlobal_DoubleClick(object sender, EventArgs e)
        {
            ShowSnapshot dlg = new ShowSnapshot(m_globalSnapshot, "Global");
            dlg.ShowDialog();
        }

        public MemSnapshot GetSnapshotByIndex(int index)
        {
            return BuildSnapshotFromDesc(m_snapshots[index]);
        }
        public string GetSnapshotNameByIndex(int index)
        {
            return m_snapshots[index].name;
        }
        public void DeleteSnapshot(int index)
        {
            m_snapshots.RemoveAt(index);
        }
        public void OnSnapshotSelected(int index)
        {
            MemSnapshot snap = BuildSnapshotFromDesc(m_snapshots[index]);

            snapshotInfo2.Init(m_snapshots[index].name);
            snapshotInfo2.Update(snap);
            snapshotInfo2.Visible = true;
            m_selectedSnapshotIndex = index;
        }

        private void snapshotInfo2_DoubleClick(object sender, EventArgs e)
        {
            ShowSnapshot dlg = new ShowSnapshot(snapshotInfo2.m_snapshot, snapshotInfo2.SnapshotName);
            dlg.ShowDialog();
        }

        private void allocationPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AllocationPoints dlg = new AllocationPoints();
            dlg.InitAllocationPoints(m_memOperations);
            dlg.ShowDialog();
        }

        private void tracedVariablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TracedVars tracedVars = new TracedVars(m_tracedVars);
            tracedVars.Show();
        }

        public int MemOperationNrForBlock(MemBlock block)
        {
            for (int i = 0; i < m_memOperations.Count; ++i)
            {
                if (m_memOperations[i].OpType == MemOperation.Type.Alloc && m_memOperations[i].UserData == block)
                    return i;
            }
            return -1;
        }

        private void snapshotList1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            snapshotInfo2_DoubleClick(sender, e);
        }

        void EnableTabPage(TabPage tp, bool enable)
        {
            tp.Enabled = enable;
            tabControl1.Refresh();
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tc = sender as TabControl;
            TabPage page = tc.TabPages[e.Index];
            if (!page.Enabled)
            {
                using (SolidBrush brush = new SolidBrush(SystemColors.GrayText))
                {
                    e.Graphics.DrawString(page.Text, page.Font, brush, e.Bounds.X + 3, e.Bounds.Y + 3);
                }
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(page.ForeColor))
                {
                    e.Graphics.DrawString(page.Text, page.Font, brush, e.Bounds.X + 3, e.Bounds.Y + 3);
                }
            }
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (!e.TabPage.Enabled)
            {
                e.Cancel = true;
            }
        }

        private MemSnapshot BuildSnapshotForFrame(int frame)
        {
            MemSnapshot snapshot = new MemSnapshot();
            int numOperations = m_memOperations.Count;
            int frames = 0;
            for (int i = 0; i < numOperations; ++i)
            {
                MemOperation op = m_memOperations[i];
                if (op.OpType == MemOperation.Type.Alloc)
                    snapshot.AddBlock(op.UserData as MemBlock);
                else if (op.OpType == MemOperation.Type.Free)
                    snapshot.RemoveBlock((ulong)op.UserData);
                else if (op.OpType == MemOperation.Type.FrameEnd)
                    ++frames;

                if (frames > frame)
                    break;
            }
            m_lastSnapshotViewFrame = frame;
            return snapshot;
        }
        private List<MemOperation> BuildOperationsListForFrame(int frame)
        {
            int numOperations = m_memOperations.Count;
            int frames = 0;
            List<MemOperation> ops = new List<MemOperation>();
            for (int i = 0; i < numOperations; ++i)
            {
                if (frames == frame)
                {
                    ops.Add(m_memOperations[i]);
                }
                if (m_memOperations[i].OpType == MemOperation.Type.FrameEnd)
                    ++frames;

                if (frames > frame)
                    break;
            }
            m_lastMemOpFrame = frame;
            return ops;
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage == tabPageFrameSnapshot)
            {
                if (m_frame != m_lastSnapshotViewFrame)
                {
                    MemSnapshot ms = BuildSnapshotForFrame(m_frame);
                    memAllocTreeFrame.BuildTree(ms);
                }
            }
            else if (e.TabPage == tabPageFrameOps)
            {
                if (m_frame != m_lastMemOpFrame)
                {
                    memOpTree1.BuildTree(BuildOperationsListForFrame(m_frame));
                }
            }
        }

        private void AdvanceFrame()
        {
            if (m_frame < m_numFrames)
                GoToFrame(m_frame + 1);
        }
        private void RewindFrame()
        {
            if (m_frame > 0)
                GoToFrame(m_frame - 1);
        }

        private void GoToFrame(int frame)
        {
            if (frameAnalysisToolStripMenuItem.Checked)
            {
                m_frame = frame;
                MemSnapshot ms = BuildSnapshotForFrame(m_frame);
                memAllocTreeFrame.BuildTree(ms);
                memOpTree1.BuildTree(BuildOperationsListForFrame(m_frame));

                toolStripStatusFrame.Text = "Frame: " + m_frame.ToString() + "/" + m_numFrames.ToString();
                UpdateUsageGraph((int)ms.NumAllocatedBytes >> 10);

                butRewind.Enabled = (m_frame > 0);
                butPrevFrame.Enabled = (m_frame > 0);
                butFFwd.Enabled = (m_frame < m_numFrames);
                butNextFrame.Enabled = (m_frame < m_numFrames);
            }
        }

        void SetTracedVar(string name, int value)
        {
            if (m_tracedVars.ContainsKey(name))
            {
                m_tracedVars[name] = value;
            }
            else
            {
                m_tracedVars.Add(name, value);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("MemTracer v0.3b\nCopyright (C) 2008-2010 Maciej Sinilo\nC2DPushGraph library copyright (C) Stuart Konen\n", 
                "About", MessageBoxButtons.OK);
        }

        private void numericUpDownSpeed_ValueChanged(object sender, EventArgs e)
        {
            m_ticksToNextFrame = 100 / (int)numericUpDownSpeed.Value;
        }

        private void frameAnalysisToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            EnableTabPage(tabPageFrameSnapshot, IsFrameAnalysisEnabled());
            EnableTabPage(tabPageFrameOps, IsFrameAnalysisEnabled());
            SetControlButtonsState(IsFrameAnalysisEnabled());
        }
        private bool IsFrameAnalysisEnabled()
        {
            return frameAnalysisToolStripMenuItem.Checked && m_numFrames > 0;
        }

        void LoadConfig(string fileName)
        {
            XmlSerializer s = new XmlSerializer(m_config.GetType());
            try
            {
                TextReader r = new StreamReader(fileName);
                m_config = s.Deserialize(r) as Config;
                r.Close();
            }
            catch
            { 
            }
        }
        void SaveConfig(string fileName)
        {
            XmlSerializer s = new XmlSerializer(m_config.GetType());
            TextWriter w = new StreamWriter(fileName, false);
            s.Serialize(w, m_config);
            w.Close();
        }

        public class Config
        {
            public string PS3BinPath
            {
                get { return m_ps3BinPath; }
                set { m_ps3BinPath = value; }
            }
            string m_ps3BinPath = "c:\\ps3bin.exe";
            public bool Verbose { get; set; }
            public bool UseCr { get; set; }
            public bool UseError { get; set; }
        };
        const string kConfigFileName = "config.xml";
        Config m_config = new Config();
    }
}
