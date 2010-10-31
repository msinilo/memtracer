using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MemTracer
{
    public partial class NewSnapshot : Form
    {
        public NewSnapshot()
        {
            InitializeComponent();
        }

        public string GetName()
        {
            return textBox1.Text;
        }
    }
}