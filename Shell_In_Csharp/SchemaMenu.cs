using Npgsql;
using System.Data;

namespace Shell_In_Csharp
{
    public static class SchemaMenu
    {
        public static void DatabaseTables(NpgsqlConnection con)
        {
            using (var conn = con)
            {
                conn.Open();
                var Schemas = con.GetSchema("Tables");

                foreach (DataRow schema in Schemas.Rows)
                {
                    Console.WriteLine(schema["TABLE_NAME"]);
                }
                con.Close();
            }
        }

        public static NpgsqlConnection ConnectToDatabase(string host, string port, string database, string Username, string password)
        {
            string connectionString = $"Host={host}; port={port};Database={database}; User Id={Username}; Password={password}";
            NpgsqlConnection con = new NpgsqlConnection(connectionString);
            return con;
        }

        public static List<string> ShowSchemas(NpgsqlConnection conn)
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand("SELECT nspname AS schema_name FROM pg_namespace WHERE nspname NOT IN ('information_schema', 'pg_catalog', 'pg_toast') ORDER BY nspname;", conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    int t = 1;
                    List<string> list = new List<string>();
                    while (reader.Read())
                    {
                        var schema = $"{t}.{reader.GetString(0)}";  // Print schema name
                        list.Add(schema);
                        t++;
                    }
                    list.Add($"{t}.Back");
                    return list;
                }
            }
        }

        public static void RetrievePostgresFunctions(NpgsqlConnection con)
        {
            // SQL query to retrieve function names in the 'public' schema
            string query = @"
                SELECT 
                    p.proname AS function_name
                    FROM pg_catalog.pg_proc p
                    LEFT JOIN pg_catalog.pg_namespace n ON n.oid = p.pronamespace
                    WHERE n.nspname = 'public'
                    ORDER BY function_name;
                ";

            using (var cmd = new NpgsqlCommand(query, con))
            {
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"Function: {reader["function_name"]}");
                    }
                }
                con.Close();
            }
        }

        public static void RetrievePostgresProcedures(NpgsqlConnection con)
        {
            // SQL query to retrieve procedure names in the 'public' schema
            string query = @"
                SELECT 
                    p.proname AS procedure_name
                    FROM pg_catalog.pg_proc p
                    LEFT JOIN pg_catalog.pg_namespace n ON n.oid = p.pronamespace
                    WHERE n.nspname = 'public'
                    AND p.prokind = 'p'  -- 'p' indicates stored procedures
                    ORDER BY procedure_name;
                ";

            using (var cmd = new NpgsqlCommand(query, con))
            {
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"Procedure: {reader["procedure_name"]}");
                    }
                }
                con.Close();
            }
        }


        public static void RetrievePostgresViews(NpgsqlConnection con)
        {
            // SQL query to retrieve view names in the 'public' schema
            string query = @"
                SELECT 
                    table_name AS view_name
                    FROM information_schema.views
                    WHERE table_schema = 'public'
                    ORDER BY view_name;
                ";

            using (var cmd = new NpgsqlCommand(query, con))
            {
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"View: {reader["view_name"]}");
                    }
                }
                con.Close();
            }
        }



    }
}
