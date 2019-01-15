using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using SIXH.DBUtility;
using ToolAPI;

namespace WcfServiceload
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“Service1”。
    public class Service1 : IService1
    {
        public Service1()
        {
        }
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }
        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        #region 数据库操作
        /// <summary>
        /// 非查询
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="providerType">数据库类型</param>
        /// <param name="sql">SQL语句或存储过程名称</param>
        /// <param name="paraList">参数列表</param>
        /// <param name="commandType">SQL语句或存储过程</param>
        /// <returns>SQL语句或存储过程</returns>
        //public int ExecuteNonQuery_ConStrAndType(string connectionString, string sql, IList<DbParameter> paraList, CommandType commandType)
        //{
        //    try
        //    {
        //        DBoperateClass db = new DBoperateClass(connectionString);
        //        int result = db.DBoperateObj.ExecuteNonQuery(sql, paraList, commandType);
        //        ToolAPI.XMLOperation.WriteLogXmlNoTail("非查询结果", string.Format("连接字符串：{0}；sql{1}；结果{2}", connectionString, sql, result.ToString()));
        //        return result;
        //    }
        //    catch(Exception ex)
        //    {
        //        ToolAPI.XMLOperation.WriteLogXmlNoTail("非查询异常", string.Format("连接字符串：{0}；sql{1}；异常{2}", connectionString, sql, ex.Message));
        //        return 0;
        //    }
        //}
        /// <summary>
        /// 查询
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="providerType">数据库类型</param>
        /// <param name="sql">SQL语句或存储过程名称</param>
        /// <param name="paraList">参数列表</param>
        /// <param name="commandType">SQL语句或存储过程</param>
        /// <returns>数据表或null</returns>
        //public DataTable ExecuteDataTable_ConStrAndType(string connectionString, string sql, IList<DbParameter> paraList, CommandType commandType)
        //{
        //    try
        //    {
        //        DBoperateClass db = new DBoperateClass(connectionString);
        //        DataTable result = db.DBoperateObj.ExecuteDataTable(sql, paraList, commandType);
        //        result.TableName = "table1";//如果没有进行命名的，没有办法在WCF中传输
        //        ToolAPI.XMLOperation.WriteLogXmlNoTail("查询结果", string.Format("连接字符串：{0}；sql{1}；结果{2}", connectionString, sql, result.Rows.Count.ToString()));
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        ToolAPI.XMLOperation.WriteLogXmlNoTail("查询异常", string.Format("连接字符串：{0}；sql{1}；异常{2}堆栈{3}", connectionString, sql, ex.Message,ex.StackTrace));
        //        return null;
        //    }
        //}



        public int ExecuteNonQuery_ConStrAndType(string connectionString, string sql, Dictionary<string, object> paraList, CommandType commandType)
        {
            try
            {
                DBoperateClass db = new DBoperateClass(connectionString);
                IList<DbParameter> para = null;
                if (paraList != null)
                {
                    para = new List<DbParameter>();
                    foreach (var paraListT in paraList)
                    {
                        para.Add(db.DBoperateObj.CreateDbParameter(paraListT.Key, paraListT.Value));
                    }
                }
                int result = db.DBoperateObj.ExecuteNonQuery(sql, para, commandType);
                ToolAPI.XMLOperation.WriteLogXmlNoTail("非查询结果", string.Format("连接字符串：{0}；sql{1}；结果{2}", connectionString, sql, result.ToString()));
                return result;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("非查询异常", string.Format("连接字符串：{0}；sql{1}；异常{2}", connectionString, sql, ex.Message));
                return 0;
            }
        }


        public DataTable ExecuteDataTable_ConStrAndType(string connectionString, string sql, Dictionary<string, object> paraList, CommandType commandType)
        {
            try
            {
                DBoperateClass db = new DBoperateClass(connectionString);
                IList<DbParameter> para = null;
                if (paraList != null)
                {
                    para = new List<DbParameter>();
                    foreach (var paraListT in paraList)
                    {
                        para.Add(db.DBoperateObj.CreateDbParameter(paraListT.Key, paraListT.Value));
                    }
                }
                DataTable result = db.DBoperateObj.ExecuteDataTable(sql, para, commandType);
                result.TableName = "table1";//如果没有进行命名的，没有办法在WCF中传输
                ToolAPI.XMLOperation.WriteLogXmlNoTail("查询结果", string.Format("连接字符串：{0}；sql{1}；结果{2}", connectionString, sql, result.Rows.Count.ToString()));
                return result;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("查询异常", string.Format("连接字符串：{0}；sql{1}；异常{2}堆栈{3}", connectionString, sql, ex.Message,ex.StackTrace));
                return null;
            }
        }

        #endregion
    }
}
