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

        public static void TablesMenu (NpgsqlConnection connection1)
        {
            schema:
            List<string> tablesProperty = new List<string>()
                {
                "Columns",
                "Insert data",
                "Select data",
                "Update data",
                "Delete data",
                "Add Column",
                "Modify Column",
                "Drop Column",
                "Back"

                };
            List<string> TableStrings = new List<string>()
                    {
                        "Create table",
                        "Select table",
                        "Back"
                    };
            int IndexTable = Program.ArrowIndex(TableStrings, " ");
            switch (IndexTable)
            {
                case 0:
                    SchemaMenu.CreateTable(connection1);
                    Console.ReadLine();
                    goto schema;
                    break;
                case 1:
                    List<string> tables = Tables.DatabaseTables(connection1);
                    if (tables.Count == 0)
                    {
                        Console.WriteLine("No Tables yet");
                        Console.ReadLine();
                        goto schema;
                    }
                    int keyTables = Program.ArrowIndex(tables, " ");
                    string selectedTable = tables[keyTables];
                tables:
                    int key = Program.ArrowIndex(tablesProperty, " ");

                    switch (key)
                    {
                        case 0:
                            List<string> columns = CRUDOperations.GetTableColumns(selectedTable, connection1);
                            Program.ArrowIndex(columns, "Columns");
                            Console.ReadLine();
                            goto tables;
                        case 1:
                            CRUDOperations.InsertData(connection1, selectedTable);
                            Console.ReadLine();
                            goto tables;
                        case 2:
                            //select
                            CRUDOperations.SelectQuery(selectedTable, connection1);
                            Console.ReadLine();
                            goto tables;
                        case 3:
                            CRUDOperations.UpdateData(connection1, selectedTable);
                            Console.ReadLine();
                            goto tables;
                        //update
                        case 4:
                            Console.Write("Enter the WHERE clause for deletion (e.g., id = 1): ");
                            string whereClause = Console.ReadLine();
                            CRUDOperations.DeleteQuery(selectedTable, whereClause, connection1);
                            Console.ReadLine();
                            goto tables;
                        //delete
                        case 5:
                            SchemaMenu.AddColumn(selectedTable, connection1);
                            Console.ReadLine();
                            goto tables;
                        case 6:
                            SchemaMenu.ModifyColumn(selectedTable, connection1);
                            Console.ReadLine();
                            goto tables;
                        case 7:
                            SchemaMenu.DropColumn(selectedTable, connection1);
                            Console.ReadLine();
                            goto tables;
                        case 8:
                            goto schema;
                            break;
                    }
                    Console.ReadLine();
                    break;
                case 2:
                    SchemaMenu.MainFeatures(connection1);
                    break;

            }
        }
    }
}
