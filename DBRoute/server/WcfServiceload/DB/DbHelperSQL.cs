using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;

namespace SIXH.DBUtility
{
    [DataContractAttribute]
    public sealed class DbHelperSQL
    {
        #region 获得数据库操作程序集
        public string ConnectionString { get; set; }
        private DbProviderFactory providerFactory;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="providerType">数据库类型枚举，参见<paramref name="providerType"/></param>
        public DbHelperSQL(string connectionString, DbProviderType providerType)
        {
            ConnectionString = connectionString;
            providerFactory = ProviderFactory.GetDbProviderFactory(providerType);
            if (providerFactory == null)
            {
                throw new ArgumentException("Can't load DbProviderFactory for given value of providerType");
            }
        }
        #endregion

        #region 根据连接字符串判断是否能够打开数据库
        /// <summary>
        /// 是否能够连接数据库
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            DbConnection connection = providerFactory.CreateConnection();
            bool flag = false;
            try
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();
                flag = true;
            }
            catch (Exception )
            {
                //throw e;
                return false;
            }
            finally
            {
                connection.Close();
            }
            return flag;
        }
        #endregion

        #region 增删改操作（非查询）返回受影响行数
        /// <summary>   
        /// 对数据库执行增删改操作，返回受影响的行数。   
        /// </summary>   
        /// <param name="sql">要执行的增删改的SQL语句</param>   
        /// <param name="parameters">执行增删改语句所需要的参数</param>
        /// <returns></returns>  
        public int ExecuteNonQuery(string sql, IList<DbParameter> parameters)
        {
            return ExecuteNonQuery(sql, parameters, CommandType.Text);
        }
        /// <summary>   
        /// 对数据库执行增删改操作，返回受影响的行数。   
        /// </summary>   
        /// <param name="sql">要执行的增删改的SQL语句</param>   
        /// <param name="parameters">执行增删改语句所需要的参数</param>
        /// <param name="commandType">执行的SQL语句的类型  TextSQL 文本命令 StoredProcedure存储过程的名称 TableDirect表的名称</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, IList<DbParameter> parameters, CommandType commandType)
        {

            using (DbCommand command = CreateDbCommand(sql, parameters, commandType))
            {
                int affectedRows = 0;
                try
                {
                    command.Connection.Open();
                    affectedRows = command.ExecuteNonQuery();
                    command.Connection.Close();
                }
                catch { }
                finally
                {

                    command.Connection.Close();
                }
                return affectedRows;
            }
        }
        #endregion

        #region 查询
        #region 执行一个查询语句，返回一个关联的DataReader(数据源读取行的一个只进流)实例
        /// <summary>   
        /// 执行一个查询语句，返回一个关联的DataReader实例   
        /// </summary>   
        /// <param name="sql">要执行的查询语句</param>   
        /// <param name="parameters">执行SQL查询语句所需要的参数</param>
        /// <returns></returns> 
        public DbDataReader ExecuteReader(string sql, IList<DbParameter> parameters)
        {
            return ExecuteReader(sql, parameters, CommandType.Text);
        }

        /// <summary>   
        /// 执行一个查询语句，返回一个关联的DataReader实例   
        /// </summary>   
        /// <param name="sql">要执行的查询语句</param>   
        /// <param name="parameters">执行SQL查询语句所需要的参数</param>
        /// <param name="commandType">执行的SQL语句的类型</param>
        /// <returns></returns> 
        public DbDataReader ExecuteReader(string sql, IList<DbParameter> parameters, CommandType commandType)
        {
            DbCommand command = CreateDbCommand(sql, parameters, commandType);
            command.Connection.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
        #endregion
        #region 执行一个查询语句，返回一个包含查询结果的DataTable
        /// <summary>   
        /// 执行一个查询语句，返回一个包含查询结果的DataTable   
        /// </summary>   
        /// <param name="sql">要执行的查询语句</param>   
        /// <param name="parameters">执行SQL查询语句所需要的参数</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sql, IList<DbParameter> parameters)
        {
            return ExecuteDataTable(sql, parameters, CommandType.Text);
        }
        /// <summary>   
        /// 执行一个查询语句，返回一个包含查询结果的DataTable   
        /// </summary>   
        /// <param name="sql">要执行的查询语句</param>   
        /// <param name="parameters">执行SQL查询语句所需要的参数</param>
        /// <param name="commandType">执行的SQL语句的类型</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sql, IList<DbParameter> parameters, CommandType commandType)
        {
            using (DbCommand command = CreateDbCommand(sql, parameters, commandType))
            {
                //command.Connection.Open();
                using (DbDataAdapter adapter = providerFactory.CreateDataAdapter())
                {
                    //获取或设置用于在数据源中选择记录的命令
                    adapter.SelectCommand = command;
                    DataTable data = new DataTable();
                    adapter.Fill(data);
                    command.Connection.Close();
                    return data;
                }

            }
        }
        #endregion
        #region 执行一个查询语句，返回查询结果的第一行第一列
        /// <summary>   
        /// 执行一个查询语句，返回查询结果的第一行第一列   
        /// </summary>   
        /// <param name="sql">要执行的查询语句</param>   
        /// <param name="parameters">执行SQL查询语句所需要的参数</param>   
        /// <returns></returns>   
        public Object ExecuteScalar(string sql, IList<DbParameter> parameters)
        {
            return ExecuteScalar(sql, parameters, CommandType.Text);
        }

        /// <summary>   
        /// 执行一个查询语句，返回查询结果的第一行第一列   
        /// </summary>   
        /// <param name="sql">要执行的查询语句</param>   
        /// <param name="parameters">执行SQL查询语句所需要的参数</param>   
        /// <param name="commandType">执行的SQL语句的类型</param>
        /// <returns></returns>   
        public Object ExecuteScalar(string sql, IList<DbParameter> parameters, CommandType commandType)
        {
            using (DbCommand command = CreateDbCommand(sql, parameters, commandType))
            {
                object result = null;
                try
                {
                    command.Connection.Open();
                    //执行查询，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略
                    result = command.ExecuteScalar();
                    command.Connection.Close();
                }
                catch (Exception )
                {
                    //throw e;
                }
                finally
                {
                    command.Connection.Close();
                }
                return result;
            }
        }
        #endregion
        #region 查询单个或多个实体集合
        ///// <summary>
        ///// 查询多个实体集合
        ///// </summary>
        ///// <typeparam name="T">返回的实体集合类型</typeparam>
        ///// <param name="sql">要执行的查询语句</param>   
        ///// <param name="parameters">执行SQL查询语句所需要的参数</param>
        ///// <returns></returns>
        //public List<T> QueryForList<T>(string sql, IList<DbParameter> parameters) where T : new()
        //{
        //    return QueryForList<T>(sql, parameters, CommandType.Text);
        //}

        ///// <summary>
        /////  查询多个实体集合
        ///// </summary>
        ///// <typeparam name="T">返回的实体集合类型</typeparam>
        ///// <param name="sql">要执行的查询语句</param>   
        ///// <param name="parameters">执行SQL查询语句所需要的参数</param>   
        ///// <param name="commandType">执行的SQL语句的类型</param>
        ///// <returns></returns>
        //public List<T> QueryForList<T>(string sql, IList<DbParameter> parameters, CommandType commandType) where T : new()
        //{
        //    DataTable data = ExecuteDataTable(sql, parameters, commandType);
        //    return EntityReader.GetEntities<T>(data);
        //}
        ///// <summary>
        ///// 查询单个实体
        ///// </summary>
        ///// <typeparam name="T">返回的实体集合类型</typeparam>
        ///// <param name="sql">要执行的查询语句</param>   
        ///// <param name="parameters">执行SQL查询语句所需要的参数</param>
        ///// <returns></returns>
        //public T QueryForObject<T>(string sql, IList<DbParameter> parameters) where T : new()
        //{
        //    return QueryForObject<T>(sql, parameters, CommandType.Text);
        //}

        ///// <summary>
        ///// 查询单个实体
        ///// </summary>
        ///// <typeparam name="T">返回的实体集合类型</typeparam>
        ///// <param name="sql">要执行的查询语句</param>   
        ///// <param name="parameters">执行SQL查询语句所需要的参数</param>   
        ///// <param name="commandType">执行的SQL语句的类型</param>
        ///// <returns></returns>
        //public T QueryForObject<T>(string sql, IList<DbParameter> parameters, CommandType commandType) where T : new()
        //{
        //    return QueryForList<T>(sql, parameters, commandType)[0];
        //}
        #endregion
        #endregion

        #region  设置一个输入参数的值
        //设置一个输入参数的值
        public DbParameter CreateDbParameter(string name, object value)
        {
            //ParameterDirection.Input 输入参数
            return CreateDbParameter(name, ParameterDirection.Input, value);
        }

        public DbParameter CreateDbParameter(string name, ParameterDirection parameterDirection, object value)
        {
            DbParameter parameter = providerFactory.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            parameter.Direction = parameterDirection;
            return parameter;
        }
        #endregion

        #region 对数据源执行的 SQL 语句或存储过程。 为表示命令的、数据库特有的类提供一个基类
        /// <summary>
        /// 创建一个DbCommand对象  表示要对数据源执行的 SQL 语句或存储过程。 为表示命令的、数据库特有的类提供一个基类。
        /// </summary>
        /// <param name="sql">要执行的查询语句</param>   
        /// <param name="parameters">执行SQL查询语句所需要的参数</param>
        /// <param name="commandType">执行的SQL语句的类型</param>
        /// <returns></returns>
        private DbCommand CreateDbCommand(string sql, IList<DbParameter> parameters, CommandType commandType)
        {
            //DbConnection表示到数据库的连接
            try{
                DbConnection connection = providerFactory.CreateConnection();
                DbCommand command = providerFactory.CreateCommand();

                connection.ConnectionString = ConnectionString;
                //connection.Open();
                //获取或设置针对数据源运行的文本命令
                command.CommandText = sql;
                //指示或指定如何解释 CommandText 属性  CommandType 值之一。默认为 Text
                command.CommandType = commandType;
                //获取或设置此 DbCommand 使用的 DbConnection
                command.Connection = connection;
                if (!(parameters == null || parameters.Count == 0))
                {
                    foreach (DbParameter parameter in parameters)
                    {
                        //获取 DbParameter 对象的集合  SQL 语句或存储过程的参数
                        command.Parameters.Add(parameter);
                    }
                }
                return command;
            }
            catch(Exception)
            {
                return null;
            }
        }
        #endregion
    }
    /// <summary>
    /// 数据库类型枚举
    /// </summary>
    public enum DbProviderType : byte
    {
        SqlServer,
        MySql,
        SQLite,
        Oracle,
        ODBC,
        OleDb,
        Firebird,
        PostgreSql,
        DB2,
        Informix,
        SqlServerCe
    }

    #region 数据库操作的抽象工厂类的复写
    /// <summary>
    /// DbProviderFactory工厂类
    /// </summary>
    public class ProviderFactory
    {
        //添加的数据库的操作程序集的名称需符合DbProviderFactories.GetFactoryClasses()方法返回值标志InvariantName列的集合。
        /**
         * System.Data.Odbc
         * System.Data.OleDb
         * System.Data.OracleClient
         * System.Data.SqlClient
         * System.Data.SqlServerCe.3.5
         * System.Data.SqlServerCe.4.0
         * MySql.Data.MySqlClient
         * */
        private static Dictionary<DbProviderType, string> providerInvariantNames = new Dictionary<DbProviderType, string>();
        private static Dictionary<DbProviderType, DbProviderFactory> providerFactoies = new Dictionary<DbProviderType, DbProviderFactory>(20);
        static ProviderFactory()
        {
            //加载已知的数据库访问类的程序集
            providerInvariantNames.Add(DbProviderType.SqlServer, "System.Data.SqlClient");
            providerInvariantNames.Add(DbProviderType.OleDb, "System.Data.OleDb");
            providerInvariantNames.Add(DbProviderType.ODBC, "System.Data.ODBC");
            providerInvariantNames.Add(DbProviderType.Oracle, "Oracle.DataAccess.Client");
            providerInvariantNames.Add(DbProviderType.MySql, "MySql.Data.MySqlClient");
            providerInvariantNames.Add(DbProviderType.SQLite, "System.Data.SQLite");
            providerInvariantNames.Add(DbProviderType.Firebird, "FirebirdSql.Data.Firebird");
            providerInvariantNames.Add(DbProviderType.PostgreSql, "Npgsql");
            providerInvariantNames.Add(DbProviderType.DB2, "IBM.Data.DB2.iSeries");
            providerInvariantNames.Add(DbProviderType.Informix, "IBM.Data.Informix");
            providerInvariantNames.Add(DbProviderType.SqlServerCe, "System.Data.SqlServerCe");
        }
        /// <summary>
        /// 获取指定数据库类型对应的程序集名称
        /// </summary>
        /// <param name="providerType">数据库类型枚举</param>
        /// <returns></returns>
        public static string GetProviderInvariantName(DbProviderType providerType)
        {
            return providerInvariantNames[providerType];
        }
        /// <summary>
        /// 获取指定类型的数据库对应的DbProviderFactory
        /// </summary>
        /// <param name="providerType">数据库类型枚举</param>
        /// <returns></returns>
        public static DbProviderFactory GetDbProviderFactory(DbProviderType providerType)
        {
            //如果还没有加载，则加载该DbProviderFactory
            if (!providerFactoies.ContainsKey(providerType))
            {
                providerFactoies.Add(providerType, ImportDbProviderFactory(providerType));
            }
            return providerFactoies[providerType];
        }
        /// <summary>
        /// 加载指定数据库类型的DbProviderFactory
        /// </summary>
        /// <param name="providerType">数据库类型枚举</param>
        /// <returns></returns>
        private static DbProviderFactory ImportDbProviderFactory(DbProviderType providerType)
        {
            string providerName = providerInvariantNames[providerType];
            DbProviderFactory factory = null;
            try
            {
                //从全局程序集中查找
                //提供一个程序集的固定名称来的带一个DbProviderFactory的实例
                factory = DbProviderFactories.GetFactory(providerName);
            }
            catch (Exception )
            {
                factory = null;
            }
            return factory;
        }
    }
    #endregion
}
