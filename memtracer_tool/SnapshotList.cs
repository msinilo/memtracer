using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MemTracer
{
    public partial class SnapshotList : UserControl
    {
        public SnapshotList()
        {
            InitializeComponent();
        }

        public void SetMainForm(MemTracerForm frm)
        {
            m_mainForm = frm;
        }

        public void AddSnapshot(string name, uint bytes, int blocks, int memOpNr)
        {
            float mb = (float)bytes / (1024 * 1024);
            string usedMem = mb.ToString("N");
            string[] rowData = { name, usedMem, blocks.ToString(), memOpNr.ToString() };
            dataGridView1.Rows.Add(rowData);
        }

        private void compareSnapshotsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 2 && m_delCompareSnapshots != null)
            {
                m_delCompareSnapshots(dataGridView1.SelectedRows[1].Index, 
                    dataGridView1.SelectedRows[0].Index);
            }
        }

        public DelegateCompareSnapshots OnCompareSnapshots
        {
            get { return m_delCompareSnapshots; }
            set { m_delCompareSnapshots = value; }
        }
        public DelegateOverlapSnapshots OnOverlapSnapshots
        {
            get { return m_delOverlapSnapshots; }
            set { m_delOverlapSnapshots = value; }
        }
        DelegateCompareSnapshots m_delCompareSnapshots = null;
        DelegateOverlapSnapshots m_delOverlapSnapshots = null;
        MemTracerForm m_mainForm = null;

        private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                ShowSnapshot dlg = new ShowSnapshot(m_mainForm.GetSnapshotByIndex(dataGridView1.SelectedCells[0].RowIndex),
                    m_mainForm.GetSnapshotNameByIndex(dataGridView1.SelectedCells[0].RowIndex));
                dlg.ShowDialog();
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            detailsToolStripMenuItem.Enabled = dataGridView1.SelectedRows.Count == 1;
            compareSnapshotsToolStripMenuItem.Enabled = dataGridView1.SelectedRows.Count == 2;
            deleteToolStripMenuItem.Enabled = dataGridView1.SelectedRows.Count == 1;
            overlapToolStripMenuItem.Enabled = dataGridView1.SelectedRows.Count == 2;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.SelectedCells.Count; ++i)
            {
                m_mainForm.DeleteSnapshot(dataGridView1.SelectedCells[i].RowIndex);
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedCells[i].RowIndex);
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.SelectedCells.Count != 0)
                m_mainForm.OnSnapshotSelected(dataGridView1.SelectedCells[0].RowIndex);
        }

        private void overlapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 2 && m_delOverlapSnapshots != null)
            {
                m_delOverlapSnapshots(dataGridView1.SelectedCells[1].RowIndex,
                    dataGridView1.SelectedCells[0].RowIndex);
            }
        }
    }
}
