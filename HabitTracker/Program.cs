using Microsoft.Data.Sqlite;
using System.Globalization;

namespace HabitTracker {

    class Habit {
        public int ID { get; set; }
        public DateTime Date { get; set; }  
        public int Quantity { get; set; }
    }

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
                    case "3":
                        DeleteRecord();
                        continue;
                    case "4":
                        UpdateRecord();
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
            GetAllRecords();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void InsertRecord() {
            string date = GetDateInput();
            int quantity = GetNumberInput();

            string command = $"INSERT into {tableName} (date, quantity) VALUES('{date}', {quantity})";
            SendCommand(command);
        }
        static private void DeleteRecord() {
            GetAllRecords();
            int id = GetRecordId();

            string command = $"DELETE from {tableName} where Id = {id}";
            SendCommand(command);
        }

        static private void UpdateRecord() {
            GetAllRecords();
            Console.WriteLine("Updating specific record:");

            int id = GetRecordId();
            if (!CheckIfExists(id)) {
                Console.WriteLine($"Record: {id} does not exist!");
                return;
            }

            string date = GetDateInput();
            int quantity = GetNumberInput();

            string command = $"UPDATE {tableName} SET date = '{date}', quantity = {quantity} WHERE Id = {id}";
            SendCommand(command);
        }
        static void GetAllRecords() {
            using (var connection = new SqliteConnection(connectionString)) {
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText =
                    $"SELECT * from {tableName} ";

                List<Habit> habits = new();
                SqliteDataReader reader = tableCommand.ExecuteReader();

                if (reader.HasRows) {
                    while (reader.Read()) {
                        var habit = new Habit {
                            ID = reader.GetInt32(0),
                            Date = DateTime.ParseExact(reader.GetString(1), "dd-mm-yy", new CultureInfo("en-US")),
                            Quantity = reader.GetInt32(2)
                        };

                        habits.Add(habit);
                    }

                    Console.WriteLine(@"  ID  DATE                     QUANTITY");
                    foreach (var habit in habits) {
                        Console.Write($"| {habit.ID} ");
                        Console.Write($"| {habit.Date.ToString()} ");
                        Console.WriteLine($"| {habit.Quantity} |");
                    }
                }
                else {
                    Console.WriteLine("No rows found!");
                }
            }
        }

        static bool CheckIfExists(int id) {
            using (var connection = new SqliteConnection(connectionString)) {
                connection.Open();

                var checkCommand = connection.CreateCommand();
                checkCommand.CommandText =
                    $"SELECT EXISTS (SELECT 1 FROM {tableName} WHERE Id = {id})";
                var checkQuery = Convert.ToInt32( checkCommand.ExecuteScalar());

                return  checkQuery > 0;
            }
        }

        static string GetDateInput() {

            string dateInput = "";
            bool success = false;
            while (!success) {
                Console.WriteLine(@"Please insert the date (dd-mm-yy)");
                dateInput = Console.ReadLine();

                if (DateTimeOffset.TryParse(dateInput, out DateTimeOffset result)){
                    success = true;
                }
                else {
                    Console.WriteLine("Invalid entry! Try again!");
                }
            }
            return dateInput;
        }
        private static int GetNumberInput() {
            Console.WriteLine(@"Please enter the quantity");

            string numberInput = Console.ReadLine();
            int value;
            bool success = int.TryParse(numberInput, out value);

            if (!success || value == 0) {
                Console.WriteLine("Invalid entry! Pelase try again.");
                value = GetNumberInput();
            }

            return value;
        }

        static int GetRecordId() {
            int id = 0;
            bool success = false;
            string recordId;

            while (!success) {
                Console.WriteLine("Enter record ID number:");
                recordId = Console.ReadLine();

                success = int.TryParse(recordId, out id);

                if (!success) {
                    Console.WriteLine("Invalid input! Try again!");
                }
            }

            return id;
        }

    }
}
