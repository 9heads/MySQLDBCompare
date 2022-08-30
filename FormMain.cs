using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Configuration;

namespace MySQLDBCompare
{
    public partial class FormMain : Form
    {

        String DataBase1ConnectString {
            get;
            set;
        }
        String DataBase2ConnectString
        {
            get;
            set;
        }

        MySqlConnection DataBase1Connection
        {
            get;
            set;
        }
        MySqlConnection DataBase2Connection
        {
            get;
            set;
        }
        public FormMain()
        {
            InitializeComponent();
            Console.SetOut(new TextBoxWriter(this.richTextBoxResult));//传入richTextBox1控件
        }


        private void buttonTest1_Click(object sender, EventArgs e)
        {
            String server = textBoxServer1.Text;
            String port = textBoxPort1.Text;
            String user = textBoxUser1.Text;
            String pwd = textBoxPassword1.Text;

            String connectStr = DataBaseTools.buildConnectStr(server,port,"",user,pwd);

            MySqlConnection connection1= DataBaseTools.createConnection(connectStr);

            try
            {
                connection1.Open();
                if (connection1.State == ConnectionState.Open)
                {
                    MessageBox.Show("Connection Opened Successfully","测试连接1");
                    connection1.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Connection Fail:\n"+ex.Message,"测试连接1");
            }
            
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            DataBase1ConnectString= ConfigurationManager.ConnectionStrings["mysqlDataBase1"].ConnectionString;

            String[] connectParams = DataBase1ConnectString.Split(';');
            foreach(String param in connectParams){
                if (param.ToLower().Contains("server"))
                {
                    String[] pv = param.Split('=');

                    textBoxServer1.Text = pv[1];
                    continue;
                }
                if (param.ToLower().Contains("port"))
                {
                    String[] pv = param.Split('=');

                    textBoxPort1.Text = pv[1];
                    continue;
                }
                if (param.ToLower().Contains("uid"))
                {
                    String[] pv = param.Split('=');

                    textBoxUser1.Text = pv[1];
                    continue;
                }
                if (param.ToLower().Contains("pwd"))
                {
                    String[] pv = param.Split('=');

                    textBoxPassword1.Text = pv[1];
                    continue;
                }
            }


            DataBase2ConnectString = ConfigurationManager.ConnectionStrings["mysqlDataBase2"].ConnectionString;
            connectParams = DataBase2ConnectString.Split(';');
            foreach (String param in connectParams)
            {
                if (param.ToLower().Contains("server"))
                {
                    String[] pv = param.Split('=');

                    textBoxServer2.Text = pv[1];
                    continue;
                }
                if (param.ToLower().Contains("port"))
                {
                    String[] pv = param.Split('=');

                    textBoxPort2.Text = pv[1];
                    continue;
                }
                if (param.ToLower().Contains("uid"))
                {
                    String[] pv = param.Split('=');

                    textBoxUser2.Text = pv[1];
                    continue;
                }
                if (param.ToLower().Contains("pwd"))
                {
                    String[] pv = param.Split('=');

                    textBoxPassword2.Text = pv[1];
                    continue;
                }
            }


            ///测试用
            this.textBoxTable1.Text = "bas_code";
            this.textBoxTable2.Text = "bas_code";
        }

        private void buttonSave1_Click(object sender, EventArgs e)
        {

        }

        private void buttonSave2_Click(object sender, EventArgs e)
        {

        }

        private void buttonTest2_Click(object sender, EventArgs e)
        {
            String server = textBoxServer2.Text;
            String port = textBoxPort2.Text;
            String user = textBoxUser2.Text;
            String pwd = textBoxPassword2.Text;

            String connectStr = DataBaseTools.buildConnectStr(server, port, "", user, pwd);

            MySqlConnection connection2 = DataBaseTools.createConnection(connectStr);

            try
            {
                connection2.Open();
                if (connection2.State == ConnectionState.Open)
                {
                    MessageBox.Show("Connection Opened Successfully", "测试连接2");
                    connection2.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Fail:\n" + ex.Message, "测试连接2");
            }

        }

        private void buttonConnect1_Click(object sender, EventArgs e)
        {
            ///连接数据库1
            String server = textBoxServer1.Text;
            String port = textBoxPort1.Text;
            String user = textBoxUser1.Text;
            String pwd = textBoxPassword1.Text;

            String connectStr = DataBaseTools.buildConnectStr(server, port, "", user, pwd);

            DataBase1Connection = DataBaseTools.createConnection(connectStr);
            DataSet ds=DataBaseTools.query(DataBase1Connection, "SHOW databases");


            comboBoxDatabaseName1.DataSource = ds.Tables[0];

            comboBoxDatabaseName1.DisplayMember = "Database";
            comboBoxDatabaseName1.ValueMember = "Database";

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonSelectTable1_Click(object sender, EventArgs e)
        {
            FormSelectTable selectTable = new FormSelectTable();
            selectTable.DataBaseConnection = DataBase1Connection;

            Console.WriteLine("DataBase Name is " + this.comboBoxDatabaseName1.Text);
            selectTable.DataBaseName = this.comboBoxDatabaseName1.Text;

            SelectTableNameDelegate afterSelect = delegate (String tableName)
            {
                this.textBoxTable1.Text = tableName;
            };
            ///回调来显示选中的数据库表名称
            selectTable.SelectTable += afterSelect;
            selectTable.ShowDialog(this);
        }

      

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void buttonCompare_Click(object sender, EventArgs e)
        {
            

            string databaseName1 = this.comboBoxDatabaseName1.Text;
            ///获取table1
            string tableName1 = this.textBoxTable1.Text;

            if ("".Equals(tableName1))
            {
                MessageBox.Show("请设置table name1", "提示信息");
                return;
            }

            string databaseName2 = this.comboBoxDatabaseName2.Text;
            ///获取table2
            string tableName2 = this.textBoxTable2.Text;
            if ("".Equals(tableName2))
            {
                MessageBox.Show("请设置table name2", "提示信息");
                return;
            }

            string sql = "select * from {0}.{1}";

            DataSet ds1 = DataBaseTools.query(DataBase1Connection,String.Format(sql, databaseName1, tableName1));

            DataTable dataTable1 = ds1.Tables[0];

            ///获取并设置主键信息
            DataColumn[] keys = DataBaseTools.getPrimaryKey(DataBase1Connection, databaseName1, tableName1);

            if (keys.Length == 0)
            {
                MessageBox.Show(String.Format("表{0}未设置主键",tableName1), "提示信息-database1");
                return;
            }

            setPrimaryKey(dataTable1, keys);

            DataSet ds2 = DataBaseTools.query(DataBase2Connection, String.Format(sql, databaseName2,tableName2));
            DataTable dataTable2 = ds2.Tables[0];
            keys = DataBaseTools.getPrimaryKey(DataBase2Connection, databaseName2, tableName2);
            if (keys.Length == 0)
            {
                MessageBox.Show(String.Format("表{0}未设置主键", tableName2), "提示信息-database2");
                return;
            }
            setPrimaryKey(dataTable2, keys);

            Console.WriteLine(string.Format("#[{0:HH:mm:ss}]{1}\r\n", DateTime.Now, "开始比较<--"));
            Console.WriteLine("#表信息");
            Console.WriteLine(string.Format("#{0}.{1}", databaseName1, tableName1));
            Console.WriteLine("#比较结果信息");

            CompareTools compare = new CompareTools();


            compare.SetProgressbarStep += setProgressbarStepHandler;


            compare.TableSchema1 = databaseName1;
            compare.TableName1 = tableName1;

            compare.TableSchema2 = databaseName2;
            compare.TableName2 = tableName2;

            this.toolStripProgressBar1.Value = Convert.ToInt32(0);
            this.toolStripProgressBar1.Maximum = dataTable1.Rows.Count+ dataTable2.Rows.Count;

            compare.CompareData(dataTable1, dataTable2, this.richTextBoxResult);

            Console.WriteLine(string.Format("#[{0:HH:mm:ss}]{1}\r\n", DateTime.Now, "完成比较-->"));
        }
        void setPrimaryKey(DataTable dt, DataColumn[] keys)
        {
            List<DataColumn> tableKeys = new List<DataColumn>();
            foreach(DataColumn key in keys)
            {
                tableKeys.Add(dt.Columns[key.ColumnName]);
            }
            dt.PrimaryKey = tableKeys.ToArray();
        }

        void setProgressbarStepHandler(int step)
        {
            //如果当前调用方不是创建控件的一方，则需要使用this.Invoke()
            //在这里，ProgressBar控件是由主线程创建的，所以子线程要对该控件进行操作
            //必须执行this.InvokeRequired进行判断。
            if (this.InvokeRequired)
            {
                SetProgressbarStepDelegate setPro = new SetProgressbarStepDelegate(setProgressbarStepHandler);
                this.Invoke(setPro, new object[] { step, this.toolStripProgressBar1 });
            }
            else
            {
                this.toolStripProgressBar1.Value = Convert.ToInt32(step);
            }

        }
        private void buttonConnect2_Click(object sender, EventArgs e)
        {
            ///连接数据库2
            String server = textBoxServer2.Text;
            String port = textBoxPort2.Text;
            String user = textBoxUser2.Text;
            String pwd = textBoxPassword2.Text;

            String connectStr = DataBaseTools.buildConnectStr(server, port, "", user, pwd);

            DataBase2Connection = DataBaseTools.createConnection(connectStr);
            DataSet ds = DataBaseTools.query(DataBase2Connection, "SHOW databases");


            comboBoxDatabaseName2.DataSource = ds.Tables[0];

            comboBoxDatabaseName2.DisplayMember = "Database";
            comboBoxDatabaseName2.ValueMember = "Database";
        }

        private void buttonSelectTable2_Click(object sender, EventArgs e)
        {
            FormSelectTable selectTable = new FormSelectTable();
            selectTable.DataBaseConnection = DataBase2Connection;

            Console.WriteLine("DataBase Name is " + this.comboBoxDatabaseName2.Text);
            selectTable.DataBaseName = this.comboBoxDatabaseName2.Text;

            SelectTableNameDelegate afterSelect = delegate (String tableName)
             {
                 this.textBoxTable2.Text = tableName;
             };

            ///回调来显示选中的数据库表名称
            selectTable.SelectTable += afterSelect;

            selectTable.ShowDialog(this);
        }
    }
}
