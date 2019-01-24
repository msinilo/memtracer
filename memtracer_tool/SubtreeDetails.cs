using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MemTracer
{
    public partial class SubtreeDetails : Form
    {
        public SubtreeDetails(ulong rootAddress)
        {
            InitializeComponent();
            memAllocTree1.SetSubtreeRootAddress(rootAddress);
        }

        public void BuildTree(MemSnapshot snap)
        {
            memAllocTree1.BuildTree(snap);
            memAllocTree1.ExpandAll();
        }

        private void tbFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                memAllocTree1.FilterEntriesBetween(System.Int32.Parse(tbFilterMin.Text), System.Int32.Parse(tbFilterMax.Text));
            }
            else if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            memAllocTree1.FilterEntriesBetween(System.Int32.Parse(tbFilterMin.Text), System.Int32.Parse(tbFilterMax.Text));
        }
    }
}