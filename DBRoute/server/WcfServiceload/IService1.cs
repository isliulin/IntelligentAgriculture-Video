using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using SIXH.DBUtility;

namespace WcfServiceload
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: 在此添加您的服务操作

        //非查询
        //[OperationContract]
        //int ExecuteNonQuery_ConStrAndType(string connectionString, string sql, IList<DbParameter> paraList, CommandType commandType);
        ////查询
        //[OperationContract]
        //DataTable ExecuteDataTable_ConStrAndType(string connectionString, string sql, IList<DbParameter> paraList, CommandType commandType);
        [OperationContract]
        int ExecuteNonQuery_ConStrAndType(string connectionString, string sql, Dictionary<string, object> paraList, CommandType commandType);
        //查询
        [OperationContract]
        DataTable ExecuteDataTable_ConStrAndType(string connectionString, string sql, Dictionary<string, object> paraList, CommandType commandType);
    }

    // 使用下面示例中说明的数据约定将复合类型添加到服务操作。
    // 可以将 XSD 文件添加到项目中。在生成项目后，可以通过命名空间“WcfServiceload.ContractType”直接使用其中定义的数据类型。
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
