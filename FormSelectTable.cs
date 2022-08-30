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

namespace MySQLDBCompare
{
    public delegate void SelectTableNameDelegate(string tableName);

    public partial class FormSelectTable : Form
    {

        public string DataBaseName{
            get; set;
        }

        public MySqlConnection DataBaseConnection
        {
            get;
            set;
        }

        public FormSelectTable()
        {
            InitializeComponent();
        }

        public event SelectTableNameDelegate SelectTable;

        private BindingSource bindingSource1 = new BindingSource();

        private void FormSelectTable_Load(object sender, EventArgs e)
        {
            string queryTables = "SELECT t.TABLE_NAME ,t.TABLE_COMMENT FROM information_schema.tables t WHERE TABLE_SCHEMA = '{0}'  ORDER BY TABLE_NAME";

            try
            {
                if(null== DataBaseConnection)
                {
                    return;
                }
                DataSet  ds =DataBaseTools.query(DataBaseConnection,String.Format(queryTables,DataBaseName));


                //this.dataGridView1.DataSource = ds.Tables[0];

           
                // Set up the DataGridView.
                //dataGridView1.Dock = DockStyle.Fill;

                // Automatically generate the DataGridView columns.
                dataGridView1.AutoGenerateColumns = true;

                // Set up the data source.
                bindingSource1.DataSource = ds.Tables[0];
                dataGridView1.DataSource = bindingSource1;

                //bindingSource1.DataMember = "TABLE_NAME";

                //dataGridView1.DataBindings.Add("TABLE_NAME", ds.Tables[0], "TABLE_NAME");
                // Automatically resize the visible rows.
                dataGridView1.AutoSizeRowsMode =  DataGridViewAutoSizeRowsMode.AllCells;

                // Set the DataGridView control's border.
                dataGridView1.BorderStyle = BorderStyle.Fixed3D;

                // Put the cells in edit mode when user enters them.
                dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
            }
            catch (Exception)
            {
                MessageBox.Show("To run this sample replace connection.ConnectionString" +
                    " with a valid connection string to a Northwind" +
                    " database accessible to your system.", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                System.Threading.Thread.CurrentThread.Abort();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ///关闭选择，判断是否选择了

            Console.WriteLine("关闭选择数据库表");
            if (this.dataGridView1.SelectedRows.Count>0)
            {
                //SelectTable()

                object value=this.dataGridView1.SelectedRows[0].Cells[0].Value;

                SelectTable(value.ToString());
            }

            this.Close();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
