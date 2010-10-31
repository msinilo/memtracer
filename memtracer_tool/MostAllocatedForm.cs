using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MemTracer
{
    public partial class MostAllocatedForm : Form
    {
        class CompareBySize : IComparer<MemBlock>
        {
            public int Compare(MemBlock lhs, MemBlock rhs)
            {
                return (int)(lhs.m_size - rhs.m_size);
            }
        }

        public MostAllocatedForm()
        {
            InitializeComponent();
        }

        public void AddMostAllocatedEntry(int blockSize, int numBlocks)
        {
            String[] data = { blockSize.ToString(), numBlocks.ToString(), (numBlocks * blockSize).ToString() };
            dataGridView1.Rows.Add(data);
        }
        public void InitLargestBlocks(List<MemOperation> memOps, int numBlocks)
        {
            MemBlock[] largestBlocks = new MemBlock[numBlocks];
            foreach (MemOperation op in memOps)
            {
                if (op.OpType == MemOperation.Type.Alloc)
                {
                    MemBlock block = op.UserData as MemBlock;
                    int i;
                    bool alreadyPresent = false;
                    for (i = 0; i < largestBlocks.Length; ++i)
                    {
                        if (largestBlocks[i] != null && block.m_size == largestBlocks[i].m_size)
                            alreadyPresent = true;
                        if (largestBlocks[i] != null && block.m_size < largestBlocks[i].m_size)
                            break;
                    }
                    if (!alreadyPresent)
                    {
                        for (int j = 1; j < i; ++j)
                            largestBlocks[j - 1] = largestBlocks[j];
                        largestBlocks[(i == 0 ? i : i - 1)] = block;
                    }
                }
            }
            for (int i = 0; i < largestBlocks.Length; ++i)
            {
                if (largestBlocks[i] != null)
                {
                    string[] data = { largestBlocks[i].m_size.ToString() };
                    dataGridView2.Rows.Add(data);
                    dataGridView2.Rows[dataGridView2.Rows.Count - 1].Tag = largestBlocks[i];
                }
            }

            Sort();
        }

        public void Sort()
        {
            dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Descending);
            dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Descending);
        }

        private void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            e.Handled = true;
            e.SortResult = Int32.Parse(e.CellValue1.ToString()) - Int32.Parse(e.CellValue2.ToString());
        }

        MemBlock[] CollectLargestBlocks(MemSnapshot snap, int n)
        {
            MemBlock[] largestBlocks = new MemBlock[snap.Blocks.Values.Count];
            int i = 0;
            foreach (MemBlock block in snap.Blocks.Values)
            {
                largestBlocks[i++] = block;
            }
            Array.Sort(largestBlocks, new CompareBySize());
                /*for (int i = 0; i < largestBlocks.Length; ++i)
                {
                    if (largestBlocks[i] != null && largestBlocks[i].m_size == block.m_size)
                        break;

                    if (largestBlocks[i] == null)
                    {
                        largestBlocks[i] = block;
                        break;
                    }
                    if (largestBlocks[i].m_size < block.m_size)
                    {
                        for (int j = i + 1; j < largestBlocks.Length; ++j)
                            largestBlocks[j] = largestBlocks[j - 1];
                        largestBlocks[i] = block;
                        break;
                    }
                }
            }*/
            return largestBlocks;
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                DataGridViewCell cell = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex];
                MemBlock block = dataGridView2.Rows[e.RowIndex].Tag as MemBlock;
                string ttText = "";
                string blockTag = block.GetTagString();
                if (blockTag.Length > 0)
                {
                    ttText = ttText + "Tag: " + blockTag + "\n----------------------\n";
                }

                uint[] callStack = CallstackTab.GetCallStack(block.m_callStackCRC);
                for (int i = 0; i < callStack.Length; ++i)
                {
                    IStackTracer.Symbol symbol = MemTracerForm.ms_MainForm.StackTracer.GetSymbolForAddress(callStack[i]);
                    if (symbol.functionName != null)
                        ttText = ttText + symbol.functionName + "\n";
                }
                cell.ToolTipText = ttText;
            }
        }
    }
}
