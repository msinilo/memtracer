using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MemTracer
{
    public partial class MemAllocTree : UserControl
    {
        public MemAllocTree()
        {
            InitializeComponent();
        }

        public void BuildTree(List<MemOperation> memOps)
        {
            m_tree.Clear();
            treeView1.Nodes.Clear();
            foreach (MemOperation op in memOps)
            {
                if (op.OpType == MemOperation.Type.Alloc)
                {
                    m_tree.AddMemBlock(op.UserData as MemBlock);
                }
            }
            PopulateTreeView(m_tree.Root, treeView1.Nodes);
        }
        public void BuildTree(MemSnapshot snapshot)
        {
            m_tree.Clear();
            treeView1.Nodes.Clear();
            foreach (MemBlock block in snapshot.Blocks.Values)
            {
                if (m_detailed)
                    m_tree.AddMemBlockSubTree(block);
                else
                    m_tree.AddMemBlock(block);
            }
            m_tree.Sort();
            treeView1.BeginUpdate();
            PopulateTreeView(m_tree.Root, treeView1.Nodes);
            treeView1.EndUpdate();
            m_snapshot = snapshot;
        }
        void BuildTree(MemSnapshot snapshot, int minMemOpNo, int maxMemOpNo)
        {
            m_tree.Clear();
            treeView1.Nodes.Clear();
            foreach (MemBlock block in snapshot.Blocks.Values)
            {
                int memOpNr = MemTracerForm.ms_MainForm.MemOperationNrForBlock(block);
                if (memOpNr < maxMemOpNo && memOpNr > minMemOpNo)
                {
                    if (m_detailed)
                        m_tree.AddMemBlockSubTree(block);
                    else
                        m_tree.AddMemBlock(block);
                }
            }
            m_tree.Sort();
            treeView1.BeginUpdate();
            PopulateTreeView(m_tree.Root, treeView1.Nodes);
            treeView1.EndUpdate();
            m_snapshot = snapshot;
        }

        void PopulateTreeView(MemBlockTree.Node tree, TreeNodeCollection nodes)
        {
            foreach (MemBlockTree.Node node in tree.Children)
            {
                nodes.Add(node.GetText());
                nodes[nodes.Count - 1].Tag = node;
                PopulateTreeView(node, nodes[nodes.Count - 1].Nodes);
            }
        }
        public void SetDoubleClickDelegate(DelegateDoubleClickNode dblClick)
        {
            m_doubleClickDelegate = dblClick;
        }
        public void SetSubtreeRootAddress(ulong addr)
        {
            m_tree.SubtreeRootAddress = addr;
            m_detailed = (addr != 0);
            if (m_detailed)
            {
                cbBottomUp.Visible = false;
                cbTagged.Visible = false;
            }
        }
        public void ExpandAll()
        {
            treeView1.ExpandAll();
        }

        public MemBlockTree m_tree = new MemBlockTree();
        DelegateDoubleClickNode m_doubleClickDelegate = null;
        MemSnapshot m_snapshot = null;
        bool m_detailed = false;

        private void cbBottomUp_CheckedChanged(object sender, EventArgs e)
        {
            m_tree.BottomUp = cbBottomUp.Checked;
            if (m_snapshot != null)
                BuildTree(m_snapshot);
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (m_doubleClickDelegate != null)
                m_doubleClickDelegate(sender, e);
        }

        private void cbTagged_CheckedChanged(object sender, EventArgs e)
        {
            m_tree.Tagged = cbTagged.Checked;
            if (m_snapshot != null)
                BuildTree(m_snapshot);
        }

        private void subtreeDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MemBlockTree.Node node = treeView1.SelectedNode.Tag as MemBlockTree.Node;
            if (node != null)
            {
                SubtreeDetails dlg = new SubtreeDetails(node.m_callAddress);
                dlg.BuildTree(m_snapshot);
                dlg.ShowDialog();
            }
        }

        public void FilterEntriesBetween(int minMemOpNo, int maxMemOpNo)
        {
            if (minMemOpNo < maxMemOpNo)
                BuildTree(m_snapshot, minMemOpNo, maxMemOpNo);
            else
                BuildTree(m_snapshot);
        }
    }
}
