using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace MemTracer
{
    public partial class TracedVars : Form
    {
        public TracedVars(Dictionary<string, int> tracedVars)
        {
            InitializeComponent();
            m_vars = tracedVars;
            RefreshVars();
        }

        void RefreshVars()
        {
            lock (m_vars)
            {
                lbVars.Items.Clear();
                foreach (String key in m_vars.Keys)
                {
                    lbVars.Items.Add(key + ": " + m_vars[key].ToString());
                }
            }
        }
        Dictionary<string, int> m_vars = null;
        Object m_lock = new Object();

        private void timer1_Tick(object sender, EventArgs e)
        {
            RefreshVars();
        }
    }
}
