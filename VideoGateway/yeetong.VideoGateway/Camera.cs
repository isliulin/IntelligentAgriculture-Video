using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace yeetong_VideoGateway
{
     [Serializable]
    public class CameraMain
    {
        public string code { set; get; }
        public string msg { set; get; }
        public CameraSon[] data { set; get; }

        public static CameraSon[] GetCameraList(string farm_id)
        {
            try
            {
                List<CameraSon> data = new List<CameraSon>();
                string sql = "select * from smart_culture_video where state=0 and farm_id='" + farm_id + "'";
                DataTable dt = MySQL_WCF.ExecuteDataTable(sql, null, CommandType.Text);
                if(dt!=null&&dt.Rows.Count>0)
                {
                    foreach(DataRow rw in dt.Rows){
                        CameraSon cs = new CameraSon();
                        cs.id = rw["camera_id"].ToString();
                        cs.name = rw["camera_name"].ToString();
                        cs.pro_id = rw["farm_id"].ToString();
                        cs.position_x = "";
                        cs.position_y = "";
                        cs.position_desc = rw["rtsp_url"].ToString();
                        cs.stream_url = rw["push_rtmp_url"].ToString();
                        cs.play_url = rw["play_rtmp_url"].ToString();
                        cs.deploy_time = rw["deploy_time"].ToString();
                        cs.brand_name = rw["brand_name"].ToString();
                        cs.angle = "0";
                        cs.stream_value = "256";
                        cs.vtype = rw["type"].ToString();
                        cs.voperation = rw["voperation"].ToString();
                        cs.stream = "256";
                        cs.is_tower_eye = "";
                        cs.craneId = "";
                        cs.proName = "视频流转换";
                        cs.isStreaming = "-1";
                        cs.PublishTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        cs.pushflowmode = rw["push_flow_mode"].ToString();
                        cs.IP = rw["ip"].ToString();
                        data.Add(cs);
                    }
                   return  data.ToArray();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
     [Serializable]
    public class CameraSon
    {
         public string id { set; get; }
         public string name { set; get; }
         public string pro_id { set; get; }
         public string position_x { set; get; }
         public string position_y { set; get; }
         public string position_desc { set; get; }
         public string stream_url { set; get; }
         public string play_url { set; get; }
         public string deploy_time { set; get; }
         public string brand_name { set; get; }
         public string angle { set; get; }
         public string stream_value { set; get; }
         public string vtype { set; get; }
         public string voperation { set; get; }
         public string stream { set; get; }
         public string is_tower_eye { set; get; }
         public string craneId { set; get; }
         public string proName { set; get; }
         public string isStreaming { set; get; }
         public string PublishTime { set; get; }
         public string pushflowmode { set; get; }
         public string IP { set; get; }
    }
}
