using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MemTracer
{
    public partial class MemOpTree : UserControl
    {
        public MemOpTree()
        {
            InitializeComponent();
        }

        public void BuildTree(List<MemOperation> ops)
        {
            treeView1.Nodes.Clear();

            m_memOps = ops;
            bool showFreedBlocks = cbShowFreed.Checked;
            m_numAllocated = m_numFreed = 0;
            m_bytesAllocated = 0;
            foreach (MemOperation op in ops)
            {
                if (op.OpType == MemOperation.Type.Alloc)
                {
                    if (showFreedBlocks || !WasFreed(op.UserData as MemBlock))
                    {
                        AddAlloc(op.UserData as MemBlock);
                    }                      
                }
                else if (op.OpType == MemOperation.Type.Free && showFreedBlocks)
                {
                    AddFree((uint)op.UserData);
                }
            }

            labelFrameOpStats.Text = "Allocated " + m_bytesAllocated.ToString() +
                " (" + (m_bytesAllocated >> 10).ToString() + "kb) in " +
                    m_numAllocated.ToString() + " block(s). ";
            if (showFreedBlocks)
            {
                labelFrameOpStats.Text += "Freed " + m_numFreed.ToString() + " block(s).";
            }
        }

        bool WasFreed(MemBlock mb)
        {
            foreach (MemOperation op in m_memOps)
            {
                if (op.OpType == MemOperation.Type.Free)
                {
                    uint address = (uint)op.UserData;
                    if (address == mb.m_address)
                        return true;
                }
            }
            return false;
        }

        void AddAlloc(MemBlock mb)
        {
            string opText = "Alloc: " + mb.m_size + " byte(s) at " + mb.m_address.ToString("X") + " (" +
                mb.GetTagString() + ")";

            treeView1.Nodes.Add(opText);
            TreeNode n = treeView1.Nodes[treeView1.Nodes.Count - 1];

            uint[] callStack = CallstackTab.GetCallStack(mb.m_callStackCRC);
            foreach (uint callStackEntry in callStack)
            {
                IStackTracer.Symbol symbol = MemTracerForm.ms_MainForm.StackTracer.GetSymbolForAddress(callStackEntry);
                if (symbol.functionName != null)
                {
                    n.Nodes.Add(symbol.functionName);
                    n = n.Nodes[n.Nodes.Count - 1];
                }
            }
            ++m_numAllocated;
            m_bytesAllocated += mb.m_size;
        }
        void AddFree(uint address)
        {
            string opText = "Free mem at: " + address.ToString("X");
            treeView1.Nodes.Add(opText);
            ++m_numFreed;
        }

        int m_numAllocated;
        uint m_bytesAllocated;
        int m_numFreed;
        List<MemOperation> m_memOps;

        private void cbShowFreed_CheckedChanged(object sender, EventArgs e)
        {
            BuildTree(m_memOps);
        }
    }
}
