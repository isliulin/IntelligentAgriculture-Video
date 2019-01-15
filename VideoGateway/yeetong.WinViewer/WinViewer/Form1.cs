using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxRDPCOMAPILib;
using System.Threading;
using RDPCOMAPILib;

namespace WinViewer
{
    public partial class 远程桌面控制端 : Form
    {
        bool isvideo = true;
        Dictionary<string, DataRow> videoprolistdictionary = new Dictionary<string, DataRow>();
        Dictionary<string, string> videoeyeprolistdictionary = new Dictionary<string, string>();
        Dictionary<string, DataRow> videoeyeconnectlistdictionary = new Dictionary<string, DataRow>();
        string connectstring = "";
        bool isdis = true;
        public 远程桌面控制端()
        {
            InitializeComponent();
            Update_comboBox1List();
        }

        #region 数据库相关
        //刷新工地列表
        private void Update_comboBox1List()
        {
            if (isvideo)
            {
                string sql = String.Format("select farm_id,farm_name,remote_connection_str,remote_time from smart_culture_client_activation where client_type='1' GROUP BY farm_id");
                DataTable result = MySQL_WCF.ExecuteDataTable(sql, null, CommandType.Text);
                videoprolistdictionary.Clear();
                comboBox1.Items.Clear();
                if (result != null && result.Rows.Count > 0)
                {
                    comboBox1.Items.Add("请选择工地");
                    for (int i = 0; i < result.Rows.Count; i++)
                    {
                        string proname = result.Rows[i]["farm_name"].ToString();
                        comboBox1.Items.Add(proname);
                        if (videoprolistdictionary.Keys.Contains(proname))
                            videoprolistdictionary[proname] = result.Rows[i];
                        else
                            videoprolistdictionary.Add(proname, result.Rows[i]);
                    }

                }
                else
                    comboBox1.Items.Add("没有找到任何农场");
                comboBox1.SelectedIndex = 0;
            }
        }
        #endregion

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            isvideo = true;
            Update_comboBox1List();
            label5.Text = "";
            label5.Tag = "0";
            closeconnect();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text != "" && comboBox1.Text != "请选择工地")
            {
                if (isvideo)//确认是否存在连接字符串
                {
                    if (videoprolistdictionary.ContainsKey(comboBox1.Text))
                    {
                        DataRow dr = videoprolistdictionary[comboBox1.Text];
                        if (string.IsNullOrEmpty(dr["remote_connection_str"].ToString()))
                        {
                            label5.Text = "没有找到连接字符串";
                            label5.Tag = "0";
                        }
                        else
                        {
                            DateTime dti = new DateTime();
                            if (DateTime.TryParse(dr["remote_time"].ToString(), out dti))
                            {
                                if ((DateTime.Now - dti).TotalSeconds > 95)
                                {
                                    label5.Text = "远程终端离线";
                                    label5.Tag = "0";
                                }
                                else
                                {
                                    label5.Text = "远程终端已就绪：" + dti.ToString("HH:mm:ss");
                                    label5.Tag = "1";
                                    connectstring = dr["remote_connection_str"].ToString();
                                }
                            }
                            else
                            {
                                label5.Text = "远程终端未启动";
                                label5.Tag = "0";
                            }
                        }
                    }
                }
            }
            closeconnect();
        }
        private void connetbutton_Click(object sender, EventArgs e)
        {
            if (connetbutton.Text=="连接"&& label5.Tag.ToString() == "1" && !string.IsNullOrEmpty(connectstring))
            {
                if(openconnect())
                {
                    connetbutton.Text = "断开连接";
                }
            }
            else if(connetbutton.Text == "断开连接")
            {
                closeconnect();
                label5.Text = "已断开远程桌面";
                connetbutton.Text = "连接";
            }
            else
                MessageBox.Show("终端未就绪或条件不具备");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Update_comboBox1List();
        }

        bool openconnect()
        {
            try
            {
                pRdpViewer.Connect(connectstring, "Viewer1", "");//绑定
                label5.Text = "正在连接远程电脑";
                label5.Refresh();
                panel2.Visible = false;
                isdis = false;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("openconnect异常" + ex.Message);
                return false;
            }

        }
        void closeconnect()
        {
            try
            {
                panel2.Visible = true;
                connetbutton.Text = "连接";
                if (!isdis)
                {

                    pRdpViewer.Disconnect();
                    isdis = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("closeconnect异常" + ex.Message);
            }
        }

        #region 窗体动画
        //关闭程序
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
            System.Environment.Exit(0);
        }
        private bool isMouseDown = false;
        private Point FormLocation;     //form的location
        private Point mouseOffset;      //鼠标的按下位置
        private void FormMove_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = true;
                FormLocation = this.Location;
                mouseOffset = Control.MousePosition;
            }
        }
        private void FormMove_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        private void FormMove_MouseMove(object sender, MouseEventArgs e)
        {
            int _x = 0;
            int _y = 0;
            if (isMouseDown)
            {
                Point pt = Control.MousePosition;
                _x = mouseOffset.X - pt.X;
                _y = mouseOffset.Y - pt.Y;

                this.Location = new Point(FormLocation.X - _x, FormLocation.Y - _y);
            }
        }




        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                pRdpViewer.RequestControl(RDPCOMAPILib.CTRL_LEVEL.CTRL_LEVEL_INTERACTIVE);//开启控制
                label5.Text = "正在控制远程电脑";
                label5.Refresh();
                panel2.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("openconnect异常" + ex.Message);
            }
           
        }
    }
}
