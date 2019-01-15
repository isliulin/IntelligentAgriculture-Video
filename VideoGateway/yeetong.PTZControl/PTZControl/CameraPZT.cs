using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PTZControl
{
    public enum PZTType
    {
        PAN_LEFT = CHCNetSDK.PAN_LEFT,//左
        TILT_UP = CHCNetSDK.TILT_UP,//上
        PAN_RIGHT = CHCNetSDK.PAN_RIGHT,//右
        TILT_DOWN = CHCNetSDK.TILT_DOWN,//下
        ZOOM_IN = CHCNetSDK.ZOOM_IN,//焦距变大
        ZOOM_OUT = CHCNetSDK.ZOOM_OUT,//焦距变小
        FOCUS_NEAR = CHCNetSDK.FOCUS_NEAR,//焦点前调
        FOCUS_FAR = CHCNetSDK.FOCUS_FAR,//焦点后调
        IRIS_OPEN = CHCNetSDK.IRIS_OPEN,//光圈扩大
        IRIS_CLOSE = CHCNetSDK.IRIS_CLOSE,//光圈缩小
        PAN_AUTO = CHCNetSDK.PAN_AUTO,  //云台左右自动扫描
        UP_LEFT = CHCNetSDK.UP_LEFT,//左上
        UP_RIGHT = CHCNetSDK.UP_RIGHT,//右上
        DOWN_LEFT = CHCNetSDK.DOWN_LEFT,//左下
        DOWN_RIGHT = CHCNetSDK.DOWN_RIGHT//右下
    }
    public class CameraPZT
    {
        private bool m_bInitSDK = false;
        private Int32 m_lUserID = -1;
        private Int32 m_lRealHandle = -1;
        const int m_lChannel = 1;
        public CameraPZT()
        {
            m_bInitSDK = CHCNetSDK.NET_DVR_Init();
        }

        public void delete(out CameraPZT pzt)
        {
            pzt = null;
        }
        #region 登录和注销
        public bool Login(string ip, string port, string user, string password)
        {
            if (m_lUserID < 0)
            {
                CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();
                //登录设备 Login the device
                m_lUserID = CHCNetSDK.NET_DVR_Login_V30(ip, int.Parse(port), user, password, ref DeviceInfo);
                if (m_lUserID < 0)
                {
                    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    return false; ;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }

        public bool logout()
        {
            if (m_lUserID >= 0)
            {
                if (!CHCNetSDK.NET_DVR_Logout(m_lUserID))
                {
                    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    return false;
                }
            }
            m_lUserID = -1;
            return true;
        }
        #endregion

        #region 预览
        public bool preview(IntPtr handle)
        {
            if (m_lUserID >= 0)
            {
                if (m_lRealHandle < 0)
                {
                    CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                    lpPreviewInfo.hPlayWnd = handle;//预览窗口
                    lpPreviewInfo.lChannel = 1;//预te览的设备通道
                    lpPreviewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                    lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                    lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                    lpPreviewInfo.dwDisplayBufNum = 15; //播放库播放缓冲区最大缓冲帧数

                    CHCNetSDK.REALDATACALLBACK RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                    IntPtr pUser = new IntPtr();//用户数据

                    //打开预览 Start live view 
                    m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, null/*RealData*/, pUser);
                    if (m_lRealHandle < 0)
                    {
                        uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return true;
            }
            return false;

        }
        public void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, ref byte pBuffer, UInt32 dwBufSize, IntPtr pUser)
        {
        }
        public bool ClosePreview()
        {
            if (m_lUserID >= 0 && m_lRealHandle > 0)
            {
                if (!CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle))
                {
                    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    return false;
                }
                return true;
            }
            return false;
        }
        #endregion

        #region 云台控制
        //开
        public bool PZTOpen(uint flag, uint speed)
        {
            bool result = false;
            if (m_lRealHandle > 0)   // 启动图像识别 
            {
                result =  CHCNetSDK.NET_DVR_PTZControlWithSpeed(m_lRealHandle, flag, 0, speed + 1);
            }
            else                      //未启动图像识别
            {
                result = CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(m_lUserID, m_lChannel, flag, 0, speed + 1);
            }
            ToolAPI.XMLOperation.WriteLogXmlNoTail("开", flag.ToString()+";"+ result.ToString());
            return true;
        }
        //关
        public bool PZTClose(uint flag, uint speed)
        {
            bool result = false;
            if (m_lRealHandle > 0)
            {
                result =  CHCNetSDK.NET_DVR_PTZControlWithSpeed(m_lRealHandle, flag, 1, speed + 1);
            }
            else
            {
                result =  CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(m_lUserID, m_lChannel, flag, 1, speed + 1);
            }
            ToolAPI.XMLOperation.WriteLogXmlNoTail("关", flag.ToString() + ";" + result.ToString());
            return true;
        }
        #endregion

        #region 一次云台控制 注册到控制注销
        /// <summary>
        /// 转动过程
        /// </summary>
        /// <param name="pztType">云台控制类型</param>
        /// <param name="time">运行时间 单位毫秒</param>
        /// <param name="speed">云台控制速度</param>
        public string PZTOperate(string pztType, int time, int speed)
        {
            try
            {
                PZTType pztTypetemp;
                if (Enum.TryParse<PZTType>(pztType, out pztTypetemp))
                {
                    new Action<PZTType, int>((PZTType pzttemp, int timetype) =>
                    {
                        PZTOpen((uint)pzttemp, (uint)speed);
                        Thread.Sleep(time);
                        PZTClose((uint)pzttemp, (uint)speed);
                    }).BeginInvoke(pztTypetemp, time, null, null);
                    return "正在执行";
                }
                return "参数有误";
            }
            catch (Exception) { return "出现异常"; }
        }

        public string PZTOperateFull(string pztType, int time, int speed, string ip, string port, string user, string password, out CameraPZT cameraPZT)
        {
            try
            {
                if (Login(ip, port, user, password))
                {
                    PZTType pztTypetemp;
                    if (Enum.TryParse<PZTType>(pztType, out pztTypetemp))
                    {
                        PZTOpen((uint)pztTypetemp, (uint)speed);
                        Thread.Sleep(time);
                        PZTClose((uint)pztTypetemp, (uint)speed);
                        logout();
                        cameraPZT = null;
                        ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\ptz", "顺利执行", pztType+";"+time + ";" + speed + ";" + ip + ";" + port + ";" + user + ";" + password);
                        return "执行成功";
                    }
                    cameraPZT = null;
                    ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\ptz", "参数有误", pztType + ";" + time + ";" + speed + ";" + ip + ";" + port + ";" + user + ";" + password);
                    return "参数有误";
                }
                cameraPZT = null;
                ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\ptz", "登录失败", pztType + ";" + time + ";" + speed + ";" + ip + ";" + port + ";" + user + ";" + password);
                return "登录失败";
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail(Application.StartupPath + "\\ptz", "执行异常", pztType + ";" + time + ";" + speed + ";" + ip + ";" + port + ";" + user + ";" + password+";"+ex.Message);
                cameraPZT = null;
                return "执行异常";
            }
        }

        #endregion

    }
}
