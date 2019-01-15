using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SIXH.DBUtility
{
    public  class DBoperateClass
    {
        public DBoperateClass(string connectionString)
        {
            try
            {
                DBoperateObj = new DbHelperSQL(connectionString, DbProviderType.MySql);
                ToolAPI.XMLOperation.WriteLogXmlNoTail("DBoperateClass构造", string.Format("连接字符串：{0}", connectionString));
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("DBoperateClass构造异常", string.Format("连接字符串：{0}；异常{2}", connectionString,ex.Message)); 
            }
        }
        public DBoperateClass(DbHelperSQL DbHelperSQLTemp)
        {
            try
            {
                DBoperateObj = DbHelperSQLTemp;
                ToolAPI.XMLOperation.WriteLogXmlNoTail("DBoperateClass构造DbHelperSQLTemp", string.Format("连接字符串：{0}}", DbHelperSQLTemp.ConnectionString));
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("DBoperateClass构造异常", string.Format("连接字符串：{0}；异常{2}", DbHelperSQLTemp.ConnectionString, ex.Message));
            }
        }
        /// <summary>
        /// 数据库操作对象
        /// </summary>
        public  DbHelperSQL DBoperateObj
        {
            get;
            set;
        }
    }
}
