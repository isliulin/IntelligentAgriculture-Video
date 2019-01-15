using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using OnvifCLRDll;

namespace yeetong_VideoGateway
{
    public class Synchro
    {
        static Synchro()
        {
            Thread SynchroTimeT = new Thread(SynchroTime) { IsBackground = true };
            SynchroTimeT.Start();
        }
        public  static void Init() { } 
        static void SynchroTime()
        {
            while(true)
            {
                try
                {
                    if(PingToServer.IsNet)
                    {
                        //客户端的时间更新
                        if (!string.IsNullOrEmpty(ValidateC.ActivationCode))
                        {
                           string sql = "update  smart_culture_client_activation set last_time = NOW() WHERE activation_id='" + ValidateC.ActivationCode + "'";
                           int result =  MySQL_WCF.ExecuteNoQuery(sql, null, CommandType.Text);
                        }
                        //摄像头的时间的更新 
                        string ipstring = DiscoveryEquipment();
                        if (!string.IsNullOrEmpty(ipstring))
                        {
                            string sql1 = "select deviceid, rtspurl ,iscall,obligate_1 ,pushflowstatus, remarks  from vsg_FIB";
                            DataTable dt = AccessOperate.AccessHelper.GetTable(sql1);
                            for(int i=0;i<dt.Rows.Count ;i++)
                            {
                                string deviceid = dt.Rows[i]["deviceid"].ToString();
                                string ip = dt.Rows[i]["remarks"].ToString();
                                string updatesql = "";
                                if(!string.IsNullOrEmpty(ip)&&ipstring.Contains(ip))
                                {
                                    updatesql = "update  smart_culture_video set last_time = NOW(),onvif_status='yes' WHERE camera_id='" + deviceid + "'";
                                }
                                else
                                {
                                    updatesql = "update  smart_culture_video set last_time = NOW(),onvif_status='no' WHERE camera_id='" + deviceid + "'";
                                }
                                int result = MySQL_WCF.ExecuteNoQuery(updatesql, null, CommandType.Text);
                            }  
                        }

                    }
                }
                catch(Exception ex)
                {
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("SynchroTime同步出现异常", ex.Message+ex.StackTrace);
                }
                Thread.Sleep(30000);//60秒来一次
            }
        }
        #region onvif 更新摄像头
        static public string DiscoveryEquipment()
        {
            try
            {
                int  CameraCount = OnvifCLRDllClassEx.GetDeviceInfoCount();

                if (CameraCount > 0)
                {
                    return AnalyticXML();
                }
                return "";
            }
            catch(Exception ex)
            {
                return "";
                ToolAPI.XMLOperation.WriteLogXmlNoTail("SynchroTime同步出现异常", ex.Message + ex.StackTrace);
            }
        }

        static  string AnalyticXML()
        {
            try
            {
                string IPString = "";
                int i = 0, j = 0;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("OnvifXML.xml");//判断是否加载成功

                XmlNodeList nodeList = xmlDoc.SelectSingleNode("data").ChildNodes;//获取Employees节点的所有子节点 

                foreach (XmlNode xn in nodeList)//遍历所有子节点 
                {
                    XmlElement xe = (XmlElement)xn;//将子节点类型转换为XmlElement类型 
                    if (xe.Name == "NetBook")
                    {
                        XmlNodeList nls = xe.ChildNodes;//继续获取xe子节点的所有子节点 
                        foreach (XmlNode xn1 in nls)//遍历 
                        {
                            XmlElement xe2 = (XmlElement)xn1;//转换类型 

                            switch (xe2.Name)
                            {
                                case "UUID":
                                    break;

                                case "serverAdr":
                                    IPString += xe2.InnerText+"&";
                                    break;

                                case "RTSPAdr":
                                    string RTSPAdr = xe2.InnerText;
                                    break;

                                default:
                                    break;
                            }

                        }
                    }
                    j = 0;
                    if (i < 50) i++;
                }
                return IPString;
            }
            catch(Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("AnalyticXML同步出现异常", ex.Message + ex.StackTrace);
                return "";
            }
        }
        #endregion
    }
}
