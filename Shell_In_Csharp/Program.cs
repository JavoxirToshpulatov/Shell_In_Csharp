using Shell_In_Csharp;

internal class Program
{
    private static void Main()
    {
        try
        {
            bool exit = false;
                        //string schema1 = "schema";
        server:
            Console.Write("Enter Host[localhost]: ");
            string? host = Console.ReadLine();

            if (string.IsNullOrEmpty(host))
                host = "localhost";

            port:
            Console.Write("Enter port[5432]: ");
            string? port = Console.ReadLine();

            if (string.IsNullOrEmpty(port))
                port = "5432";

            int result = 0;
            if (!int.TryParse(port, out result))
            {
                Console.WriteLine("Port cannot be like a string");
                goto port;
            }

            Console.Write("Enter Username[postgres]: ");
            string? username = Console.ReadLine();

            if (string.IsNullOrEmpty(username))
                username = "postgres";

            password:
            Console.Write("Enter password: ");
            string? password = Console.ReadLine();
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Password cannot be empty");
                goto password;
            }

            while (!exit)
            {
                try
                {
                    var connection = SchemaMenu.ConnectToServer(host, port, username, password);

                    using (var conn = connection)
                    {
                        Console.WriteLine("Successfully connected to server");
                        Console.ReadLine();

                        List<string> databases = SchemaMenu.Databases(conn);
                        databaseGoTo:
                        int databseIndex = ArrowIndex(databases, "Databases");

                        if (databases[databseIndex] == "Back to Host")
                        {
                            Console.Clear();
                            goto server;
                        }

                        var connection1 = SchemaMenu.ConnectToDatabase(host, port, username, password, databases[databseIndex]);
                        SchemaMenu.MainFeatures(connection1);
                    }
                }
                catch
                {
                    Console.WriteLine("Username or password incorrect");
                    Console.ReadLine();
                    Console.Clear();
                    goto server;
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Main();
        }
    }



    public static int ArrowIndex(List<string> list, string name)
    {
        int selectIndex = 0;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("\t\t\t" + name);

            for (int i = 0; i < list.Count; i++)
            {
                if (i == selectIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.WriteLine(list[i]);
                Console.ResetColor();
            }

            ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
            if (consoleKeyInfo.Key == ConsoleKey.UpArrow) selectIndex = (selectIndex - 1 + list.Count) % list.Count;
            else if (consoleKeyInfo.Key == ConsoleKey.DownArrow) selectIndex = (selectIndex + 1) % list.Count;
            else if (consoleKeyInfo.Key == ConsoleKey.Enter) return selectIndex;
        }
    }
}