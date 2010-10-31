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
        public SubtreeDetails(uint rootAddress)
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
                memAllocTree1.FilterOutEntriesNewerThan(System.Int32.Parse(tbFilter.Text));
            }
            else if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            memAllocTree1.FilterOutEntriesNewerThan(System.Int32.Parse(tbFilter.Text));
        }
    }
}