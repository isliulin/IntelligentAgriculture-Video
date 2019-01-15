using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AccessOperate;

namespace yeetong_VideoGateway
{
    public partial class Form1 : Form
    {
        Process pr;//声明一个进程类对象
        Process PTZpr;//声明一个进程类对象
        Process AppAutoUpdtae;//声明一个进程类对象
       
        string SVGID = "";
        string proName = "";
        bool isfirst = true;

        bool IsStop = false;

        public Form1()
        {
            ValidateC.ValidateCInit();
            InitializeComponent();
            label1.Text = "版权：北京一通无限科技有限公司 " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            label12.Text = ValidateC.ProjectName;
            UpdatePushflowstatus("-1", "0");
            UpdateVSG();
            Tcp_Client.Init();
            Thread UpdateCLT = new Thread(UpdateCL) { IsBackground = true, Priority = ThreadPriority.Highest };
            UpdateCLT.Start();
            Synchro.Init();

            //开启远程桌面
            Thread CheckT = new Thread(remote.Check) { IsBackground = true, Priority = ThreadPriority.Highest };
            CheckT.Start();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //定时刷新开启
            timer1.Enabled = true;
            timer1.Start();
            //视频流传输开启
            if (!isRun())
            {
                pr = new Process();
                pr.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + @"CLR_C\Exercise2.exe";
                pr.Start();
            }
            pictureBox11.Tag = "1";
            pictureBox11.Image = global::yeetong_VideoGateway.Properties.Resources.CLR_R;
            toolTip1.SetToolTip(this.pictureBox11, "视频传输正在运行");

            timer3.Enabled = true;
            timer3.Start();
            //QClient.CallQueuing.Clear();//清空呼叫队列
            try
            {
                string strtemp = "update vsg_FIB set callClientCount='0'";
                int result = AccessOperate.AccessHelper.InsertData(strtemp);
            }
            catch (Exception ex) { }

            StartPTZ();//PTZ的相关还包括自动更新的软件以及视频流的软件的查看开启和关闭
            pictureBox9.Tag = "1";
            pictureBox9.Image = global::yeetong_VideoGateway.Properties.Resources.PTZ_R;
            toolTip1.SetToolTip(this.pictureBox9, "视频传输正在运行");
           
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (pr != null)
                {
                    pr.Kill();
                    pr = null;
                }
                setKill();
                setKillPTZ();
            }
            catch (Exception) { }
        }

        #region 更改项目信息
        //更改项目编号
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //1、先判断传输流是否在工作，在工作就关闭，不再工作就无所谓
                remind("监测流传输是否在工作");
                //QClient.CallQueuing.Clear();//清空呼叫队列
                try
                {
                    string strtemp = "update vsg_FIB set callClientCount='0'";
                    int result = AccessOperate.AccessHelper.InsertData(strtemp);
                }
                catch (Exception ex) { }
                if (pictureBox11.Tag.ToString() == "1")
                {
                    timer3.Stop();
                    timer3.Enabled = false;
                    if (pr != null)
                    {
                        pr.Kill();
                        pr = null;
                    }
                    setKill();
                    pictureBox11.Image = global::yeetong_VideoGateway.Properties.Resources.CLR_S;
                    pictureBox11.Refresh();
                    toolTip1.SetToolTip(this.pictureBox11, "视频传输未启动");
                    UpdatePushflowstatus("-1", "0");
                    //QClient.CallQueuing.Clear();//清空呼叫队列
                    remind("暂时关闭流传输");
                }
                remind("获取摄像头列表");

                if (ValidateC.Projectid != "")
                {
                    CameraSon[] clist = CameraMain.GetCameraList(ValidateC.Projectid);
                    if (clist != null && clist.Length > 0)
                    {
                        foreach (CameraSon cTemp in clist)
                        {
                            string strtemp = "select * from vsg_FIB where deviceid = '" + cTemp.id + "'";
                            DataTable dt = AccessOperate.AccessHelper.GetTable(strtemp);
                            if (dt != null && dt.Rows.Count > 0)//存在就更新
                            {
                                try
                                {
                                    StringBuilder str = new StringBuilder();
                                    str.Append("UPDATE  vsg_FIB set ");
                                    str.Append("devicename='").Append(cTemp.name).Append("',");
                                    str.Append("devicelocation='").Append("视频流").Append("',");
                                    str.Append("pushflowmode='").Append(cTemp.pushflowmode).Append("',");
                                    str.Append("projectid='").Append(ValidateC.Projectid).Append("',");
                                    str.Append("rtspurl='").Append(cTemp.position_desc).Append("',");
                                    str.Append("remarks='").Append(cTemp.IP).Append("',");
                                    str.Append("rtmpurl='").Append(cTemp.stream_url).Append("'");
                                    str.Append(" where deviceid = '").Append(cTemp.id).Append("'");
                                    string sql = str.ToString();
                                    int result = AccessOperate.AccessHelper.InsertData(sql);
                                    if (!(result > 0))
                                    {
                                        //ToolAPI.XMLOperation.WriteLogXmlNoTail("更新摄像机参数失败", cTemp.id);
                                        remind("更新摄像头列表失败" + cTemp.id);
                                    }
                                    else
                                    {
                                        remind("更新摄像头列表成功" + cTemp.id);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    remind("更新摄像头" + cTemp.id + "异常：" + ex.Message);
                                }
                            }
                            else//不存在就插入
                            {
                                try
                                {
                                    StringBuilder str = new StringBuilder();
                                    str.Append("INSERT INTO vsg_FIB (deviceid,devicename,devicelocation,pushflowmode,projectid,rtspurl,remarks,rtmpurl) VALUES (");
                                    str.Append("'").Append(cTemp.id).Append("',");
                                    str.Append("'").Append(cTemp.name).Append("',");

                                    str.Append("'").Append("").Append("',");
                                    str.Append("'").Append(cTemp.pushflowmode).Append("',");
                                    str.Append("'").Append(ValidateC.Projectid).Append("',");
                                    str.Append("'").Append(cTemp.position_desc).Append("',");
                                    str.Append("'").Append(cTemp.IP).Append("',");
                                    str.Append("'").Append(cTemp.stream_url).Append("'");
                                    str.Append(")");
                                    string sql = str.ToString();
                                    int result = AccessOperate.AccessHelper.InsertData(sql);
                                    if (!(result > 0))
                                    {
                                        //ToolAPI.XMLOperation.WriteLogXmlNoTail("更新摄像机参数失败", cTemp.id);
                                        remind("插入摄像头列表失败" + cTemp.id);
                                    }
                                    else
                                    {
                                        remind("插入摄像头列表成功" + cTemp.id);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    remind("添加摄像头" + cTemp.id + "异常：" + ex.Message);
                                }
                            }
                        }
                    }
                }
                else
                {
                    //2
                    remind("清除原项目残留");
                    String sql = "delete from vsg_FIB ";
                    int result = AccessOperate.AccessHelper.InsertData(sql);
                    if (result > 0)
                    {
                        remind("清除原项目残留完成");
                    }
                }
                //4
                remind("更新界面元素");
                UpdateVSG();
                label12.Text = ValidateC.ProjectName;
                remind("监测流传输复原到上一状态");
                if (pictureBox11.Tag.ToString() == "1")
                {
                    if (!isRun())
                    {
                        pr = new Process();
                        pr.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + @"CLR_C\Exercise2.exe";
                        pr.Start();
                    }
                    pictureBox11.Image = global::yeetong_VideoGateway.Properties.Resources.CLR_R;
                    pictureBox11.Refresh();
                    toolTip1.SetToolTip(this.pictureBox11, "视频传输正在运行");
                    timer3.Enabled = true;
                    timer3.Start();
                }

                //MessageBox.Show("更新完成,请检查拉流地址和推流地址是否正确，以免影响正常传输");
                label2.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作出现异常：" + ex.Message);
            }
        }
        //更改项目编号
        void remind(string flag) { label2.Visible = true; label2.Text = "进度提醒：" + flag; label2.Refresh(); }
        #endregion

        #region 窗体移动 和变换

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            //this.WindowState = FormWindowState.Minimized;
            this.Hide(); //1
            this.notifyIcon1.Visible = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                if (pr != null)
                {
                    pr.Kill();
                    pr = null;
                }
                setKill();
                setKillPTZ();
            }
            catch (Exception) { }
            System.Environment.Exit(0);
        }
        private bool isMouseDown = false;
        private Point FormLocation;     //form的location
        private Point mouseOffset;      //鼠标的按下位置
        private void FormMove_MouseDown(object sender, MouseEventArgs e)
        {

        }
        private void FormMove_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        private void FormMove_MouseMove(object sender, MouseEventArgs e)
        {

        }
        #endregion

        #region 更改转发表
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string SelectDevSn1 = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                if (SelectDevSn1 != "")
                {
                    SVGID = SelectDevSn1;
                    UpdateBase(SVGID);
                }
                else
                {
                    MessageBox.Show("点击的行得到的设备编号不合法");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作出现异常：" + ex.Message);
            }
        }
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            //panel3.Visible = false;
        }
        void UpdateVSG()
        {
            try
            {
                //String sql = "select id as 序号,deviceid as 设备标识,devicename as 设备名称,devicelocation as 设备位置,pushflowmode as 推流模式,rtspurl as 拉流地址,rtmpurl as 推流地址,iscall as 呼叫状态,pushflowstatus as 推流状态,remarks  as 备注 from vsg_FIB";
                String sql = "select deviceid as 设备标识,devicename as 设备名称, switch(pushflowmode='1','长期',pushflowmode='0','呼叫')  as 推流模式,switch(iscall='0','未呼叫',iscall='1','正在呼叫') as 呼叫状态,obligate_1 as 更新时间,switch(pushflowstatus='-1','未启动',pushflowstatus='0','正在连接初始化',pushflowstatus='1','正在推流',pushflowstatus='2','推流异常或失败',pushflowstatus='3','重新唤醒中',pushflowstatus='4','流地址为空') as 推流状态,remarks  as 备注 from vsg_FIB";
                DataTable dt = AccessOperate.AccessHelper.GetTable(sql);
                dataGridView1.DataSource = dt;
                dataGridView1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作出现异常：" + ex.Message);
            }
        }
        void UpdateBase(string SVGID )
        {
            try
            {
                String sql = "select deviceid ,devicename ,devicelocation ,pushflowmode ,rtspurl ,rtmpurl ,iscall ,pushflowstatus ,remarks   from vsg_FIB where deviceid='" + SVGID + "'";
                DataTable dt = AccessOperate.AccessHelper.GetTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    panel3.Visible = true;
                    textBox2.Text = dt.Rows[0]["deviceid"].ToString();
                    textBox3.Text = dt.Rows[0]["devicename"].ToString();
                    textBox5.Text = dt.Rows[0]["rtspurl"].ToString();
                    textBox6.Text = dt.Rows[0]["rtmpurl"].ToString();
                }
            }
            catch(Exception ex)
            {

            }
        }
        #endregion


        //定时器事件
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isfirst)
            {
                isfirst = false;
                this.Hide();
            }
            UpdateVSG();

        }


        //更新所有流传输的状态
        int UpdatePushflowstatus(string pushflowstatus, string iscall)
        {
            StringBuilder str = new StringBuilder();
            str.Append("UPDATE  vsg_FIB set ");
            str.Append("pushflowstatus='").Append(pushflowstatus).Append("'");
            str.Append(", isupdate='1'");
            str.Append(", iscall='").Append(iscall).Append("'");
            string sql = str.ToString();
            int result = AccessOperate.AccessHelper.InsertData(sql);
            return result;
        }
        //遍历哪个进程是否被人为关闭了
        private void timer3_Tick(object sender, EventArgs e)
        {
            timer3.Stop();
            timer3.Enabled = false;
            if (isRun())
            {
                pictureBox11.Tag = "1";
                pictureBox11.Image = global::yeetong_VideoGateway.Properties.Resources.CLR_R;
                toolTip1.SetToolTip(this.pictureBox11, "视频传输正在运行");
            }
            else
            {
                pictureBox11.Tag = "0";
                pictureBox11.Image = global::yeetong_VideoGateway.Properties.Resources.CLR_S;
                toolTip1.SetToolTip(this.pictureBox11, "视频传输未启动");
                //UpdatePushflowstatus("-1", "0");
            }
            if (isRunPTZ())
            {
                pictureBox9.Tag = "1";
                pictureBox9.Image = global::yeetong_VideoGateway.Properties.Resources.PTZ_R;
                toolTip1.SetToolTip(this.pictureBox9, "云台控制正在运行");
            }
            else
            {
                pictureBox9.Tag = "1";
                pictureBox9.Image = global::yeetong_VideoGateway.Properties.Resources.PTZ_S;
                toolTip1.SetToolTip(this.pictureBox9, "云台控制未启动");
            }

            if (remote.IsStart)
            {
                pictureBox7.Image = global::yeetong_VideoGateway.Properties.Resources.remoteopen;
                toolTip1.SetToolTip(this.pictureBox9, "远程打开");
            }
            else
            {
                pictureBox7.Image = global::yeetong_VideoGateway.Properties.Resources.remoteclose;
                toolTip1.SetToolTip(this.pictureBox9, "远程关闭");
            }

            timer3.Enabled = true;
            timer3.Start();
        }

        //查看那个软件是否正在运行
        bool isRun()
        {
            try
            {
                Process[] processList = System.Diagnostics.Process.GetProcesses();
                foreach (Process p in processList)
                {
                    if (p.ProcessName.Contains("Exercise2"))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            { return false; }
        }
        //查看远程的有没有被打开
    

        bool setKill()
        {
            try
            {
                Process[] processList = System.Diagnostics.Process.GetProcesses();
                foreach (Process p in processList)
                {
                    if (p.ProcessName.Contains("Exercise2"))
                    {
                        p.Kill();
                        return true;
                    }
                }
                return true;
            }
            catch (Exception)
            { return false; }
        }
   
        //网络遍历 是否正常
        private void timer4_Tick(object sender, EventArgs e)
        {
            timer4.Stop();
            timer4.Enabled = false;
            try
            {
                if (!PingToServer.IsNet)//连着网
                {
                    UpdatePushflowstatus("-1", "0");
                    //QClient.CallQueuing.Clear();//清空呼叫队列
                    try
                    {
                        string strtemp = "update vsg_FIB set callClientCount='0'";
                        int result = AccessOperate.AccessHelper.InsertData(strtemp);
                    }
                    catch (Exception ex) { }
                    pictureBox5.Image = global::yeetong_VideoGateway.Properties.Resources.NETS;
                    toolTip1.SetToolTip(this.pictureBox5, "网络失去连接");
                    pictureBox5.Refresh();
                }
                else
                {
                    pictureBox5.Image = global::yeetong_VideoGateway.Properties.Resources.NET;
                    toolTip1.SetToolTip(this.pictureBox5, "网络正常");
                    pictureBox5.Refresh();
                }
            }
            catch (Exception) { }
            timer4.Enabled = true;
            timer4.Start();
        }


        #region 更新这个工地的摄像头列表
        void UpdateCL()
        {
            while (true)
            {
                try
                {

                    if (PingToServer.IsNet)
                    {
                        CameraSon[] clist = CameraMain.GetCameraList(ValidateC.Projectid);
                        if (clist != null && clist.Length > 0)
                        {
                            string idList = "&";
                            foreach (CameraSon cTemp in clist)
                            {
                                string strtemp = "select * from vsg_FIB where deviceid = '" + cTemp.id + "'";
                                DataTable dt = AccessOperate.AccessHelper.GetTable(strtemp);
                                if (dt != null && dt.Rows.Count > 0)//存在就更新
                                {
                                    try
                                    {
                                        StringBuilder str = new StringBuilder();
                                        str.Append("UPDATE  vsg_FIB set ");
                                        str.Append("devicename='").Append(cTemp.name).Append("',");
                                        str.Append("devicelocation='").Append("视频流").Append("',");
                                        str.Append("pushflowmode='").Append(cTemp.pushflowmode).Append("',");
                                        str.Append("projectid='").Append(ValidateC.Projectid).Append("',");
                                        str.Append("rtspurl='").Append(cTemp.position_desc).Append("',");
                                        str.Append("remarks='").Append(cTemp.IP).Append("',");
                                        str.Append("rtmpurl='").Append(cTemp.stream_url).Append("'");


                                        str.Append(" where deviceid = '").Append(cTemp.id).Append("'");
                                        string sql = str.ToString();
                                        int result = AccessOperate.AccessHelper.InsertData(sql);
                                        if (!(result > 0))
                                        {
                                            ToolAPI.XMLOperation.WriteLogXmlNoTail("更新摄像机参数失败", cTemp.id);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ToolAPI.XMLOperation.WriteLogXmlNoTail("更新摄像机参数异常", cTemp.id + ";" + ex.Message);
                                    }
                                }
                                else//不存在就插入
                                {
                                    try
                                    {
                                        StringBuilder str = new StringBuilder();
                                        str.Append("INSERT INTO vsg_FIB (deviceid,devicename,devicelocation,pushflowmode,projectid,rtspurl,remarks,rtmpurl) VALUES (");
                                        str.Append("'").Append(cTemp.id).Append("',");
                                        str.Append("'").Append(cTemp.name).Append("',");
                                        str.Append("'").Append("").Append("',");
                                        str.Append("'").Append(cTemp.pushflowmode).Append("',");
                                        str.Append("'").Append(ValidateC.Projectid).Append("',");
                                        str.Append("'").Append(cTemp.position_desc).Append("',");
                                        str.Append("'").Append(cTemp.IP).Append("',");
                                        str.Append("'").Append(cTemp.stream_url).Append("'");
                                        str.Append(")");
                                        string sql = str.ToString();
                                        int result = AccessOperate.AccessHelper.InsertData(sql);
                                        if (!(result > 0))
                                        {
                                            ToolAPI.XMLOperation.WriteLogXmlNoTail("插入摄像机参数失败", cTemp.id);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ToolAPI.XMLOperation.WriteLogXmlNoTail("插入摄像机参数异常", cTemp.id + ";" + ex.Message);
                                    }
                                }
                                Thread.Sleep(100);
                                idList += cTemp.id + "&";
                            }
                            string deldetestrtemp = "delete from vsg_FIB where Instr('" + idList + "','&'+deviceid+'&')<=0";
                            int deleteresult = AccessOperate.AccessHelper.InsertData(deldetestrtemp);
                        }
                        else
                        {
                            string deldetestrtemp = "delete from vsg_FIB where Instr('','&'+deviceid+'&')<=0";
                            int deleteresult = AccessOperate.AccessHelper.InsertData(deldetestrtemp);
                        }
                    }
                }
                catch (Exception)
                { }
                Thread.Sleep(60000);
            }
        }
        #endregion

        #region PTZ
        void StartPTZ()
        {
            try
            {
                if (!isRunPTZ())
                {
                    PTZpr = new Process();
                    PTZpr.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + @"PTZ\PTZControl.exe";
                    PTZpr.Start();
                }
                Thread SelfTestPTZT = new Thread(SelfTestPTZ) { IsBackground = true };
                SelfTestPTZT.Start();
                ToolAPI.XMLOperation.WriteLogXmlNoTail("PTZ启动", "启动成功");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("PTZ启动", "启动异常" + ex.Message);
            }
        }
        void ClosePTZ()
        {
            try
            {
                if (PTZpr != null)
                {
                    PTZpr.Kill();
                    PTZpr = null;
                }
                setKillPTZ();
                ToolAPI.XMLOperation.WriteLogXmlNoTail("PTZ关闭", "关闭成功");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("PTZ关闭", "关闭异常" + ex.Message);
            }
        }
        bool isRunPTZ()
        {
            try
            {
                Process[] processList = System.Diagnostics.Process.GetProcesses();
                foreach (Process p in processList)
                {
                    if (p.ProcessName == "PTZControl")
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            { return false; }
        }
        bool setKillPTZ()
        {
            try
            {
                Process[] processList = System.Diagnostics.Process.GetProcesses();
                foreach (Process p in processList)
                {
                    if (p.ProcessName == "PTZControl")
                    {
                        p.Kill();
                        return true;
                    }
                }
                return true;
            }
            catch (Exception)
            { return false; }
        }
        void SelfTestPTZ()
        {
            while (true)
            {
                try
                {
                    if (!isRunPTZ())
                    {

                        setKillPTZ();
                        PTZpr = new Process();
                        PTZpr.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + @"PTZ\PTZControl.exe";
                        PTZpr.Start();
                        ToolAPI.XMLOperation.WriteLogXmlNoTail("PTZ被关闭", "自启成功");

                    }
                    if (!isRun())
                    {

                        setKill();
                        //string strtemp = "select * from vsg_FIB where pushflowstatus = '5'";
                        //DataTable dt = AccessOperate.AccessHelper.GetTable(strtemp);
                        //if (dt != null && dt.Rows.Count > 0)//存在就更新
                        //{
                        //    //QClient.CallQueuing.Clear();//清空呼叫队列
                        try //只要本程序一直在运行，那么我就能一直收到MQTT的更新，所以没有必要针对QClient.CallQueuing，而是只需要把推流状态初始掉就行了
                        {
                            string strtemp = "update vsg_FIB set pushflowstatus='-1'";
                            int result = AccessOperate.AccessHelper.InsertData(strtemp);
                        }
                        catch (Exception ex) { }
                        //}
                        //else
                        //{

                        //}
                        pr = new Process();
                        pr.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + @"CLR_C\Exercise2.exe";
                        pr.Start();
                        ToolAPI.XMLOperation.WriteLogXmlNoTail("CLR被关闭", "自启成功");

                    }
                  
                }
                catch (Exception ex)
                {
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("自检测异常", ex.Message);
                }
                Thread.Sleep(3000);
            }
        }
        #endregion

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

      

        private void pictureBox6_Click_1(object sender, EventArgs e)
        {
            ActivationCode acc = new ActivationCode(true);
            acc.ShowDialog();
        }
    }
}
