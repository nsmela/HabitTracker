using Microsoft.Data.Sqlite;
using System;

namespace HabitTracker {

    class Program {
        const string connectionString = @"Data Source=habit-Tracker.db";
        const string tableName = "yourHabit";

        static void Main(string[] args) {
            CreateDatabase();
            GetUserInput();
        }

        static void CreateDatabase() {

            /*Creating a connection passing the connection string as an argument
            This will create the database for you, there's no need to manually create it.
            And no need to use File.Create().*/
            using (var connection = new SqliteConnection(connectionString)) {
                //Creating the command that will be sent to the database
                using (var tableCmd = connection.CreateCommand()) {

                    connection.Open();
                    //Declaring what is that command (in SQL syntax)
                    tableCmd.CommandText =
                        @"CREATE TABLE IF NOT EXISTS yourHabit (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT,
                    Quantity INTEGER
                    )";

                    // Executing the command, which isn't a query, it's not asking to return data from the database.
                    tableCmd.ExecuteNonQuery();
                    connection.Close();
                }
                // We don't need to close the connection or the command. The 'using statement' does that for us.
            }

        }

        static void GetUserInput() {

            bool closeApp = false;
            while (!closeApp) {
                string mainMenu = @"
MAIN MENU
What would you like to do?
Type your choice:
0: Quit
1: View All Records
2: Insert Record
3: Delete Record
4: Update Record
----------------------------";

                Console.Clear();
                Console.WriteLine(mainMenu);
                var input = Console.ReadLine();

                switch (input) {
                    case "0":
                        closeApp = true;
                        Console.WriteLine("Goodbye");
                        continue;
                    case "1":
                        ViewAllRecords();
                        continue;
                    case "2":
                        InsertRecord();
                        continue;
                    default:
                        Console.WriteLine("Invalid entry. Please try again.");
                        Console.ReadKey();
                        continue;
                }
            }

        }

        static void SendCommand(string command) {
            using (var connection = new SqliteConnection(connectionString)) {
                //Creating the command that will be sent to the database
                using (var tableCmd = connection.CreateCommand()) {

                    connection.Open();
                    //Declaring what is that command (in SQL syntax)
                    tableCmd.CommandText = command;

                    // Executing the command, which isn't a query, it's not asking to return data from the database.
                    tableCmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        static void ViewAllRecords() {
            Console.Clear();
            
        }

        private static void InsertRecord() {
            string date = GetDateInput();
            int quantity = GetNumberOutput();

            string command = $"INSERT into {tableName} (date, quantity) VALUES('{date}', {quantity})";
            SendCommand(command);
        }


        static string GetDateInput() {
            Console.WriteLine(@"Please insert the date (dd/mm/yy) or type 0 to return to main menu");

            string dateInput = Console.ReadLine();
            if (dateInput == "0") GetUserInput();

            //TODO: check date format

            return dateInput;
        }
        private static int GetNumberOutput() {
            Console.WriteLine(@"Please enter the quantity");

            string numberInput = Console.ReadLine();
            int value;
            bool success = int.TryParse(numberInput, out value);

            if (!success || value == 0) {
                Console.WriteLine("Invalid entry! Pelase try again.");
                value = GetNumberOutput();
            }

            return value;
        }
    }
}
