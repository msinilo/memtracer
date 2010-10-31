using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;


namespace MemTracer
{
    public partial class ConnectDialog : Form
    {
        public ConnectDialog(int portNr)
        {
            InitializeComponent();
            m_portNr = portNr;
        }

        public String GetServer()
        {
            if (IPOnlyNumbersAndDots(tbServer.Text))
                return tbServer.Text;
            else
                return GetIP(tbServer.Text);
        }
        public int GetPort()
        {
            return Convert.ToInt32(tbPort.Text);
        }

        private static String GetLocalIP()
        {
            String strHostName = Dns.GetHostName();
            return GetIP(strHostName);
        }
        private static String GetIP(String strHostName)
        {
            // Find host by name
            IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);

            // Grab the first IP addresses
            String IPStr = "";
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                IPStr = ipaddress.ToString();
                if (IPOnlyNumbersAndDots(IPStr))
                {
                    return IPStr;
                }
            }
            return IPStr;
        }

        private void ConnectDialog_Shown(object sender, EventArgs e)
        {
            tbServer.Text = GetLocalIP();
            tbPort.Text = m_portNr.ToString();
        }
        int m_portNr;

        private void tbPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private static bool IPOnlyNumbersAndDots(String s)
        {
            for (int i = 0; i < s.Length; ++i)
            {
                if (!char.IsDigit(s[i]) && s[i] != '.')
                    return false;
            }
            return true;
        }
    }
}
