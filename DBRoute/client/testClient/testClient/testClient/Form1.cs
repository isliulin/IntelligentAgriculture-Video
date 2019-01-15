using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using testClient.ServiceReference1;

namespace testClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
        /*
        DB_IP=127.0.0.1
;本地数据库地址
DB_Name = datatransceiver
; 数据库名称
 DB_User = root
 ; 数据库使用账户名
  DB_Password = zhaowuzu147
  ; 数据库密码
   DB_Type = MySql
   ; 数据库类型SqlServer或MySql
    [netSqlGroup]
    */
        private void button1_Click(object sender, EventArgs e)
        {
            string sql = @"select * from test";

            String Con = string.Format("Data Source={0};Database={1};User={2};Password={3}", "127.0.0.1", "datatransceiver", "root", "zhaowuzu147");
            using (
                ServiceReference1.Service1Client sc = new ServiceReference1.Service1Client())
            {
                sc.Open();

                string ss = sc.GetData(1);
                //int ii = sc.ExecuteNonQuery_ConStrAndType(Con, "delete from 3d_current where siteNo ='6107'", null, CommandType.Text);
                //DataTable dt = sc.ExecuteDataTable_ConStrAndType(Con, "select * from 3d_current", null, CommandType.Text);
                //DataTable dt = sc.ExecuteDataTable_ConStrAndType(Con, "pro_getPersonInfo", paraList, CommandType.StoredProcedure);
                DataTable result = sc.ExecuteDataTable_ConStrAndType(Con, sql, null, CommandType.Text);
                sc.Close();
            }
        }
    }
}
