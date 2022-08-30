using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace MySQLDBCompare
{
    public delegate void SetProgressbarStepDelegate(int step);
    class CompareTools
    {
        public SetProgressbarStepDelegate SetProgressbarStep { get; set; }

        public CompareTools()
        {

        }
        public string TableSchema1 { get; set; }

        public string TableName1 { get; set; }

        public string TableSchema2 { get; set; }
        public string TableName2 { get; set; }


        /// <summary>
        /// 比较两个表的数据
        /// </summary>
        /// <param name="dataTable1"></param>
        /// <param name="dataTable2"></param>
        /// <param name="richTextBoxResult"></param>
        public void CompareData(DataTable dataTable1, DataTable dataTable2, RichTextBox richTextBoxResult)
        {
            List<String> insertSqlList = new List<string>();
            List<String> updateSqlList = new List<string>();
            List<String> deleteSqlList = new List<string>();

            ///表1中存在的数据
            DataColumn[] keys = dataTable1.PrimaryKey;

            int step = 1;
            foreach (DataRow dr in dataTable1.Rows)
            {
                SetProgressbarStep(step);
                step++;
                String filterExpression = SQLHelper.buildFilterExpression(dr, keys);
                DataRow[] selectedRows = dataTable2.Select(filterExpression);

                if (null == selectedRows || selectedRows.Length == 0)
                {
                    //未找到数据-执行insert
                    ///Console.WriteLine(String.Format("data base2 未找到数据 {0}", filterExpression));
                    String insertSql = SQLHelper.buildInsertSQL(TableSchema2, TableName2, dataTable1, dr);
                    insertSqlList.Add(insertSql);
                }
                else
                {
                    //找到了数据，判断是否是相等

                    IEqualityComparer<DataRow> comparer = DataRowComparer.Default;

                    bool r = comparer.Equals(dr, selectedRows[0]);

                    if (r)
                    {
                        ///Console.WriteLine(String.Format("存在相同的数据 {0}", filterExpression));
                        
                    }
                    else
                    {
                        ///Console.WriteLine(String.Format("存在有差异的数据 {0}", filterExpression));

                        List<DataCell> resList = new List<DataCell>();

                        foreach (DataColumn col in dataTable1.Columns)
                        {
                            DataCell cell = new DataCell();

                            cell.ColumnName = col.ColumnName;

                            cell.Column = col;
                            if (false==CompareObject(dr[cell.ColumnName], selectedRows[0][cell.ColumnName]))
                            {
                                cell.Value = dr[cell.ColumnName];
                                resList.Add(cell);
                            }
                        }


                        //IEnumerable<DataCell> en2 = listA.Concat(listB).Except(listA.Intersect(listB, comparer2), comparer2);// 容斥原理

                        //foreach (DataCell cell in resList)
                        //{
                        //    Console.WriteLine(String.Format("--->>>> {0}={1}", cell.ColumnName,cell.Value));
                        //}

                        String updateSql= SQLHelper.buildUpdateSQL(TableSchema2, TableName2, resList, filterExpression);

                        ///Console.WriteLine(String.Format("update sql：{0}", updateSql));
                        updateSqlList.Add(updateSql);
                    }

                }
            }

            
            ///表2中存在，表1中不存在的数据
            keys = dataTable2.PrimaryKey;
            foreach (DataRow dr in dataTable2.Rows)
            {
                SetProgressbarStep(step);
                step++;
                String filterExpression = SQLHelper.buildFilterExpression(dr, keys);
                DataRow[] selectedRows = dataTable1.Select(filterExpression);

                if (null == selectedRows || selectedRows.Length == 0)
                {
                    //未找到数据-执行insert
                    //Console.WriteLine(String.Format("data base2 中需要删除的数据--》未找到数据 {0}", filterExpression));
                    ///生成删除SQL
                    string delSql=SQLHelper.buildDeleteSQL(TableSchema2,TableName2, filterExpression);
                    //Console.WriteLine(delSql);
                    deleteSqlList.Add(delSql);
                }

            }

            ///删除
            Console.WriteLine(String.Format("###delete{0}", "data base2"));
            foreach (String sql in deleteSqlList)
            {
                Console.WriteLine(String.Format("{0}", sql));
            }
            ///修改
            Console.WriteLine(String.Format("###update{0}", "data base2"));
            foreach (String sql in updateSqlList)
            {
                Console.WriteLine(String.Format("{0}", sql));
            }
            ///插入
            Console.WriteLine(String.Format("###insert {0}", "data base2"));
            foreach (String sql in insertSqlList)
            {
                Console.WriteLine(String.Format("{0}", sql));
            }
        }
        bool CompareObject(Object obj1, Object obj2)
        {
            
            if(null==obj1 && null == obj2)
            {
                return true;
            }

            if (null == obj1)
            {
                return false;
            }

            return obj1.Equals(obj2);

        }
    }

}
