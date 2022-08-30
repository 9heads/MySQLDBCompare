using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using System.Data;

namespace MySQLDBCompare
{
    class DataBaseTools
    {
        /// <summary>
        /// 获取连接对象
        /// </summary>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public static MySqlConnection createConnection(String connectStr)
        {
            MySqlConnection conn = new MySqlConnection(connectStr);

            return conn;

        }

        /// <summary>
        /// 构建connectString
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static String buildConnectStr(String server,String port,String database,String user,String password)
        {
            String connectStr;
            if (null == database || "".Equals(database))
            {
                connectStr = String.Format("Server={0};Port={1};uid={2};pwd={3}", server, port, user, password);
            }
            else
            {
                connectStr = String.Format("Server={0};Port={1};Database={2};uid={3};pwd={4}", server, port, database, user, password);
            }
            

            return connectStr;
        }

        public static DataSet query(MySqlConnection connection,string queryString)
        {
            
            DataSet data = new DataSet();
            try
            {
                connection.Open();
                MySqlDataAdapter command = new MySqlDataAdapter(queryString, connection);
                command.Fill(data, "ds");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return data;
        }

        /// <summary>
        /// 获取表的主键信息
        /// </summary>
        /// <param name="tableSchema"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataColumn[] getPrimaryKey(MySqlConnection connection,String tableSchema, String tableName)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("SELECT	k.COLUMN_NAME,	t.table_name,table_schema,constraint_type");
            sql.Append(" from information_schema.table_constraints t");
            sql.Append(" JOIN information_schema.key_column_usage k USING ( constraint_name, table_schema, table_name )");
            sql.Append(" WHERE	t.constraint_type = 'PRIMARY KEY'");
            sql.Append(" and t.TABLE_SCHEMA='").Append(tableSchema).Append("' and t.TABLE_NAME ='").Append(tableName).Append("'");


            Console.WriteLine(String.Format("获取主键SQL：{0}",sql.ToString()));
            DataSet ds = query(connection, sql.ToString());

            List<DataColumn> keys = new List<DataColumn>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DataColumn key = new DataColumn();
                key.ColumnName = dr["COLUMN_NAME"].ToString();
                keys.Add(key);
            }
            return keys.ToArray();

        }

    }
}
