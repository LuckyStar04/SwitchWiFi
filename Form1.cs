using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SwitchWiFi
{
    public partial class Form1 : Form
    {

        [DllImport("user32")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint control, Keys vk);
        [DllImport("user32")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private cmd c = new cmd();
        private string status;

        private bool buttonstatus;
        #region buttonstatus
        private bool ButtonStatus
        {
            get
            {
                return buttonstatus;
            }
            set
            {
                buttonstatus = value;
                switch (value)
                {
                    case true:
                        {
                            btnSwitch.Location = new Point(84, 3);
                            btnSwitch.ForeColor = Color.SeaGreen;
                            btnSwitch.Text = "On";
                            panel1.BackColor = Color.SeaGreen;
                            this.BackColor = Color.LightSeaGreen;
                            ntfOpenWiFi.Icon = Properties.Resources.wifi_color;
                        }
                        break;
                    case false:
                        {
                            btnSwitch.Location = new Point(3, 3);
                            btnSwitch.ForeColor = Color.Brown;
                            btnSwitch.Text = "Off";
                            panel1.BackColor = Color.Brown;
                            this.BackColor = Color.LightCoral;
                            ntfOpenWiFi.Icon = Properties.Resources.wifi_gray;
                        } break;
                }
            }
        }
        #endregion

        private bool registersuccess;

        public Form1()
        {
            InitializeComponent();
        }

        private void Item_Launch_Click(object sender, EventArgs e)
        {
            switch (ButtonStatus)
            {
                case false:
                    {
                        status = c.execute("netsh wlan start hostednetwork");
                        if (status.Contains("已启动"))
                        {
                            ntfOpenWiFi.ShowBalloonTip(1, "Wi-Fi 发射", "发射已打开。", ToolTipIcon.Info);
                            ButtonStatus = true;
                        }
                    }
                    break;
                case true:
                        ntfOpenWiFi.ShowBalloonTip(1, "Wi-Fi 发射", "发射已打开。", ToolTipIcon.Info); break;
            }
        }

        private void Item_Shutdown_Click(object sender, EventArgs e)
        {
            switch (ButtonStatus)
            {
                case false:
                        ntfOpenWiFi.ShowBalloonTip(1, "Wi-Fi 发射", "发射已关闭。", ToolTipIcon.Info); break;
                case true:
                    {
                        status = c.execute("netsh wlan stop hostednetwork");
                        if (status.Contains("已停止"))
                        {
                            ntfOpenWiFi.ShowBalloonTip(1, "Wi-Fi 发射", "发射已关闭。", ToolTipIcon.Info);
                            ButtonStatus = false;
                        }
                    }
                    break;
            }
        }

        private void Item_Exit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定退出吗？", "Wi-Fi发射控制台", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK) Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            registersuccess = false;
            timer1.Start();
            ReadStatus();
            registersuccess = RegisterHotKey(this.Handle, 53812, 0x0002, Keys.OemQuestion);
            Application.ApplicationExit += Application_ApplicationExit;
            switch(ButtonStatus)
            {
                case(true):
                    ntfOpenWiFi.ShowBalloonTip(1, "Wi-Fi 发射", "发射已打开。", ToolTipIcon.Info);break;
                case (false):
                    ntfOpenWiFi.ShowBalloonTip(1, "Wi-Fi 发射", "发射已关闭。", ToolTipIcon.Info); break;
            }
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            timer1.Stop();
            ntfOpenWiFi.Dispose();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (registersuccess) this.Visible = false;
            else
            {
                label2.Visible = true;
            }
        }

        private void ntfOpenWiFi_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
            }
            else if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }

        private void btnSwitch_Click(object sender, EventArgs e)
        {
            switch (ButtonStatus)
            {
                case false:
                    {
                        status = c.execute("netsh wlan start hostednetwork");
                        if (status.Contains("已启动"))
                        {
                            ntfOpenWiFi.ShowBalloonTip(1, "Wi-Fi 发射", "发射已打开。", ToolTipIcon.Info);
                            ButtonStatus = true;
                        }
                    }
                    break;
                case true:
                    {
                        status = c.execute("netsh wlan stop hostednetwork");
                        if (status.Contains("已停止"))
                        {
                            ntfOpenWiFi.ShowBalloonTip(1, "Wi-Fi 发射", "发射已关闭。", ToolTipIcon.Info);
                            ButtonStatus = false;
                        }
                    }
                    break;
            }
        }

        private void ReadStatus()
        {
            status = c.execute("netsh wlan show hostednetwork");
            if (status.Contains("已启动"))
                ButtonStatus = true;
            else
                ButtonStatus = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ReadStatus();
        }

        private void Form1_VisibleChanged(object sender, EventArgs e)
        {
            switch (this.Visible)
            {
                case true: timer1.Interval = 1000; break;
                case false: timer1.Interval = 600000; break;
            }
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
        }


        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x0312:
                    if (m.WParam.ToString().Equals("53812"))
                    {
                        btnSwitch_Click(this, new EventArgs());
                        this.Show();
                        this.WindowState = FormWindowState.Normal;
                        this.Activate();
                        //status = c.execute("netsh wlan start hostednetwork");
                        //if (status.Contains("已启动"))
                        //    ButtonStatus = true;
                        //SwitchButton();
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ntfOpenWiFi.Visible = false;
            UnregisterHotKey(this.Handle, 53812);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "确定退出吗？", "Wi-Fi发射控制台", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK) Application.Exit();
        }

        private void label3_MouseEnter(object sender, EventArgs e)
        {
            label3.ForeColor = Color.SteelBlue;
        }

        private void label3_MouseLeave(object sender, EventArgs e)
        {
            label3.ForeColor = Color.White;
        }

        private void ntfOpenWiFi_BalloonTipClicked(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }
    }
}
