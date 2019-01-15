using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Data.Common;

namespace yeetong_VideoGateway
{
    public class MySQL_WCF
    {
        //在来一个集群数据库的查询
        public static string ConnString { get; set; }
        //static DbHelperSQL dbNetdefault;
        static MySQL_WCF()
        {
            string connectionstr = string.Format("Data Source={0};Database={1};User={2};Password={3}", "172.24.108.167", "yeetong_zhyz", "wisdom_root", "JIwLi5j40SY#o1Et");
            //string connectionstr = string.Format("Data Source={0};Database={1};User={2};Password={3}", "39.104.20.2", "yeetong_zhyz", "wisdom_root", "JIwLi5j40SY#o1Et");
            ConnString = connectionstr;

            //dbNetdefault = new DbHelperSQL(string.Format("Data Source={0};Port={1};Database={2};User={3};Password={4}", "192.168.66.190", "3306", "wisdomsite", "root", "goyo87987771"), DbProviderType.MySql);
        }
        #region 查询方式
        /// <summary>
        /// 查询表
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string str, IList<DbParameter> parameters, CommandType commandType)
        {
            try
            {
                using (yeetong.DBClient.ServiceReference1.Service1Client sc = new yeetong.DBClient.ServiceReference1.Service1Client())
                {
                    sc.Open();
                    DataTable result = sc.ExecuteDataTable_ConStrAndType(ConnString, str, null, CommandType.Text);
                    sc.Close();
                    return result;
                }

                //DataTable result = dbNetdefault.ExecuteDataTable(str, parameters, commandType);
                //return result;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("ExecuteNoQuery异常", ex.Message + ex.StackTrace);
                return null;
            }
        }

        public static int ExecuteNoQuery(string str, IList<DbParameter> parameters, CommandType commandType)
        {
            try
            {
                using (yeetong.DBClient.ServiceReference1.Service1Client sc = new yeetong.DBClient.ServiceReference1.Service1Client())
                {
                    sc.Open();
                    int result = sc.ExecuteNonQuery_ConStrAndType(ConnString, str, null, CommandType.Text);
                    sc.Close();
                    return result;
                }

                //int result = dbNetdefault.ExecuteNonQuery(str, parameters, commandType);
                //return result;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("ExecuteNoQuery异常", ex.Message + ex.StackTrace);
                return 0;
            }
        }
        #endregion

    }
}