using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using PTZControl;
using ToolAPI;

namespace PTZControl
{
    /// <summary>
    /// 里面已经加上业务
    /// </summary>
    public class Tcp_Client
    {
        static Socket socketClient;
        static byte[] RecvBuffer = new byte[1024];
        static event Action<byte[], int> OnSocketResolveRecvEvent;
        static string ProjectId = "";
        public static string KEYT = "*Gy9";

        static DateTime LastReceiveTime = DateTime.Now;

        static Tcp_Client()
        {
            OnSocketResolveRecvEvent += new Action<byte[], int>(ReceiveAnalysis);

            string Path = AppDomain.CurrentDomain.BaseDirectory.Replace(@"\PTZ\", @"\") + @"Config.ini";
            string projectidTemp = INIOperate.IniReadValue("Identity", "projectid", Path);
            string ProjectIdtemp = CryptoTool.Decrypt_DES(projectidTemp, KEYT);
            ProjectId = ProjectIdtemp;

            ////测试用
            //ProjectId = "649bfb0694f64d408c1837a5e8b3cc29";



            socketClient = null;
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketClient.Connect("39.104.228.149", 7009);
            StartRecvMessage();
            Topic_take();//建立连接后先进行订阅
            ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\tcp", "完成订阅", "");
            Thread.Sleep(500);
            //心跳的线程
            Thread HeartbeatProcessT = new Thread(HeartbeatProcess) { IsBackground = true, Priority = ThreadPriority.Highest };
            HeartbeatProcessT.Start();

            //设备状态的遍历线程
            Thread StateProcessT = new Thread(StateProcess) { IsBackground = true, Priority = ThreadPriority.Highest };
            StateProcessT.Start();
            ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\tcp", "构造完成", ProjectId);
        }
        public static void Init() { }

        #region 数据接收
        /// <summary>
        ///接收数据
        /// </summary>
        static public void StartRecvMessage()
        {
            try
            {
                byte[] RecvBufferTemp = new byte[1024];
                //异步 BeginReceive 操作必须通过调用 EndReceive 方法来完成。通常，该方法由 callback 委托调用。
                //个用户定义对象，其中包含接收操作的相关信息。当操作完成时，此对象会被传递给 EndReceive 委托。
                socketClient.BeginReceive(RecvBufferTemp, 0, RecvBufferTemp.Length, 0, new AsyncCallback(RecvCallBack), RecvBufferTemp);
            }
            catch (Exception e)
            {
                DisSocket();
            }
        }
        /// <summary>
        /// 接收数据回掉函数
        /// </summary>
        /// <param name="ar"></param>
        static void RecvCallBack(IAsyncResult ar)
        {
            byte[] RecvBufferTemp = ar.AsyncState as byte[];
            int bytesRead = 0;
            try
            {
                //接收到的数据的数量
                bytesRead = socketClient.EndReceive(ar);
            }
            catch (Exception e)
            {
                return;
            }
            if (bytesRead > 0)
            {
                try
                {
                    //接收事件
                    OnSocketResolveRecvEvent(RecvBufferTemp, bytesRead);//如果它接受的数据大于0了，就直接返回到这个函数里了
                }
                catch
                { }
                finally
                {
                    StartRecvMessage();
                }
            }
            else//正常对方关闭后会给发送一个没字节的信号
            {
                SocketConnect();
            }
        }
        #endregion

        #region 发送数据
        /// <summary>
        /// 发送字符串
        /// </summary>
        /// <param name="message">字符串</param>
        static public void SendMessage(string message)
        {
            byte[] sendbuffer = Encoding.Default.GetBytes(message);
            //发送消息
            SendBuffer(sendbuffer);
            ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\tcpSend", "原包", message);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sendBuff">字节流</param>
        static public void SendBuffer(byte[] sendBuff)
        {

            int l = 5120;
            if (sendBuff.Length <= l)
            {
                try
                {
                    socketClient.BeginSend(sendBuff, 0, sendBuff.Length, 0, new AsyncCallback(SendMessageCallBack), socketClient);
                }
                catch(Exception ex)
                {
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("SendBuffer 异常1", ex.Message);
                    try
                    {
                            DisSocket();
                        socketClient = null;
                        socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socketClient.Connect("39.104.228.149", 7009);
                        ToolAPI.XMLOperation.WriteLogXmlNoTail("重新建立tcp链接", "");
                    }
                    catch(Exception ex2)
                    {
                        ToolAPI.XMLOperation.WriteLogXmlNoTail("PTZ 重新建立tcp链接异常", ex2.Message);
                    }
                }
            }
            else
            {
                int c = sendBuff.Length / l, mod = sendBuff.Length % l;
                byte[] tempByte = new byte[l];
                for (int i = 0; i <= c; i++)
                {
                    if (i == c && mod > 0)//有剩佘字节
                    {
                        Array.Copy(sendBuff, i * l, tempByte, 0, mod);
                        try
                        {
                            socketClient.BeginSend(tempByte, 0, mod, 0, new AsyncCallback(SendMessageCallBack), socketClient);
                        }
                        catch(Exception ex)
                        {
                            ToolAPI.XMLOperation.WriteLogXmlNoTail("SendBuffer 异常2", ex.Message);
                            try
                            {
                                 DisSocket();
                                socketClient = null;
                                socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                socketClient.Connect("39.104.228.149", 7009);
                                ToolAPI.XMLOperation.WriteLogXmlNoTail("重新建立tcp链接2", "");
                            }
                            catch (Exception ex2)
                            {
                                ToolAPI.XMLOperation.WriteLogXmlNoTail("PTZ 重新建立tcp链接异常2", ex2.Message);
                            }
                        }
                    }
                    else if (i < c)
                    {
                        Array.Copy(sendBuff, i * l, tempByte, 0, l);
                        try
                        {
                            socketClient.BeginSend(tempByte, 0, l, 0, new AsyncCallback(SendMessageCallBack), socketClient);
                        }
                        catch(Exception ex)
                        {
                            ToolAPI.XMLOperation.WriteLogXmlNoTail("SendBuffer 异常3", ex.Message);
                            try
                            {
                                    DisSocket();
                                socketClient = null;
                                socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                socketClient.Connect("39.104.228.149", 7009);
                                ToolAPI.XMLOperation.WriteLogXmlNoTail("重新建立tcp链接3", "");
                            }
                            catch (Exception ex2)
                            {
                                ToolAPI.XMLOperation.WriteLogXmlNoTail("PTZ 重新建立tcp链接异常3", ex2.Message);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="ar"></param>
        static void SendMessageCallBack(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int bytesSend = client.EndSend(ar);
            }
            catch(Exception ex )
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("SendMessageCallBack 异常", ex.Message);
            }
        }
        #endregion

        #region socket开启和关闭
        /// <summary>
        /// 关闭远程服务器连接并删除socke对象
        /// </summary>
       public static void DisSocket()
        {
            try
            {
                if (socketClient != null)
                {
                    socketClient.Shutdown(SocketShutdown.Both);
                }
            }
            catch(Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("PTZ DisSocket异常", ex.Message);
            }
            finally
            {
                try
                {
                    socketClient.Close();//释放连接对象并关闭连接
                }
                catch (Exception ex)
                {
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("PTZ DisSocket异常", ex.Message);
                }
            }
        }
        /// <summary>
        /// 连接初始化
        /// </summary>
        static public void SocketConnect()
        {
            try
            {
                DisSocket();
                socketClient = null;
                socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socketClient.Connect("39.104.228.149", 7009);
                StartRecvMessage();
                Topic_take();//建立连接后先进行订阅
                ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\tcp", "连接结束", "");
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\tcp", "连接异常", ex.Message);
            }
        }
        #endregion

        #region 数据拼接（订阅，发送数据,心跳）
        static void Topic_take()
        {
            try
            {
                string topic = "PTZ/" + ProjectId + "/Call";
                string message = string.Format("#topic#add#{0}#{1}#", topic, ProjectId);
                SendMessage(message);
                ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\tcp", "数据订阅", topic);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\tcp", "数据订阅异常", ex.Message);
                Thread.Sleep(1000);
                SocketConnect();
            }
        }
        static void Topic_remove(string proid)
        {
            try
            {
                string topic = "PTZ/" + proid + "/Call";
                string message = string.Format("#topic#remove#{0}#{1}#", topic, proid);
                SendMessage(message);
                ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\tcp", "数据订阅移除", topic);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\tcp", "数据订阅移除异常", ex.Message);
                Thread.Sleep(1000);
                SocketConnect();
            }
        }
        static void Send_Data(string dev, string state)
        {
            try
            {
                string topic = string.Format("PTZ/{0}/{1}/PTZStatus", ProjectId, dev);
                string content = string.Format("S&{0}&{1}&{2}&PTZ&E", dev, state, DateTime.Now.ToString("yyyyMMddHHmmss"));
                string sendmessage = string.Format("#send_data#{0}#{1}#{2}#", topic, content, ProjectId);
                SendMessage(sendmessage);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\tcp", "数据发送异常", ex.Message);
                Thread.Sleep(1000);
                SocketConnect();
            }
        }
        static void Heartbeat_take()
        {
            try
            {
                string message = string.Format("#heartbeat#{0}#{1}#", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ProjectId);
                SendMessage(message);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\tcp", "数据心跳异常", ex.Message);
                Thread.Sleep(1000);
                SocketConnect();
            }
        }
        #endregion

        #region 接收解析
        static void ReceiveAnalysis(byte[] bufftemp, int count)
        {
            try
            {
                string data_str = Encoding.UTF8.GetString(bufftemp, 0, count);//这里使用utf-8来表示的
                ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\tcpReceive", "接收", data_str);
                string[] data_strAry = data_str.Split('#');

                if (data_strAry.Length >= 4)
                {
                    switch (data_strAry[1])
                    {
                        case "heartbeat": LastReceiveTime = DateTime.Now; break;
                        case "topic": LastReceiveTime = DateTime.Now; break;
                        //case "send_data":  break;
                        case "receive_data": receive_data_analyze(data_strAry[2]); break;
                        default: break;
                    }
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\tcp", "接收数据解析异常", ex.Message);
            }
        }
        //服务器数据包
        static void receive_data_analyze(string value_str)
        {
            try
            {
                string[] temp = value_str.Split('&');
                if (temp.Length == 12 && temp[0] == "S" && temp[11] == "E")
                {
                    string devid = temp[1];
                    try
                    {
                        CameraPZT camera = new CameraPZT();
                        ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\tcp", "开始执行", value_str);
                        Send_Data(devid, "1");
                        string result = camera.PZTOperateFull(temp[6], int.Parse(temp[7]), int.Parse(temp[8]), temp[2], temp[3], temp[4], temp[5], out camera);
                        ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\tcp", "执行结果", devid + ";" + result);
                        if (result == "执行成功")
                            Send_Data(devid, "2");
                        else
                            Send_Data(devid, "-1");
                    }
                    catch (Exception ex)
                    {
                        ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\tcp", "操作摄像机异常", value_str + ";" + ex.Message);
                        Send_Data(devid, "-1");
                    }
                }
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\tcp", "服务器数据解析异常", ex.Message);
            }
        }
        #endregion

        #region 心跳
        static void HeartbeatProcess()
        {
            while (true)
            {
                Thread.Sleep(75000);
                try
                {
                    Heartbeat_take();
                    Topic_take();
                    //判断有没有接受到这个的应答
                    Thread.Sleep(10000);
                    if ((DateTime.Now-LastReceiveTime).TotalSeconds>85)
                    {
                        //干掉那个socket
                        ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\tcp", "接收未被置位", "准备重新连接");
                        SocketConnect();
                    }

                }
                catch (Exception ex)
                { }
              
            }
        }
        #endregion

        #region 工地id遍历
        static void StateProcess()
        {
            while (true)
            {
                while (true)
                {
                    try
                    {
                        ProjectIDInit();
                    }
                    catch (Exception) { }
                    Thread.Sleep(1000);
                }
            }
        }

        #region 工地变化
        static void ProjectIDInit()
        {
            string Path = AppDomain.CurrentDomain.BaseDirectory.Replace(@"\PTZ\", @"\") + @"Config.ini";
            string projectidTemp = INIOperate.IniReadValue("Identity", "projectid", Path);
            string ProjectIdtemp = CryptoTool.Decrypt_DES(projectidTemp, KEYT);
            if (ProjectIdtemp != ProjectId)
            {
                Topic_remove(ProjectId);//先移除老的订阅
                ProjectId = ProjectIdtemp;
                Topic_take();//添加新的订阅
            }
        }
        #endregion
        #endregion
    
       
    }
}
