using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell_In_Csharp
{
    public  class Tables
    {
        public static List<string> DatabaseTables(NpgsqlConnection con)
        {
            List<string> tables = new List<string>();
            var Schemas = con.GetSchema("Tables");

            foreach (DataRow schema in Schemas.Rows)
            {
                var table_name = (string)(schema["TABLE_NAME"]);
                tables.Add(table_name);
            }
            return tables;
        }
    }
}
