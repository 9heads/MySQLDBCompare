using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLDBCompare
{
    /// <summary>
    /// sql处理类
    /// </summary>
    class SQLHelper
    {
        /// DATE_FORMAT('2017-09-20 08:30:45',   '%Y-%m-%d %H:%i:%S');
        static string DATE_FORMAT_DEFAULT = "%Y/%m/%d %H:%i:%S";



        public static string buildDeleteSQL(String tableSchema, String tableName, String filterExpression)
        {

            return string.Format("delete from {0}.{1} where {2};", tableSchema, tableName, filterExpression);

        }

        public static string buildUpdateSQL(String tableSchema, String tableName, List<DataCell> cells, String filterExpression)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(string.Format("update {0}.{1} set ", tableSchema, tableName));
            for (int i = 0; i < cells.Count; i++)
            {
                DataCell cell = cells[i];

                ///Console.WriteLine("{0} DataType is {1}", cell.ColumnName, cell.Column.DataType);

                switch (cell.Column.DataType.ToString())
                {
                    case "System.DateTime":
                        sql.Append(string.Format("{0}=str_to_date('{1}','{2}')", cell.ColumnName, cell.Value, DATE_FORMAT_DEFAULT));
                        break;
                    default:
                        sql.Append(string.Format("{0}='{1}'", cell.ColumnName, cell.Value));
                        break;
                }

                
                if (i < cells.Count-1)
                {
                    sql.Append(",");
                }
            }

            sql.Append(string.Format(" where {0};",filterExpression));
            return sql.ToString();
        }

        public static string buildInsertSQL(String tableSchema, String tableName,DataTable datatable, DataRow data)
        {
            StringBuilder sql = new StringBuilder();
            

            StringBuilder cols = new StringBuilder();
            StringBuilder values = new StringBuilder();
            for (int i = 0; i < datatable.Columns.Count; i++)
            {
                String colName = datatable.Columns[i].ColumnName;
                cols.Append(colName);

                Object value = data[colName];
                ///Console.WriteLine(datatable.Columns[i].DataType);
                switch (datatable.Columns[i].DataType.ToString())
                {
                    case "System.DateTime":
                        values.Append(string.Format("str_to_date('{0}','{1}')", data[colName], DATE_FORMAT_DEFAULT));
                        break;
                    case "System.Int16":
                    case "System.Int32":
                        if (null==value)
                        {
                            values.Append(string.Format("{0}", "null"));
                        }
                        else
                        {
                            if ("System.DBNull".Equals(value.GetType().ToString()))
                            {
                                values.Append(string.Format("{0}", "null"));
                            }
                            else
                            {
                                values.Append(string.Format("{0}", data[colName]));
                            }
                            
                        }
                        
                        break;
                    default:
                        values.Append(string.Format("'{0}'", data[colName]));

                        break;
                }

                if (i < datatable.Columns.Count - 1)
                {
                    cols.Append(",");
                    values.Append(",");
                }
            }

            sql.Append(string.Format("insert into {0}.{1} ({2})values({3});", tableSchema, tableName,cols.ToString(),values.ToString()));
            return sql.ToString();
        }

        public static String buildFilterExpression(DataRow dr, DataColumn[] keys)
        {
            string filterExpression = "";
            foreach (DataColumn key in keys)
            {
                if (filterExpression.Length == 0)
                {
                    filterExpression = String.Format("{0}='{1}'", key.ColumnName, dr[key].ToString());
                }
                else
                {
                    filterExpression = String.Format("{0} and {1}='{2}'", filterExpression, key.ColumnName, dr[key].ToString());
                }

            }
            return filterExpression;
        }
    }
}
