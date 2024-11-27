using System;
using System.Data.SqlClient;
using System.IO;

namespace Reading_Data_from_SQL
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=SampleDB;Trusted_Connection=True;";
            string csvFilePath = @"C:\Users\sunka\Downloads\ExportedData.csv";
       


            try
            {
                // Prepare the CSV file
                using (StreamWriter writer = new StreamWriter(csvFilePath))
                {
                    // Write the CSV header
                    writer.WriteLine("Column1,Column2");

                    // Establish connection for reading
                    using (SqlConnection readConnection = new SqlConnection(connectionString))
                    {
                        readConnection.Open();
                        Console.WriteLine("Connected to SQL Server Express successfully.");

                        // Query to read data from SourceTable
                        string selectQuery = "SELECT Column1, Column2 FROM SourceTable";

                        using (SqlCommand selectCommand = new SqlCommand(selectQuery, readConnection))
                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            Console.WriteLine("Reading data from SourceTable...");

                            while (reader.Read())
                            {
                                // Read data from SourceTable
                                string column1 = reader["Column1"].ToString();
                                string column2 = reader["Column2"].ToString();
                                Console.WriteLine($"Read: Column1 = {column1}, Column2 = {column2}");

                                // Save data to DestinationTable
                                SaveToDestinationTable(connectionString, column1, column2);

                                // Write data to the CSV file
                                writer.WriteLine($"{column1},{column2}");
                            }
                        }
                    }
                }

                Console.WriteLine($"Data successfully saved to {csvFilePath}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        static void SaveToDestinationTable(string connectionString, string column1, string column2)
        {
            // Insert data into DestinationTable
            string insertQuery = "INSERT INTO DestinationTable (Column1, Column2) VALUES (@Column1, @Column2)";

            using (SqlConnection writeConnection = new SqlConnection(connectionString))
            {
                writeConnection.Open();
                using (SqlCommand insertCommand = new SqlCommand(insertQuery, writeConnection))
                {
                    insertCommand.Parameters.AddWithValue("@Column1", column1);
                    insertCommand.Parameters.AddWithValue("@Column2", column2);

                    insertCommand.ExecuteNonQuery();
                    Console.WriteLine($"Data saved to DestinationTable: Column1 = {column1}, Column2 = {column2}");
                }
            }
        }
    }
}
