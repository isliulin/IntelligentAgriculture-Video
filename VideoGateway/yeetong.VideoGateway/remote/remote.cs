using RDPCOMAPILib;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToolAPI;

namespace yeetong_VideoGateway
{
    public class remote
    {
        #region 远程连接
        static RDPSession m_pRdpSession = null;
        static string  invitationString = "";
        static string ProjectID = "";
        public static bool IsStart = false;
        static void RDPContentInit()
        {
            m_pRdpSession = new RDPSession();// 新建RDP Session

            m_pRdpSession.OnAttendeeConnected += new _IRDPSessionEvents_OnAttendeeConnectedEventHandler(OnAttendeeConnected);//当与会者连接到会话时调用
            m_pRdpSession.OnAttendeeDisconnected += new _IRDPSessionEvents_OnAttendeeDisconnectedEventHandler(OnAttendeeDisconnected);//当与会者断开与会话的连接时调用
            m_pRdpSession.OnControlLevelChangeRequest += new _IRDPSessionEvents_OnControlLevelChangeRequestEventHandler(OnControlLevelChangeRequest);//当查看器请求控件时调用

            // _rdpSession.SetDesktopSharedRect(rect.X, rect.Y, rect.Right, rect.Bottom); // 设置共享区域，如果不设置默认为整个屏幕，当然如果有多个屏幕，还是设置下主屏幕，否则，区域会很大

            m_pRdpSession.Open();// 打开会话
            IRDPSRAPIInvitation pInvitation = m_pRdpSession.Invitations.CreateInvitation("WinPresenter", "PresentationGroup", "", 5);// 创建申请
            invitationString = pInvitation.ConnectionString;
            //把连接字符串做一下记录
            WriteToFile(invitationString);
            //连接到数据库
        }

        static void RDPClose()
        {
            if (m_pRdpSession != null)
            {
                m_pRdpSession.Close();// 关闭会话
                Marshal.ReleaseComObject(m_pRdpSession);
                m_pRdpSession = null;
            }
        }

        public static void WriteToFile(string InviteString)
        {
            using (StreamWriter sw = File.CreateText("D:\\inv.xml"))
            {
                sw.WriteLine(InviteString);
            }
        }

        //连接时触发的事件
        private static void OnAttendeeConnected(object pObjAttendee)
        {
            IRDPSRAPIAttendee pAttendee = pObjAttendee as IRDPSRAPIAttendee;
            // pAttendee.ControlLevel = CTRL_LEVEL.CTRL_LEVEL_VIEW;
            pAttendee.ControlLevel = CTRL_LEVEL.CTRL_LEVEL_VIEW;
            //LogTextBox.Text += ("Attendee Connected: " + pAttendee.RemoteName + Environment.NewLine);
        }
        //断开连接是触发的事件
        static void OnAttendeeDisconnected(object pDisconnectInfo)
        {
            IRDPSRAPIAttendeeDisconnectInfo pDiscInfo = pDisconnectInfo as IRDPSRAPIAttendeeDisconnectInfo;
            //LogTextBox.Text += ("Attendee Disconnected: " + pDiscInfo.Attendee.RemoteName + Environment.NewLine);
        }
        //当查看器请求控件时调用
        static void OnControlLevelChangeRequest(object pObjAttendee, CTRL_LEVEL RequestedLevel)
        {
            IRDPSRAPIAttendee pAttendee = pObjAttendee as IRDPSRAPIAttendee;
            pAttendee.ControlLevel = RequestedLevel;
        }
        #endregion

        #region 监测设备编号变更和网络异常后的状态
       public static void Check()
        {
            while (true)
            {
                try
                {
                    //是否存在网络
                    if (PingToServer.IsNet)//存在网络
                    {
                        string projectidTemp = INIOperate.IniReadValue("Identity", "projectid", AppDomain.CurrentDomain.BaseDirectory + "\\Config.ini");
                        ProjectID = CryptoTool.Decrypt_DES(projectidTemp, ValidateC.KEYT);
                        //判断m_pRdpSession远程桌面是否开启，没有开启先进行开启
                        if (m_pRdpSession == null)
                        {
                            RDPContentInit();
                        }
                        //更新数据库对应的远程桌面连接和时间
                        if (ProjectID != "")
                        //更新数据库对应的远程桌面连接和时间
                        {
                            string sql = string.Format("UPDATE smart_culture_client_activation SET remote_connection_str = '{0}',remote_time='{1}' WHERE farm_id = '{2}' AND client_type='1'", invitationString, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ProjectID);
                            int result = MySQL_WCF.ExecuteNoQuery(sql, null, CommandType.Text);
                            Updatestatus(1);
                        }
                        else
                        {
                            Updatestatus(2);
                        }
                        //桌面听信正常工作
                        IsStart = true;
                    }
                    else//不存在网络
                    {
                        RDPClose();//远程共享
                        Updatestatus(0);//桌面提醒没有工作
                        IsStart = false;
                    }
                }
                catch (Exception ex)
                {
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("Check异常", ex.Message);
                    Updatestatus(2);//桌面提醒没有工作
                }
                Thread.Sleep(30000);
            }
        }
        static void Updatestatus(int status)
        {
            //if (this.InvokeRequired)
            //{
            //    Action<int> action = new Action<int>(Updatestatus);
            //    this.Invoke(action, status);
            //}
            //else
            //{
            //    if (status == 1)
            //    {
            //        this.BackgroundImage = global::Winshare1.Properties.Resources.start;
            //    }
            //    else if (status == 2)
            //        this.BackgroundImage = global::Winshare1.Properties.Resources.exception;
            //    else
            //        this.BackgroundImage = global::Winshare1.Properties.Resources.stop;
            //}
        }
        #endregion
    }
}
