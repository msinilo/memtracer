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
    public partial class ShowSnapshot : Form
    {
        public ShowSnapshot(MemSnapshot snap, string name)
        {
            InitializeComponent();

            Text = name;
            snapshotInfoControl1.Init(name);
            snapshotInfoControl1.Update(snap);
            memAllocTree1.BuildTree(snap);
            memAllocTree1.SetDoubleClickDelegate(new DelegateDoubleClickNode(DoubleClickNode));
        }
        public ShowSnapshot(List<MemOperation> ops, string name)
        {
            InitializeComponent();

            Text = name;
            snapshotInfoControl1.Init(name);
            //snapshotInfoControl1.Update(snap);
            memAllocTree1.BuildTree(ops);
            memAllocTree1.SetDoubleClickDelegate(new DelegateDoubleClickNode(DoubleClickNode));
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam); 

        private void ScrollToLine(uint line)
        {
            //line = 1;
            //string selectedLine = this.richTextBox1.Lines[line - 1];
            //int selStart = 0;
            //for (int i = 0; i < line - 1; ++i)
            //{
            //    // +1 == newline
            //    //string thisLine = richTextBox1.Lines[i];
            //    selStart += richTextBox1.Lines[i].Length + 1;
            //}

            //richTextBox1.Focus();
            //richTextBox1.SelectionLength = selectedLine.Length;
            //richTextBox1.SelectionStart = selStart;
            //richTextBox1.ScrollToCaret();

            SendMessage(richTextBox1.Handle, 0x00b6, 0, (int)line - 1);                      
        }
        private void OpenSourceFile(MemBlockTree.Node node)
        {
            if (node.Symbol.fileName == null)
            {
                label1.Text = "";
                return;
            }

            try
            {
                this.richTextBox1.LoadFile(node.Symbol.fileName, RichTextBoxStreamType.PlainText);
                ScrollToLine(node.Symbol.line);
                label1.Text = node.Symbol.fileName + "@" + node.Symbol.line;
            }
            catch (System.IO.IOException)
            {
                label1.Text = "";
            }
        }
        private void DoubleClickNode(object sender, System.EventArgs e)
        {
            TreeView tree = sender as TreeView;
            if (tree != null && tree.SelectedNode != null)
            {
                MemBlockTree.Node memNode = tree.SelectedNode.Tag as MemBlockTree.Node;
                OpenSourceFile(memNode);
            }
        }

    }
}