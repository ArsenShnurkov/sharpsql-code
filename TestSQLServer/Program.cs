using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Threading;

namespace TestSQLServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string connectionString = @"Initial Catalog=master;" +
                            @"Data Source=localhost;" +
                            @"User ID=sa;" +
                            @"Password=javacash";

                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT * from systypes", connection);

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine(reader["Name"]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            Console.WriteLine("= alles ok =");
            Thread.Sleep(50000);
        }
    }
}
