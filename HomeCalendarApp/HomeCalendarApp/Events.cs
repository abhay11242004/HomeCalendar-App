using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data.SQLite;
using System.Xml.Linq;
using System.Globalization;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{
    // ====================================================================
    // CLASS: Events
    //        - A collection of Event items,
    //        - Read / write to file
    //        - etc
    // ====================================================================

    /// <summary>
    /// A collection of <c><see cref="Event"/></c> objects for <see cref="HomeCalendar">calendar program</see>.
    /// </summary>
    /// <remarks>
    /// The list of <c><see cref="Event"/></c> objects can be read from or written to a file specified.
    /// </remarks>
    public class Events
    {
        public const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

        private SQLiteConnection _connection;

        /// <summary>
        /// Creates a new instance of <c><see cref="Events"/></c> using a <paramref name="dbConnection"/>.
        /// </summary>
        /// <param name="dbConnection">A connection to the database.</param>
        /// <example>
        /// Demonstrates usage of <c><see cref="Events"/></c> constructor.
        /// <code>
        /// // Connects to database
        /// Database.existingDatabase("testDB");
        /// 
        /// // Creates a new events object
        /// Events events = new Events(Database.dbConnection);
        ///
        /// // Adds a new event object to the database
        /// events.Add(DateTime.Now, 2, 30.0, "Programming");
        /// </code>
        /// </example>
        public Events(SQLiteConnection dbConnection)
        {
            _connection = dbConnection;
        }

        /// <summary>
        /// Retrieves <c><see cref="Event"/></c> instance with <paramref name="id">given Id</paramref> from database.
        /// </summary>
        /// <param name="id">The Id of <c><see cref="Event"/></c> object to be retrieved.</param>
        /// <returns>The <c><see cref="Event"/></c> instance with <paramref name="i">given Id</paramref>.</returns>
        /// <exception cref="SQLiteException">Thrown when no <c><see cref="Event"/></c> object in database has <paramref name="i">given Id</paramref>.</exception>
        /// <example>
        /// Demonstrates usage of <c><see cref="GetEventFromId(int)"/></c>.
        /// <code>
        /// // Connects to database
        /// Database.existingDatabase("testDB");
        /// 
        /// // Creates a new events object
        /// Events events = new Events(Database.dbConnection);
        /// 
        /// // Displays details of event instance with Id of 3
        /// Event e = events.GetEventFromId(3);
        /// 
        /// Console.WriteLine(e.Details);
        /// </code>
        /// </example>
        public Event GetEventFromId(int id)
        {
            SQLiteCommand cmd = new SQLiteCommand(_connection)
            {
                CommandText = "SELECT StartDateTime, CategoryId, DurationInMinutes, Details FROM events WHERE Id = @id",
            };

            cmd.Parameters.AddWithValue("id", id);

            SQLiteDataReader r = cmd.ExecuteReader();

            if (!r.Read())
                throw new SQLiteException($"Cannot find event with id {id}");

            return new Event(id, DateTime.ParseExact(r.GetString(0), DATE_TIME_FORMAT, CultureInfo.InvariantCulture), r.GetInt32(1), r.GetDouble(2), r.GetString(3));
        }

        /// <summary>
        /// Adds a <c><see cref="Event"/></c> object with given parameters and auto-generated Id to the database.
        /// </summary>
        /// <param name="startDateTime">The start date and time of <c><see cref="Event"/></c>.</param>
        /// <param name="categoryId">The category Id of <c><see cref="Event"/></c>.</param>
        /// <param name="durationInMinutes">The duration of <c><see cref="Event"/></c> measured in minutes.</param>
        /// <param name="details">The details of <c><see cref="Event"/></c>.</param>
        /// <exception cref="SQLiteException">Thrown when <paramref name="categoryId"/> is not found in the database</exception>
        /// <example>
        /// Demonstrates usage of <c><see cref="Add(DateTime, int, double, string)"/></c>.
        /// <code>
        /// // Connects to database
        /// Database.existingDatabase("testDB");
        /// 
        /// // Creates a new events object
        /// Events events = new Events(Database.dbConnection);
        /// 
        /// // Adds a new event object to the database
        /// events.Add(DateTime.Now, 2, 30.0, "Programming");
        /// </code>
        /// </example>
        public void Add(DateTime startDateTime, int categoryId, Double durationInMinutes, String details)
        {
            SQLiteCommand cmd = new SQLiteCommand(_connection)
            {
                CommandText = "INSERT INTO events(StartDateTime, CategoryId, DurationInMinutes, Details) VALUES(@startDateTime, @categoryId, @durationInMinutes, @details)"
            };

            cmd.Parameters.AddWithValue("startDateTime", startDateTime.ToString(DATE_TIME_FORMAT));
            cmd.Parameters.AddWithValue("categoryId", categoryId);
            cmd.Parameters.AddWithValue("durationInMinutes", durationInMinutes);
            cmd.Parameters.AddWithValue("details", details);

            if (cmd.ExecuteNonQuery() < 1)
            {
                throw new SQLiteException("category id does not exist");
            }

            cmd.Dispose();
        }

        // ====================================================================
        // Delete Event
        // ====================================================================

        /// <summary>
        /// Deletes <c><see cref="Event"/></c> with <paramref name="id"/> from the database.
        /// </summary>
        /// <param name="id">The Id of <c><see cref="Event"/></c> object to be deleted.</param>
        /// <exception cref="SQLiteException">Thrown when <paramref name="id"/> of event is not found in the database</exception>
        /// <example>
        /// Demonstrates usage of <c><see cref="Delete(int)"/></c>.
        /// <code>
        /// // Connects to database
        /// Database.existingDatabase("testDB");
        /// 
        /// // Creates a new events object
        /// Events events = new Events(Database.dbConnection);
        /// 
        /// // Removes event object with Id of 2 in database
        /// events.Delete(2);
        /// </code>
        /// </example>
        public void Delete(int id)
        {
            SQLiteCommand cmd = new SQLiteCommand(_connection)
            {
                CommandText = "DELETE FROM events WHERE Id = @id"
            };

            cmd.Parameters.AddWithValue("id", id);

            if (cmd.ExecuteNonQuery() < 1)
            {
                throw new SQLiteException("Error, invalid event id");
            }

            cmd.Dispose();
        }

        // ====================================================================
        // Return list of Events
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================

        /// <summary>
        /// Retrieves the list of <c><see cref="Event"/></c> objects from database.
        /// </summary>
        /// <returns>The list of <c><see cref="Event"/></c> objects retrieved from database.</returns>
        /// <example>
        /// Demonstrates usage of <c><see cref="List()"/></c>.
        /// <code>
        /// // Connects to database
        /// Database.existingDatabase("testDB");
        /// 
        /// // Creates a new events object
        /// Events events = new Events(Database.dbConnection);
        /// 
        /// // Prints details of all event objects retrieved from database
        /// foreach (Event event in events.List())
        ///     Console.WriteLine(event.Details);
        /// </code>
        /// </example>
        public List<Event> List()
        {
            var list = new List<Event>();

            SQLiteCommand cmd = new SQLiteCommand(_connection)
            {
                CommandText = "SELECT Id, StartDateTime, CategoryId, DurationInMinutes, Details FROM events ORDER BY Id"
            };

            SQLiteDataReader r = cmd.ExecuteReader();

            while (r.Read())
                list.Add(new Event(r.GetInt32(0), DateTime.ParseExact(r.GetString(1), DATE_TIME_FORMAT, CultureInfo.InvariantCulture), r.GetInt32(2), r.GetDouble(3), r.GetString(4)));

            return list;
        }

        /// <summary>
        /// Updates the given <c><see cref="event"/></c> instance stored in database with given properties.
        /// </summary>
        /// <param name="id">The Id of <c><see cref="event"/></c> to update in database.</param>
        /// <param name="startDateTime">The new start date time of <c><see cref="event"/></c>.</param>
        /// <param name="durationInMinutes">The new duration in minutes of <c><see cref="event"/></c>.</param>
        /// <param name="details">The new details of <c><see cref="event"/></c>.</param>
        /// <param name="categoryId">The new category Id of <c><see cref="event"/></c>.</param>
        /// <exception cref="SQLiteException">Thrown when <paramref name="id"/> of event is not found in the database</exception>
        /// <example>
        /// Demonstrates usage of <c><see cref="UpdateProperties(int, DateTime, double, string, int)"/></c>.
        /// <code>
        /// // Connects to database
        /// Database.existingDatabase("testDB");
        /// 
        /// // Creates a new events object
        /// Events events = new Events(Database.dbConnection);
        /// 
        /// // Updates the event with id of 2
        /// events.UpdateProperties(2, new DateTime(), 20.0, "Coffee break", 1);
        /// </code>
        /// </example>

        public void UpdateProperties(int id, DateTime startDateTime, Double durationInMinutes, String details, int categoryId)
        {
            SQLiteCommand cmd = new SQLiteCommand(_connection)
            {
                CommandText = "UPDATE events SET StartDateTime = @startDateTime, CategoryId = @categoryId, DurationInMinutes = @durationInMinutes, Details = @details WHERE Id = @id"
            };

            cmd.Parameters.AddWithValue("startDateTime", startDateTime.ToString(DATE_TIME_FORMAT));
            cmd.Parameters.AddWithValue("categoryId", categoryId);
            cmd.Parameters.AddWithValue("durationInMinutes", durationInMinutes);
            cmd.Parameters.AddWithValue("details", details);
            cmd.Parameters.AddWithValue("id", id);

            if (cmd.ExecuteNonQuery() < 1)
            {
                throw new SQLiteException("Error, invalid event id");
            }

            cmd.Dispose();
        }
    }
}

