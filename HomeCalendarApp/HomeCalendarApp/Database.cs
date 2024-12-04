using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Threading;
using System.Data.SqlClient;

// ===================================================================
// Very important notes:
// ... To keep everything working smoothly, you should always
//     dispose of EVERY SQLiteCommand even if you recycle a 
//     SQLiteCommand variable later on.
//     EXAMPLE:
//            Database.newDatabase(GetSolutionDir() + "\\" + filename);
//            var cmd = new SQLiteCommand(Database.dbConnection);
//            cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES('Whatever')";
//            cmd.ExecuteNonQuery();
//            cmd.Dispose();
//
// ... also dispose of reader objects
//
// ... by default, SQLite does not impose Foreign Key Restraints
//     so to add these constraints, connect to SQLite something like this:
//            string cs = $"Data Source=abc.sqlite; Foreign Keys=1";
//            var con = new SQLiteConnection(cs);
//
// ===================================================================


namespace Calendar
{
    /// <summary>
    /// Provides creation / connection to new / existing database for Home Calendar App.
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Gets the connection to database.
        /// </summary>
        public static SQLiteConnection dbConnection { get { return _connection; } }
        private static SQLiteConnection _connection;

        // ===================================================================
        // create and open a new database
        // ===================================================================

        /// <summary>
        /// Initializes <paramref name="filename">new database file</paramref> for Home Calendar App and populates <c><see cref="dbConnection"/>connection</c>.
        /// </summary>
        /// <param name="filename">The new database file to connect to.</param>
        /// <example>
        /// Demonstrates usage of <c><see cref="newDatabase(string)"/></c>.
        /// <code>
        /// // Creates and connects to a new database
        /// Database.newDatabase("newdb.db");
        /// 
        /// // Creates categories with new database and populates categories with default data
        /// var categories = new Categories(Database.dbConnection, true);
        /// 
        /// // Retrieves the list of categories stored in database
        /// var list = categories.List();
        /// </code>
        /// </example>
        public static void newDatabase(string filename)
        {

            // If there was a database open before, close it and release the lock
            CloseDatabaseAndReleaseFile();

            // your code

            _connection = new SQLiteConnection($"Data Source={filename}; Foreign Keys=1");

            _connection.Open();

            string[] nonQueryCommands = new string[]
            {
                "DROP TABLE IF EXISTS events",
                "DROP TABLE IF EXISTS categories",
                "DROP TABLE IF EXISTS categoryTypes",

                "CREATE TABLE categoryTypes(Id INTEGER PRIMARY KEY AUTOINCREMENT, Description TEXT)",
                "CREATE TABLE categories(Id INTEGER PRIMARY KEY AUTOINCREMENT, Description TEXT, TypeId INT NOT NULL, FOREIGN KEY(TypeId) REFERENCES categoryTypes(Id))",
                "CREATE TABLE events(Id INTEGER PRIMARY KEY AUTOINCREMENT, StartDateTime TEXT, Details TEXT, DurationInMinutes DOUBLE, CategoryId INT NOT NULL, FOREIGN KEY(CategoryId) REFERENCES categories(Id))"
            };

            foreach (string command in nonQueryCommands)
                ExecuteNonQuery(command);

        }

        // ===================================================================
        // open an existing database
        // ===================================================================

        /// <summary>
        /// Populates <c><see cref="dbConnection"/>connection</c> to an <paramref name="filename">existing database</paramref>.
        /// </summary>
        /// <param name="filename">The existing database file to connect to.</param>
        /// <example>
        /// Demonstrates usage of <c><see cref="existingDatabase(string)"/></c>.
        /// <code>
        /// // Connects to an existing database
        /// Database.existingDatabase("testdb.db");
        /// 
        /// // Creates categories with existing database
        /// var categories = new Categories(Database.dbConnection, false);
        /// 
        /// // Retrieves the list of categories stored in database
        /// var list = categories.List();
        /// </code>
        /// </example>
        public static void existingDatabase(string filename)
        {

            CloseDatabaseAndReleaseFile();

            // your code

            _connection = new SQLiteConnection($"Data Source={filename}; Foreign Keys=1");

            _connection.Open();
        }

        // ===================================================================
        // close existing database, wait for garbage collector to
        // release the lock before continuing
        // ===================================================================

        /// <summary>
        /// Stops connection to current database.
        /// </summary>
        /// <example>
        /// Demonstrates usage of <c><see cref="CloseDatabaseAndReleaseFile"/></c>.
        /// <code>
        /// // Connects to a database
        /// Database.existingDatabase("database1.db");
        /// 
        /// // Stops connection
        /// Database.CloseDatabaseAndReleaseFile();
        /// 
        /// // Connects to another database
        /// Database.existingDatabase("database2.db");
        /// </code>
        /// </example>
        public static void CloseDatabaseAndReleaseFile()
        {
            if (Database.dbConnection != null)
            {
                // close the database connection
                Database.dbConnection.Close();


                // wait for the garbage collector to remove the
                // lock from the database file
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private static void ExecuteNonQuery(string strCommand)
        {
            SQLiteCommand command = new SQLiteCommand(_connection);

            command.CommandText = strCommand;
            command.ExecuteNonQuery();

            command.Dispose();
        }
    }
}
