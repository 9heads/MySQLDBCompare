using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLDBCompare
{
    class DataCell
    {
        public string ColumnName { get; set; }

        public DataColumn Column { get; set; }

        public Object Value { get; set; }

        public DataCell()
        {

        }
    }
}
