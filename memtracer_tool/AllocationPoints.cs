using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace MemTracer
{
    public partial class AllocationPoints : Form
    {
        class AllocInfo
        {
            public IStackTracer.Symbol symbol;
            public int numBytes;
            public int numBlocks;
            public uint address;
        }

        public AllocationPoints()
        {
            InitializeComponent();
        }

        public void InitAllocationPoints(List<MemOperation> memOps)
        {
            MemBlockTree tree = new MemBlockTree();
            uint totalMem = 0;
            foreach (MemOperation op in memOps)
            {
                if (op.OpType == MemOperation.Type.Alloc)
                {
                    MemBlock block = op.UserData as MemBlock;
                    tree.AddMemBlock(block);
                    totalMem += block.m_size;
                }
            }
            foreach (MemOperation op in memOps)
            {
                if (op.OpType == MemOperation.Type.Alloc)
                    AddAllocationPoint(op.UserData as MemBlock, tree);
                }

            for (int i = 0; i < dataGridAllocs.Rows.Count; ++i)
            {
                AllocInfo allocInfo = dataGridAllocs.Rows[i].Tag as AllocInfo;
                float percentage = allocInfo.numBytes * 100.0f / totalMem;
                String[] rowData = { allocInfo.symbol.functionName, allocInfo.numBytes.ToString(), allocInfo.numBlocks.ToString(), 
                                       percentage.ToString() };
                dataGridAllocs.Rows[i].SetValues(rowData);
            }
        }

        private int FindRowIndexForCallerAddress(uint address)
        {
            for (int i = 0; i < dataGridAllocs.Rows.Count; ++i)
            {
                if (((AllocInfo)dataGridAllocs.Rows[i].Tag).address == address)
                    return i;
            }
            return -1;
        }
        void AddAllocationPoint(MemBlock block, MemBlockTree tree)
        {
            int rootIndex = block.FindFirstValidSymbolIndex();
            if (rootIndex < 0)
                return;

            uint[] callStack = CallstackTab.GetCallStack(block.m_callStackCRC);
            for (int i = rootIndex; i < callStack.Length; ++i)
            {
                IStackTracer.Symbol symbol = MemTracerForm.ms_MainForm.StackTracer.GetSymbolForAddress(callStack[i]);
                if (symbol.functionName == null)
                    continue;
                int rowIndex = FindRowIndexForCallerAddress(callStack[i]);
                if (rowIndex >= 0)
                {
                    //AllocInfo allocInfo = dataGridAllocs.Rows[rowIndex].Tag as AllocInfo;
                    //allocInfo.numBytes += (int)block.m_size;
                    //++allocInfo.numBlocks;
                }
                else
                {
                    //MemBlockTree.Node node = tree.Root.FindChildByCallAddress(callStack[i], true);
                    List<MemBlockTree.Node> nodes = new List<MemBlockTree.Node>();
                    tree.Root.CollectChildrenByCallAddress(callStack[i], true, nodes);

                    AllocInfo allocInfo = new AllocInfo();
                    allocInfo.symbol = symbol;
                    allocInfo.address = callStack[i];
                    
                    foreach (MemBlockTree.Node node in nodes)
                    {
                        allocInfo.numBytes += (int)node.GetAllocatedSize();
                        allocInfo.numBlocks += node.GetNumAllocatedBlocks();
                    }

                    String[] rowData = { symbol.functionName, allocInfo.numBytes.ToString(), allocInfo.numBlocks.ToString(), "" };
                    dataGridAllocs.Rows.Add(rowData);
                    //allocInfo.numBytes = (int)node.GetAllocatedSize();
                    //allocInfo.numBlocks = node.GetNumAllocatedBlocks();
                    dataGridAllocs.Rows[dataGridAllocs.Rows.Count - 1].Tag = allocInfo;
                }
            }

        }

        private void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Name == "colAllocs" || e.Column.Name == "colBytes")
            {
                e.Handled = true;
                int d = Int32.Parse(e.CellValue1.ToString()) - Int32.Parse(e.CellValue2.ToString());
                if (d < 0)
                    e.SortResult = -1;
                else if (d > 0)
                    e.SortResult = 1;
                else
                    e.SortResult = 0;
            }
            else if (e.Column.Name == "colPerc")
            {
                e.Handled = true;
                double d = Convert.ToDouble(e.CellValue1.ToString()) - Convert.ToDouble(e.CellValue2.ToString());
                if (d < 0.0)
                    e.SortResult = -1;
                else if (d > 0.0)
                    e.SortResult = 1;
                else
                    e.SortResult = 0;
            }
        }

        private void dataGridAllocs_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                DataGridViewCell cell = dataGridAllocs.Rows[e.RowIndex].Cells[e.ColumnIndex];
                AllocInfo info = dataGridAllocs.Rows[e.RowIndex].Tag as AllocInfo;
                if (info.symbol.functionName != null)
                    cell.ToolTipText = info.symbol.functionName + "@" + info.symbol.line;
                else
                    cell.ToolTipText = "";
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam); 
        private void ScrollToLine(uint line)
        {
            SendMessage(richTextBox1.Handle, 0x00b6, 0, (int)line - 1);
        }
        private void OpenSourceFile(IStackTracer.Symbol symbol)
        {
            if (symbol.fileName == null)
                return;

            try
            {
                this.richTextBox1.LoadFile(symbol.fileName, RichTextBoxStreamType.PlainText);
                ScrollToLine(symbol.line);
            }
            catch (System.IO.IOException)
            {
            }
        }
 
        private void dataGridAllocs_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                DataGridViewCell cell = dataGridAllocs.Rows[e.RowIndex].Cells[e.ColumnIndex];
                AllocInfo info = dataGridAllocs.Rows[e.RowIndex].Tag as AllocInfo;
                OpenSourceFile(info.symbol);
            }
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }
    }
}