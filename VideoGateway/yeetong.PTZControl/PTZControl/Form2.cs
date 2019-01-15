using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PTZControl
{
    public partial class Form2 : Form
    {
        public Form2()
        {

            InitializeComponent();
        }
        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_APPWINDOW = 0x40000;
                const int WS_EX_TOOLWINDOW = 0x80;
                CreateParams cp = base.CreateParams;
                cp.ExStyle &= (~WS_EX_APPWINDOW);    // 不显示在TaskBar
                cp.ExStyle |= WS_EX_TOOLWINDOW;      // 不显示在Alt+Tab
                return cp;
            }
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            this.Visible = false;//去掉左下角的最小化的窗口
            Tcp_Client.Init();
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Tcp_Client.DisSocket();
        }
    }
}
