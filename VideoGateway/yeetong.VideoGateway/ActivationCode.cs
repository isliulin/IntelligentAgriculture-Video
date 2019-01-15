using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ToolAPI;

namespace yeetong_VideoGateway
{
    public partial class ActivationCode : Form
    {
        public ActivationCode(bool isEdit)
        {
            InitializeComponent();
            if (isEdit)
            {
                textBox1.Text = ValidateC.ActivationCode;
                textBox1.Enabled = false;
                pictureBox1.Visible = false;
                pictureBox2.Visible = true;
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            label5.Focus();
        }

        void txt_LostFocus(object sender, EventArgs e)
        {
            if (textBox1.Text == "激活码")
                textBox1.Text = "";
        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "")
                {
                    MessageBox.Show("激活码为空");
                }
                else
                {
                    if (PingToServer.IsNet)
                    {
                        string sql = "select * from smart_culture_client_activation where activation_id='" + textBox1.Text + "' and activation_status=0 and client_type='1'";
                        DataTable dt = MySQL_WCF.ExecuteDataTable(sql, null, CommandType.Text);




                        if (dt != null && dt.Rows.Count > 0)
                        {
                            int changeCount = int.Parse(dt.Rows[0]["change_count"].ToString())+1;
                            string Mac = ValidateC.GetMacAddressByNetworkInformation();
                            sql = "UPDATE smart_culture_client_activation SET activation_status = 1,win_mac='" + Mac + "',change_count=" + changeCount.ToString() + " WHERE activation_id='" + textBox1.Text + "' and client_type='1'";
                            int result = MySQL_WCF.ExecuteNoQuery(sql, null, CommandType.Text);
                            if (result > 0)
                            {
                                string projectidTemp = dt.Rows[0]["farm_id"].ToString();
                                string proname = dt.Rows[0]["farm_name"].ToString();
                                INIOperate.IniWriteValue("Identity", "isActivation", CryptoTool.Encrypt_DES("true", ValidateC.KEYT), AppDomain.CurrentDomain.BaseDirectory + "\\Config.ini");
                                INIOperate.IniWriteValue("Identity", "activationCode", CryptoTool.Encrypt_DES(textBox1.Text, ValidateC.KEYT), AppDomain.CurrentDomain.BaseDirectory + "\\Config.ini");
                                INIOperate.IniWriteValue("Identity", "mac", CryptoTool.Encrypt_DES(Mac, ValidateC.KEYT), AppDomain.CurrentDomain.BaseDirectory + "\\Config.ini");
                                INIOperate.IniWriteValue("Identity", "projectid", CryptoTool.Encrypt_DES(projectidTemp, ValidateC.KEYT), AppDomain.CurrentDomain.BaseDirectory + "\\Config.ini");
                                INIOperate.IniWriteValue("Identity", "projectName", CryptoTool.Encrypt_DES(proname, ValidateC.KEYT), AppDomain.CurrentDomain.BaseDirectory + "\\Config.ini");
                                ValidateC.IsActivation = true;
                                ValidateC.Mac = Mac;
                                ValidateC.Projectid = projectidTemp;
                                ValidateC.ActivationCode = textBox1.Text;
                                ValidateC.ProjectName = proname;
                                MessageBox.Show("激活成功，您可以正常使用了。");
                                this.Close();
                            }
                        }
                        else
                        {
                            MessageBox.Show("激活码无效或已被使用");
                        }
                    }
                    else
                    {
                        MessageBox.Show("失去网络连接");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("异常：" + ex.Message);
            }
        }

        private void ActivationCode_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateC.IsActivation)
                {
                    if (textBox1.Text == "")
                    {
                        MessageBox.Show("激活码为空");
                    }
                    else
                    {
                        if (PingToServer.IsNet)
                        {
                            string sql = "UPDATE smart_culture_client_activation SET activation_status = 0  WHERE activation_id='" + textBox1.Text + "'  and client_type='1'";
                            int result = MySQL_WCF.ExecuteNoQuery(sql, null, CommandType.Text);
                            if (result > 0)
                            {
                                INIOperate.IniWriteValue("Identity", "isActivation", CryptoTool.Encrypt_DES("false", ValidateC.KEYT), AppDomain.CurrentDomain.BaseDirectory + "\\Config.ini");
                                ValidateC.IsActivation = false;
                                MessageBox.Show("撤销成功");
                                System.Environment.Exit(0);
                            }
                        }
                        else
                        {
                            MessageBox.Show("请先连接网络");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("当前处于非激活状态");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("异常：" + ex.Message);
            }
        }



        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
