using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace VSGTool
{
    public class PingToServer
    {
        static public bool IsNet = true; 
        static PingToServer()
        {
            //网络遍历
            Thread PingServerT = new Thread(PingServer) { IsBackground = true, Priority = ThreadPriority.Highest };
            PingServerT.Start();
        }

        static void PingServer()
        {
            while(true)
            {
                try{
                    IsNet = MyPing();
                }
                catch(Exception){}
                Thread.Sleep(1000);
            }
        }
        #region 外网连接
        /// <summary>
        /// Ping命令检测网络是否畅通
        /// </summary>
        /// <param name="urls">URL数据</param>
        /// <param name="errorCount">ping时连接失败个数</param>
        /// <returns></returns>
        public static bool MyPing()
        {
            string url = "114.55.126.208";
            bool isconn = true;
            Ping ping = new Ping();
            int errorCount = 0;
            try
            {
                PingReply pr;
                for (int i = 0; i < 5; i++)
                {
                    pr = ping.Send(url);
                    if (pr.Status == IPStatus.Success)
                        errorCount++;
                }
            }
            catch
            {
                errorCount = 0;
            }
            if (errorCount > 0)
                isconn = true;
            else
                isconn = false;
            return isconn;
        }
        #endregion
    }
}
