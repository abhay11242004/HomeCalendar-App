using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================


namespace Calendar
{
    // ====================================================================
    // CLASS: HomeCalendar
    //        - Combines a Categories Class and an Events Class
    //        - One File defines Category and Events File
    //        - etc
    // ====================================================================

    /// <summary>
    /// Represents the calendar program storing <c><see cref="Categories"/></c> and <c><see cref="Events"/></c>.
    /// </summary>
    /// <remarks>
    /// <c><see cref="Categories"/></c> and <c><see cref="Events"/></c> stored in <c><see cref="HomeCalendar"/></c> database can be modified.
    /// </remarks>
    /// <example>
    /// <br />
    /// <b>Demonstrates usage of <c><see cref="HomeCalendar"/></c>.</b>
    /// <code>
    /// // database that stores events, categories and categoryTypes
    /// const string CALENDAR_DB = "testdbInput";
    /// 
    /// // Creates home calendar using an existing database (without restoring original state of database)
    /// HomeCalendar homeCalendar = new HomeCalendar(CALENDAR_DB);
    /// </code>
    /// <br />
    /// <b>Tables in <c>testdbInput</c> database</b>
    /// <code>
    /// categories
    /// categoryTypes
    /// events
    /// </code>
    /// <br />
    /// <b>Content of <c>categories</c> table</b>
    /// <code>
    /// <![CDATA[
    /// Id  |   Description         |   TypeID
    /// 1   |   School              |   1
    /// 2   |   Work                |   1
    /// 3   |   Fun                 |   1
    /// 4   |   Medical             |   1
    /// 5   |   Sleep               |   1
    /// 6   |   Working             |   2
    /// 7   |   On call             |   2
    /// 8   |   Canadian Holidays   |   4
    /// 9   |   Vacation            |   3
    /// 10  |   Wellness days       |   3
    /// 11  |   Birthdays           |   3
    /// 12  |   Non Standard        |   1
    /// ]]>
    /// </code>
    /// <br />
    /// <b>Content of <c>events</c> table</b>
    /// <code>
    /// <![CDATA[
    /// Id	|	StartDateTime	    |	DurationInMinutes	|	Details	                |	CategoryId
    /// 1	|	2018-01-10 10:00:00	|	40.0	            |	App Dev Homework        |	3
    /// 2	|	2020-01-09 00:00:00	|	1440.0	            |	Honolulu	            |	9
    /// 3	|	2020-01-10 00:00:00	|	1440.0	            |	Honolulu	            |	9
    /// 4	|	2020-01-20 11:00:00	|	180.0	            |	On call security	    |	7
    /// 5	|	2018-01-11 19:30:00	|	15.0	            |	staff meeting           |	2
    /// 6	|	2020-01-01 00:00:00	|	1440.0	            |	New Year's	            |	8
    /// 7	|	2020-01-12 00:00:00	|	1440.0	            |	Wendy's birthday	    |	11
    /// 8	|	2018-01-11 10:15:00	|	60.0	            |	Sprint retrospective    |	2
    /// 9	|	2019-01-11 09:30:00	|	60.0	            |	training	            |	2
    /// ]]>
    /// </code>
    /// <br />
    /// <code>
    /// // Displays all categories (id , description, type)
    /// foreach (Category category in homeCalendar.categories.List())
    ///     Console.WriteLine(string.Format("{0, 4} | {1, -24} | {2, -16}", category.Id, category.Description, category.Type));
    /// </code>
    /// <br />
    /// <code>
    /// <b>Output</b>
    ///   12 | Non Standard             | Event
    ///    1 | School                   | Event
    ///    2 | Work                     | Event
    ///    3 | Fun                      | Event
    ///    4 | Medical                  | Event
    ///    5 | Sleep                    | Event
    ///    6 | Working                  | Event
    ///    7 | On call                  | Event
    ///    8 | Canadian Holidays        | Holiday
    ///    9 | Vacation                 | Event
    ///   10 | Wellness days            | Event
    ///   11 | Birthdays                | Event
    /// </code>
    /// <code>
    /// // Adds a nap event starting now, lasting for 20 minutes to home calendar
    /// homeCalendar.events.Add(DateTime.Now, 5, 20, "Nap");
    /// 
    /// // Saves calendar data to CALENDAR_FILE, including the newly added event
    /// homeCalendar.SaveToFile(CALENDAR_FILE);
    /// 
    /// // Displays all events stored in home calendar in the work category taking place before or at 2020/01/01 12 AM
    /// const string FORMAT = "{0, -24} | {1, -24} | {2, 14} | {3, -18} | {4, 11}";
    /// string[] header = { "Short Description", "Start Date Time", "Duration (min)", "Category", "Busy Time (min)" };
    /// 
    /// Console.WriteLine(string.Format(FORMAT, header));
    /// 
    /// foreach (CalendarItem c in homeCalendar.GetCalendarItems(null, new DateTime(2020, 1, 1), true, 2))
    /// {
    ///     object[] row = { c.ShortDescription ?? "", c.StartDateTime, c.DurationInMinutes, c.Category, c.BusyTime };    
    /// 
    ///     Console.WriteLine(string.Format(FORMAT, row));
    /// }
    /// </code>
    /// <br />
    /// <code>
    /// <b>Output</b>
    /// Short Description        | Start Date Time          | Duration (min) | Category           | Busy Time (min)
    /// Sprint retrospective     | 1/11/2018 10:15:00 AM    |             60 | Work               |          60
    /// staff meeting            | 1/11/2018 7:30:00 PM     |             15 | Work               |          75
    /// </code>
    /// </example>
    public class HomeCalendar
    {
        private Categories _categories;
        private Events _events;

        private static DateTime DEFAULT_START = new DateTime(1900, 1, 1);
        private static DateTime DEFAULT_END = new DateTime(2500, 1, 1);


        // ====================================================================
        // Properties
        // ===================================================================

        // Properties (categories and events object)

        /// <summary>
        /// Gets <c><see cref="Categories"/></c> of current instance.
        /// </summary>
        public Categories categories { get { return _categories; } }

        /// <summary>
        /// Gets <c><see cref="Events"/></c> of current instance.
        /// </summary>
        public Events events { get { return _events; } }

        // -------------------------------------------------------------------
        // Constructor (existing calendar ... must specify file)
        // -------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of <c><see cref="HomeCalendar"/></c>, populating its <c><see cref="Categories"/></c> and <c><see cref="Events"/></c> from files specified in the content of <paramref name="calendarFileName"/>.
        /// </summary>
        /// <param name="databaseFile">The database to read from containing <c><see cref="Categories"/></c> and <c><see cref="Events"/></c>.</param>
        /// <param name="newDB">If true, a database is created at <paramref name="databaseFile"/>, populated with default data and connected to by current instance. Otherwise, the database at <paramref name="databaseFile"/> is connected to by current instance.</param>
        /// <example>
        /// Demonstrates usage of parameterized constructor of <c><see cref="HomeCalendar"/></c>.
        /// <code>
        /// // Creates a new instance of home calendar with existing database
        /// HomeCalendar homeCalendar = new HomeCalendar("testdb.db");
        /// 
        /// // Displays details of all events
        /// foreach (Event e in homeCalendar.events.List())
        ///     Console.WriteLine(e.Details);
        /// </code>
        /// </example>
        public HomeCalendar(String databaseFile, bool newDB = false)
        {
            // if database exists, and user doesn't want a new database, open existing DB
            if (!newDB && File.Exists(databaseFile))
            {
                Database.existingDatabase(databaseFile);
            }

            // file did not exist, or user wants a new database, so open NEW DB
            else
            {
                Database.newDatabase(databaseFile);
                newDB = true;
            }

            // create the category object
            _categories = new Categories(Database.dbConnection, newDB);

            // create the _events course
            _events = new Events(Database.dbConnection);

        }

        #region GetList

        private enum GroupBy
        {
            None,
            Month,
            Category,
            MonthAndCategory
        }

        private SQLiteDataReader GetRawCalendarItems(DateTime? start, DateTime? end, int? categoryID, string? month, GroupBy groupBy)
        {
            const string MONTH_COLUMN = "SUBSTR(E.StartDateTime, 1, 7)";

            // builds query text for retrieving list
            StringBuilder txt = new StringBuilder("SELECT ");

            switch (groupBy)
            {
                case GroupBy.None:
                    txt.Append("C.Id, C.Description, E.Id, E.StartDateTime, E.DurationInMinutes, E.Details");
                    break;

                case GroupBy.Month:
                    txt.Append($"SUM(E.DurationInMinutes), {MONTH_COLUMN}");
                    break;

                case GroupBy.Category:
                    txt.Append("C.Id, C.Description");
                    break;

                case GroupBy.MonthAndCategory:
                    txt.Append($"SUM(E.DurationInMinutes), {MONTH_COLUMN}, C.Id");
                    break;
            }

            txt.Append(" FROM categories C INNER JOIN events E ON C.Id = E.CategoryId WHERE E.StartDateTime >= @start AND E.StartDateTime <= @end");

            if (categoryID is not null)
                txt.Append(" AND E.CategoryId = @categoryID");

            if (month is not null)
                txt.Append($" AND {MONTH_COLUMN} = @monthFilter");


            switch (groupBy)
            {
                case GroupBy.None:
                    txt.Append($" ORDER BY E.StartDateTime");
                    break;
                case GroupBy.Month:
                    txt.Append($" GROUP BY {MONTH_COLUMN} ORDER BY E.StartDateTime");
                    break;

                case GroupBy.Category:
                    txt.Append(" GROUP BY C.Id ORDER BY C.Description");
                    break;

                case GroupBy.MonthAndCategory:
                    txt.Append($" GROUP BY BY {MONTH_COLUMN} AND C.Id ORDER BY E.StartDateTime");
                    break;
            }


            SQLiteCommand cmd = new SQLiteCommand(txt.ToString(), Database.dbConnection);

            // populates query parameters for retrieving list

            cmd.Parameters.AddWithValue("start", (start ?? DEFAULT_START).ToString(Events.DATE_TIME_FORMAT));
            cmd.Parameters.AddWithValue("end", (end ?? DEFAULT_END).ToString(Events.DATE_TIME_FORMAT));

            if (categoryID is not null)
                cmd.Parameters.AddWithValue("categoryID", categoryID);

            if (month is not null)
                cmd.Parameters.AddWithValue("monthFilter", month);

            return cmd.ExecuteReader();
        }


        // ============================================================================
        // Get all events list
        // ============================================================================

        /// <summary>
        /// Gets the list of <c><see cref="CalendarItem"/></c> objects generated using <c><see cref="Categories"/></c> and <c><see cref="Events"/></c> of current instance, following requirements specifed by given parameters.
        /// </summary>
        /// <param name="Start">Minimum StartDateTime of <c><see cref="CalendarItem"/></c> objects to return. (if provided)</param>
        /// <param name="End">Maximum StartDateTime of <c><see cref="CalendarItem"/></c> objects to return. (if provided)</param>
        /// <param name="FilterFlag">If true, the list of <c><see cref="CalendarItem"/></c> objects to return must have category Id of <paramref name="CategoryID"/>. If false, the list of <c><see cref="CalendarItem"/></c> objects to return can have any category Id. </param>
        /// <param name="CategoryID">The category Id all <c><see cref="CalendarItem"/></c> objects in the list to return must have if <paramref name="FilterFlag"/> is true.</param>
        /// <returns>The list of <c><see cref="CalendarItem"/></c> objects having StartDateTime within interval formed by <paramref name="Start"/> and <paramref name="End"/> inclusively (if provided), having category Id of <paramref name="CategoryID"/> if <paramref name="FilterFlag"/> is true, sorted by StartDateTime in ascending order.</returns>
        /// <example>
        /// 
        /// For all examples below, assume the calendar file contains the
        /// following elements:
        /// 
        /// <code>
        /// Cat_ID  Event_ID  StartDateTime           Details                 DurationInMinutes
        ///    3       1      1/10/2018 10:00:00 AM   App Dev Homework             40
        ///    9       2      1/9/2020 12:00:00 AM    Honolulu		             1440
        ///    9       3      1/10/2020 12:00:00 AM   Honolulu                   1440
        ///    7       4      1/20/2020 11:00:00 AM   On call security            180
        ///    2       5      1/11/2018 7:30:00 PM    staff meeting                15
        ///    8       6      1/1/2020 12:00:00 AM    New Year's                 1440
        ///   11       7      1/12/2020 12:00:00 AM   Wendy's birthday           1440
        ///    2       8      1/11/2018 10:15:00 AM   Sprint retrospective         60
        /// </code>
        ///
        /// <b>Getting a list of ALL calendar items.</b>
        /// 
        /// <code>
        /// // Creates home calendar from existing database
        /// HomeCalendar calendar = new HomeCalendar("testdb");
        /// 
        /// // Displays calendar items
        /// foreach (var ci in calendar.GetCalendarItems(null, null, false, 0))
        /// {
        ///   Console.WriteLine(
        ///      String.Format("{0} {1,-20}  {2,8} {3,12}",
        ///          ci.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
        ///          ci.ShortDescription,
        ///          ci.DurationInMinutes, ci.BusyTime)
        ///    );
        /// }
        /// </code>
        /// 
        /// Output:
        /// <code>
        /// 2018/Jan/10/10/00 App Dev Homework            40           40
        /// 2018/Jan/11/10/15 Sprint retrospective        60          100
        /// 2018/Jan/11/19/30 staff meeting               15          115
        /// 2020/Jan/01/00/00 New Year's                1440         1555
        /// 2020/Jan/09/00/00 Honolulu                  1440         2995
        /// 2020/Jan/10/00/00 Honolulu                  1440         4435
        /// 2020/Jan/12/00/00 Wendy's birthday          1440         5875
        /// 2020/Jan/20/11/00 On call security           180         6055
        /// </code>
        /// 
        /// <b>Getting a list of calendar items with StartDateTime no later than 2020 January 10th, 12 am.</b>
        /// 
        /// <code>
        /// // Creates home calendar from existing database
        /// HomeCalendar calendar = new HomeCalendar("testdb");
        /// 
        /// // Displays calendar items
        /// foreach (var ci in calendar.GetCalendarItems(null, new DateTime(2020, 1, 10), false, 0))
        /// {
        ///   Console.WriteLine(
        ///      String.Format("{0} {1,-20}  {2,8} {3,12}",
        ///          ci.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
        ///          ci.ShortDescription,
        ///          ci.DurationInMinutes, ci.BusyTime)
        ///    );
        /// }
        /// </code>
        /// 
        /// Output:
        /// <code>
        /// 2018/Jan/10/10/00 App Dev Homework            40           40
        /// 2018/Jan/11/10/15 Sprint retrospective        60          100
        /// 2018/Jan/11/19/30 staff meeting               15          115
        /// 2020/Jan/01/00/00 New Year's                1440         1555
        /// 2020/Jan/09/00/00 Honolulu                  1440         2995
        /// 2020/Jan/10/00/00 Honolulu                  1440         4435
        /// </code>
        /// 
        /// <b>Getting a list of calendar items with StartDateTime between 2020 January 1st, 12 am and 2020 January 10th, 12 am inclusively.</b>
        /// 
        /// <code>
        /// // Creates home calendar from existing database
        /// HomeCalendar calendar = new HomeCalendar("testdb");
        /// 
        /// // Displays calendar items
        /// foreach (var ci in calendar.GetCalendarItems(new DateTime(2020, 1, 11), new DateTime(2020, 1, 10), false, 0))
        /// {
        ///   Console.WriteLine(
        ///      String.Format("{0} {1,-20}  {2,8} {3,12}",
        ///          ci.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
        ///          ci.ShortDescription,
        ///          ci.DurationInMinutes, ci.BusyTime)
        ///    );
        /// }
        /// </code>
        /// 
        /// Output:
        /// <code>
        /// 2018/Jan/10/10/00 App Dev Homework            40           40
        /// 2018/Jan/11/10/15 Sprint retrospective        60          100
        /// 2018/Jan/11/19/30 staff meeting               15          115
        /// 2020/Jan/01/00/00 New Year's                1440         1555
        /// 2020/Jan/09/00/00 Honolulu                  1440         2995
        /// 2020/Jan/10/00/00 Honolulu                  1440         4435
        /// </code>
        /// 
        /// <b>Getting a list of calendar items with category Id of 2.</b>
        /// 
        /// <code>
        /// // Creates home calendar from existing database
        /// HomeCalendar calendar = new HomeCalendar("testdb");
        /// 
        /// // Displays calendar items
        /// foreach (var ci in calendar.GetCalendarItems(null, null, true, 2))
        /// {
        ///   Console.WriteLine(
        ///      String.Format("{0} {1,-20}  {2,8} {3,12} {4, 4}",
        ///          ci.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
        ///          ci.ShortDescription,
        ///          ci.DurationInMinutes, ci.BusyTime, ci.CategoryID)
        ///    );
        /// }
        /// </code>
        /// 
        /// Output:
        /// <code>
        /// 2018/Jan/11/10/15 Sprint retrospective        60          100    2
        /// 2018/Jan/11/19/30 staff meeting               15          115    2
        /// </code>
        /// 
        /// <b>Getting a list of calendar items with category Id of 2 and StartDateTime no later than 2018 January 11th 12 pm.</b>
        /// 
        /// <code>
        /// // Creates home calendar from existing database
        /// HomeCalendar calendar = new HomeCalendar("testdb");
        /// 
        /// // Displays calendar items
        /// foreach (var ci in calendar.GetCalendarItems(null, new DateTime(2018, 1, 11, 12, 0, 0), true, 2))
        /// {
        ///   Console.WriteLine(
        ///      String.Format("{0} {1,-20}  {2,8} {3,12} {4, 4}",
        ///          ci.StartDateTime.ToString("yyyy/MMM/dd/HH/mm"),
        ///          ci.ShortDescription,
        ///          ci.DurationInMinutes, ci.BusyTime, ci.CategoryID)
        ///    );
        /// }
        /// </code>
        /// 
        /// Output:
        /// <code>
        /// 2018/Jan/11/10/15 Sprint retrospective        60          100    2
        /// </code>
        /// </example>

        public List<CalendarItem> GetCalendarItems(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {


            Double totalBusyTime = 0;
            List<CalendarItem> items = new List<CalendarItem>();

            SQLiteDataReader r = GetRawCalendarItems(Start, End, (FilterFlag) ? CategoryID : null, null, GroupBy.None);

            // populates list from query output
            while (r.Read())
            {
                Double durationInMinutes = r.GetDouble(4);

                totalBusyTime += durationInMinutes;

                items.Add(new CalendarItem
                {
                    CategoryID = r.GetInt32(0),
                    Category = r.GetString(1),
                    EventID = r.GetInt32(2),
                    StartDateTime = DateTime.ParseExact(r.GetString(3), Events.DATE_TIME_FORMAT, CultureInfo.InvariantCulture),
                    DurationInMinutes = durationInMinutes,
                    ShortDescription = r.GetString(5),
                    BusyTime = totalBusyTime
                });
            }

            return items;
        }

        // ============================================================================
        // Group all events month by month (sorted by year/month)
        // returns a list of CalendarItemsByMonth which is 
        // "year/month", list of calendar items, and totalBusyTime for that month
        // ============================================================================

        /// <summary>
        /// Gets the list of <c><see cref="CalendarItemsByMonth"/></c> objects generated using <c><see cref="Categories"/></c> and <c><see cref="Events"/></c> of current instance, following requirements specified by given parameters.
        /// </summary>
        /// <param name="Start">Minimum StartDateTime of <c><see cref="CalendarItem"/></c> objects within <c><see cref="CalendarItemsByMonth"/></c> objects to return. (if provided)</param>
        /// <param name="End">Maximum StartDateTime of <c><see cref="CalendarItem"/></c> objects within <c><see cref="CalendarItemsByMonth"/></c> objects to return. (if provided)</param>
        /// <param name="FilterFlag">If true, the list of <c><see cref="CalendarItem"/></c> objects within <c><see cref="CalendarItemsByMonth"/></c> objects to return must have category Id of <paramref name="CategoryID"/>. If false, the list of <c><see cref="CalendarItem"/></c> objects within <c><see cref="CalendarItemsByMonth"/></c> objects to return can have any category Id. </param>
        /// <param name="CategoryID">The category Id all <c><see cref="CalendarItem"/></c> objects within <c><see cref="CalendarItemsByMonth"/></c> objects to return must have if <paramref name="FilterFlag"/> is true.</param>
        /// <returns>The list of <c><see cref="CalendarItemsByMonth"/></c> objects having StartDateTime within interval formed by <paramref name="Start"/> and <paramref name="End"/> inclusively (if provided), having category Id of <paramref name="CategoryID"/> if <paramref name="FilterFlag"/> is true, grouped by month of StartDateTime and sorted by StartDateTime in ascending order.</returns>
        /// <example>
        /// Demonstrates usage of <c><see cref="GetCalendarItemsByMonth(DateTime?, DateTime?, bool, int)"/></c>
        /// <code>
        /// // Creates home calendar from existing database
        /// HomeCalendar homeCalendar = new HomeCalendar("testdb");
        /// 
        /// // Defines row format for events table
        /// const string ROW_FORMAT = "{0, -20} | {1, -18} | {2, 8} | {3, -18} | {4, 9}";
        ///
        /// // Defines header for events table
        /// string header = string.Format(ROW_FORMAT, "Short Description", "Start Date Time", "Duration", "Category Name", "Busy Time");
        ///
        /// // Defines row separator for events table
        /// string separator = new string('-', header.Length);
        ///
        /// // Gets all calendar items grouped by month and sorted by StartDateTime in ascending order
        /// var calendarItemsByMonthList = homeCalendar.GetCalendarItemsByMonth(null, null, false, -1);
        ///
        /// // Displays events table
        /// 
        /// // Displays header
        /// Console.WriteLine(header);
        ///
        /// // Displays rows
        /// foreach (CalendarItemsByMonth calendarItemsByMonth in calendarItemsByMonthList)
        /// {
        ///     Console.WriteLine(separator);
        ///
        ///     // For each month displays month string and total busy time
        ///     Console.WriteLine(string.Format(ROW_FORMAT, "", calendarItemsByMonth.Month ?? "", "", "", calendarItemsByMonth.TotalBusyTime));
        ///
        ///     // For each calendar item in this month displays its information
        ///     foreach (CalendarItem ci in calendarItemsByMonth.Items)
        ///         Console.WriteLine(ROW_FORMAT, ci.ShortDescription ?? "", ci.StartDateTime.ToString("yyyy/MMM/dd HH:mm"), ci.DurationInMinutes, ci.Category ?? "", ci.BusyTime);
        /// }
        /// </code>
        /// 
        /// For content of events file, check example of <c><see cref="GetCalendarItems(DateTime?, DateTime?, bool, int)"/></c>.
        /// 
        /// Output:
        /// <code>
        /// <![CDATA[
        /// Short Description    | Start Date Time    | Duration | Category Name      | Busy Time
        /// -------------------------------------------------------------------------------------
        ///                      | 2018/01            |          |                    |       115
        /// App Dev Homework     | 2018/Jan/10 10:00  |       40 | Fun                |        40
        /// Sprint retrospective | 2018/Jan/11 10:15  |       60 | Work               |       100
        /// staff meeting        | 2018/Jan/11 19:30  |       15 | Work               |       115
        /// -------------------------------------------------------------------------------------
        ///                      | 2020/01            |          |                    |      5940
        /// New Year's           | 2020/Jan/01 00:00  |     1440 | Canadian Holidays  |      1555
        /// Honolulu             | 2020/Jan/09 00:00  |     1440 | Vacation           |      2995
        /// Honolulu             | 2020/Jan/10 00:00  |     1440 | Vacation           |      4435
        /// Wendy's birthday     | 2020/Jan/12 00:00  |     1440 | Birthdays          |      5875
        /// On call security     | 2020/Jan/20 11:00  |      180 | On call            |      6055
        /// ]]>
        /// </code>
        /// </example>
        public List<CalendarItemsByMonth> GetCalendarItemsByMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            const int MONTH_STRING_START_INDEX = 5;

            var itemsByMonth = new List<CalendarItemsByMonth>();

            SQLiteDataReader months = GetRawCalendarItems(Start, End, (FilterFlag) ? CategoryID : null, null, GroupBy.Month);

            double totalBusyTime = 0;

            while (months.Read())
            {
                string yearAndMonth = months.GetString(1);

                char[] a = yearAndMonth.ToCharArray();

                a[4] = '/';

                string yearAndMonthOtherFormat = new string(a);

                CalendarItemsByMonth itemsThisMonth = new CalendarItemsByMonth
                {
                    TotalBusyTime = months.GetDouble(0),
                    Month = yearAndMonthOtherFormat,
                    Items = new List<CalendarItem>()
                };

                SQLiteDataReader thisMonth = GetRawCalendarItems(Start, End, (FilterFlag) ? CategoryID : null, yearAndMonth, GroupBy.None);

                while (thisMonth.Read())
                {
                    double durationInMinutes = thisMonth.GetDouble(4);
                    totalBusyTime += durationInMinutes;

                    itemsThisMonth.Items.Add(new CalendarItem
                    {
                        CategoryID = thisMonth.GetInt32(0),
                        Category = thisMonth.GetString(1),
                        EventID = thisMonth.GetInt32(2),
                        StartDateTime = DateTime.ParseExact(thisMonth.GetString(3), Events.DATE_TIME_FORMAT, CultureInfo.InvariantCulture),
                        DurationInMinutes = durationInMinutes,
                        ShortDescription = thisMonth.GetString(5),
                        BusyTime = durationInMinutes
                    });
                }

                itemsByMonth.Add(itemsThisMonth);
            }

            return itemsByMonth;
        }

        // ============================================================================
        // Group all events by category (ordered by category name)
        // ============================================================================

        /// <summary>
        /// Gets the list of <c><see cref="CalendarItemsByCategory"/></c> objects generated using <c><see cref="Categories"/></c> and <c><see cref="Events"/></c> of current instance, following requirements specified by given parameters.
        /// </summary>
        /// <param name="Start">Minimum StartDateTime of <c><see cref="CalendarItem"/></c> objects within <c><see cref="CalendarItemsByCategory"/></c> objects to return. (if provided)</param>
        /// <param name="End">Maximum StartDateTime of <c><see cref="CalendarItem"/></c> objects within <c><see cref="CalendarItemsByCategory"/></c> objects to return. (if provided)</param>
        /// <param name="FilterFlag">If true, the list of <c><see cref="CalendarItem"/></c> objects within <c><see cref="CalendarItemsByCategory"/></c> objects to return must have category Id of <paramref name="CategoryID"/>. If false, the list of <c><see cref="CalendarItem"/></c> objects within <c><see cref="CalendarItemsByCategory"/></c> objects to return can have any category Id. </param>
        /// <param name="CategoryID">The category Id all <c><see cref="CalendarItem"/></c> objects within <c><see cref="CalendarItemsByCategory"/></c> objects to return must have if <paramref name="FilterFlag"/> is true.</param>
        /// <returns>The list of <c><see cref="CalendarItemsByCategory"/></c> objects having StartDateTime within interval formed by <paramref name="Start"/> and <paramref name="End"/> inclusively (if provided), having category Id of <paramref name="CategoryID"/> if <paramref name="FilterFlag"/> is true, grouped by category and sorted by details of the category in ascending order.</returns>
        /// <example>
        /// Demonstrates usage of <c><see cref="GetCalendarItemsByCategory(DateTime?, DateTime?, bool, int)"/></c>
        /// <code>
        /// // Creates home calendar from existing database
        /// HomeCalendar calendar = new HomeCalendar("testdb");
        /// 
        /// // Defines row format for events table
        /// const string ROW_FORMAT = "{0, -20} | {1, -18} | {2, 8} | {3, -18} | {4, 9}";
        ///
        /// // Defines header for events table
        /// string header = string.Format(ROW_FORMAT, "Short Description", "Start Date Time", "Duration", "Category Name", "Busy Time");
        ///
        /// // Defines row separator for events table
        /// string separator = new string('-', header.Length);
        ///
        /// // Gets all calendar items grouped by category and sorted by StartDateTime in ascending order
        /// var calendarItemsByCategoryList = homeCalendar.GetCalendarItemsByCategory(null, null, false, -1);
        ///
        /// // Displays events table
        /// 
        /// // Displays header
        /// Console.WriteLine(header);
        ///
        /// // Displays rows
        /// foreach (CalendarItemsByCategory calendarItemsByCategory in calendarItemsByCategoryList)
        /// {
        ///     Console.WriteLine(separator);
        ///
        ///     // For each category displays category string and total busy time
        ///     Console.WriteLine(string.Format(ROW_FORMAT, "", "", "", calendarItemsByCategory.Category, calendarItemsByCategory.TotalBusyTime));
        ///
        ///     // For each calendar item in this category displays its information
        ///     foreach (CalendarItem ci in calendarItemsByCategory.Items)
        ///         Console.WriteLine(ROW_FORMAT, ci.ShortDescription ?? "", ci.StartDateTime.ToString("yyyy/MMM/dd HH:mm"), ci.DurationInMinutes, ci.Category ?? "", ci.BusyTime);
        /// }
        /// </code>
        /// 
        /// For content of events file, check example of <c><see cref="GetCalendarItems(DateTime?, DateTime?, bool, int)"/></c>.
        /// 
        /// Output:
        /// <code>
        /// <![CDATA[
        /// Short Description    | Start Date Time    | Duration | Category Name      | Busy Time
        /// -------------------------------------------------------------------------------------
        ///                      |                    |          | Birthdays          |      1440
        /// Wendy's birthday     | 2020/Jan/12 00:00  |     1440 | Birthdays          |      5875
        /// -------------------------------------------------------------------------------------
        ///                      |                    |          | Canadian Holidays  |      1440
        /// New Year's           | 2020/Jan/01 00:00  |     1440 | Canadian Holidays  |      1555
        /// -------------------------------------------------------------------------------------
        ///                      |                    |          | Fun                |        40
        /// App Dev Homework     | 2018/Jan/10 10:00  |       40 | Fun                |        40
        /// -------------------------------------------------------------------------------------
        ///                      |                    |          | On call            |       180
        /// On call security     | 2020/Jan/20 11:00  |      180 | On call            |      6055
        /// -------------------------------------------------------------------------------------
        ///                      |                    |          | Vacation           |      2880
        /// Honolulu             | 2020/Jan/09 00:00  |     1440 | Vacation           |      2995
        /// Honolulu             | 2020/Jan/10 00:00  |     1440 | Vacation           |      4435
        /// -------------------------------------------------------------------------------------
        ///                      |                    |          | Work               |        75
        /// Sprint retrospective | 2018/Jan/11 10:15  |       60 | Work               |       100
        /// staff meeting        | 2018/Jan/11 19:30  |       15 | Work               |       115
        /// ]]>
        /// </code>
        /// </example>

        public List<CalendarItemsByCategory> GetCalendarItemsByCategory(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {

            List<CalendarItemsByCategory> itemsByCategory = new List<CalendarItemsByCategory>();
            List<string> categories = new List<string>();
            List<int> categoryIds = new List<int>();
            SQLiteDataReader r = GetRawCalendarItems(Start, End, (FilterFlag) ? CategoryID : null, null, GroupBy.Category);

            // gets all categories and id's
            while (r.Read())
            {
                categories.Add(r.GetString(1));
                categoryIds.Add(r.GetInt32(0));
            }

            int index = 0;
            Double totalBusyTime = 0;

            //getting each item by category
            foreach (int catId in categoryIds)
            {
                string txt = "SELECT CategoryId, Details, Id, StartDateTime, DurationInMinutes FROM events WHERE CategoryId = @catId AND StartDateTime >= @start AND StartDateTime <= @end ORDER BY StartDateTime";

                SQLiteCommand cmd = new SQLiteCommand(txt, Database.dbConnection);

                cmd.Parameters.AddWithValue("start", (Start ?? DEFAULT_START).ToString(Events.DATE_TIME_FORMAT));
                cmd.Parameters.AddWithValue("end", (End ?? DEFAULT_END).ToString(Events.DATE_TIME_FORMAT));
                cmd.Parameters.AddWithValue("catId", catId);
                SQLiteDataReader reader = cmd.ExecuteReader();
                List<CalendarItem> items = new List<CalendarItem>();

                string category = categories[index];
                while (reader.Read())
                {
                    Double durationInMinutes = reader.GetDouble(4);
                    totalBusyTime += durationInMinutes;
                    items.Add(new CalendarItem
                    {
                        CategoryID = reader.GetInt32(0),
                        Category = category,
                        EventID = reader.GetInt32(2),
                        StartDateTime = DateTime.ParseExact(reader.GetString(3), Events.DATE_TIME_FORMAT, CultureInfo.InvariantCulture),
                        DurationInMinutes = durationInMinutes,
                        ShortDescription = reader.GetString(1),
                        BusyTime = totalBusyTime
                    });
                }
                //adding to calendar item by category
                itemsByCategory.Add(new CalendarItemsByCategory
                {
                    Category = categories[index],
                    Items = items,
                    TotalBusyTime = totalBusyTime
                });
                index++;
            }

            return itemsByCategory;
        }



        // ============================================================================
        // Group all events by category and Month
        // creates a list of Dictionary objects with:
        //          one dictionary object per month,
        //          and one dictionary object for the category total busy times
        // 
        // Each per month dictionary object has the following key value pairs:
        //           "Month", <name of month>
        //           "TotalBusyTime", <the total durations for the month>
        //             for each category for which there is an event in the month:
        //             "items:category", a List<CalendarItem>
        //             "category", the total busy time for that category for this month
        // The one dictionary for the category total busy times has the following key value pairs:
        //             for each category for which there is an event in ANY month:
        //             "category", the total busy time for that category for all the months
        // ============================================================================

        /// <summary>
        /// Gets the list of <c>Dictionary</c> objects generated using <c><see cref="Categories"/></c> and <c><see cref="Events"/></c> of current instance, following requirements specified by given parameters.
        /// </summary>
        /// <param name="Start">Minimum StartDateTime of <c><see cref="CalendarItem"/></c> objects within <c><see cref="CalendarItemsByCategory"/></c> objects to return. (if provided)</param>
        /// <param name="End">Maximum StartDateTime of <c><see cref="CalendarItem"/></c> objects within <c><see cref="CalendarItemsByCategory"/></c> objects to return. (if provided)</param>
        /// <param name="FilterFlag">If true, the list of <c><see cref="CalendarItem"/></c> objects within <c><see cref="CalendarItemsByCategory"/></c> objects to return must have category Id of <paramref name="CategoryID"/>. If false, the list of <c><see cref="CalendarItem"/></c> objects within <c><see cref="CalendarItemsByCategory"/></c> objects to return can have any category Id. </param>
        /// <param name="CategoryID">The category Id all <c><see cref="CalendarItem"/></c> objects within <c><see cref="CalendarItemsByCategory"/></c> objects to return must have if <paramref name="FilterFlag"/> is true.</param>
        /// <returns>The list of <c>Dictionary</c> objects with <c><see cref="CalendarItem"/></c> objects having StartDateTime within interval formed by <paramref name="Start"/> and <paramref name="End"/> inclusively (if provided), having category Id of <paramref name="CategoryID"/> if <paramref name="FilterFlag"/> is true, grouped by month then cateogry, and sorted by StartDateTime in ascending order.</returns>
        /// <remarks>
        /// Inside the list to be returned, there's one <c>Dictionary</c> for each month of StartDateTime of <c><see cref="CalendarItem"/></c>.
        /// The month of each <c>Dictionary</c> can be accessed using the key <c>"Month"</c>, the monthly total busy time of each <c>Dictionary</c> can be accessed using the key <c>"TotalBusyTime"</c>.
        /// <c><see cref="CalendarItem"/></c> objects inside each dictionary is grouped by category. list of <c><see cref="CalendarItem"/></c> objects of a category in a month can be accessed via dictionary of that month by the key <c>"items:&lt;category&gt;"</c>, where <c>&lt;category&gt;</c> is the string description of category; and the total busy time of <c><see cref="CalendarItem"/></c> objects of a category in a month can be accessed via the dictionary of that month by the key <c>"&lt;category&gt;"</c>.
        /// </remarks>
        /// <example>
        /// Demonstrates usage of <c><see cref="GetCalendarDictionaryByCategoryAndMonth(DateTime?, DateTime?, bool, int)"/></c>
        /// <code>
        /// // Creates home calendar from existing database
        /// HomeCalendar homeCalendar = new HomeCalendar("testdb");
        /// 
        /// // Defines row format for event table and category table
        /// const string EVENT_ROW_FORMAT = "{0, -20} | {1, -18} | {2, 8} | {3, -18} | {4, 9}";
        /// const string CATEGORY_ROW_FORMAT = "{0, -24} | {1, 20}";
        /// 
        /// // Defines header for event table and category table
        /// string eventHeader = string.Format(EVENT_ROW_FORMAT, "Short Description", "Start Date Time", "Duration", "Category Name", "Busy Time");
        /// string categoryHeader = string.Format(CATEGORY_ROW_FORMAT, "Category", "Busy Time (Minutes)");
        /// 
        /// // Defines row separators for event table
        /// string separator1 = new string('=', eventHeader.Length);
        /// string separator2 = new string('-', eventHeader.Length);
        /// 
        /// // Gets all calendar items grouped by month and category, sorted by StartDateTime in ascending order
        /// var calendarDictionaryByCategoryAndMonth = homeCalendar.GetCalendarDictionaryByCategoryAndMonth(null, null, false, -1);
        /// 
        /// // Displays event table header
        /// Console.WriteLine(eventHeader);
        /// 
        /// // For each record (all records keep track of calendar items and other information per category per month, except for the last record, which is for total busy time per category)
        /// foreach (var record in calendarDictionaryByCategoryAndMonth)
        /// {
        ///     string month = record["Month"].ToString();
        /// 
        ///     // If iterating over months
        ///     if (month != "TOTALS")
        ///     {
        ///         // Displays event table rows
        ///         Console.WriteLine(separator1);
        /// 
        ///         // For each month displays month string and total busy time
        ///         Console.WriteLine(string.Format(EVENT_ROW_FORMAT, "", month, "", "", record["TotalBusyTime"].ToString()));
        /// 
        ///         // For each category in this month
        ///         for (int i = 2; i &lt; record.Count; i += 2)
        ///         {
        ///             var categoryRecord = record.ElementAt(i + 1);
        /// 
        ///             // Displays name and total busy time of this category in this month
        ///             Console.WriteLine(separator2);
        ///             Console.WriteLine(string.Format(EVENT_ROW_FORMAT, "", "", "", categoryRecord.Key, categoryRecord.Value));
        /// 
        ///             // Displays each calendar item in this category in this month
        ///             foreach (CalendarItem ci in (List&lt;CalendarItem&gt;)record.ElementAt(i).Value)
        ///                 Console.WriteLine(string.Format(EVENT_ROW_FORMAT, ci.ShortDescription ?? "", ci.StartDateTime.ToString("yyyy/MMM/dd HH:mm"), ci.DurationInMinutes, ci.Category ?? "", ci.BusyTime));
        ///         }
        ///     }
        ///     // If finished iterating over months
        ///     else
        ///     {
        ///         Console.WriteLine();
        /// 
        ///         // Displays category table header
        ///         Console.WriteLine(categoryHeader);
        ///         Console.WriteLine(new string('-', categoryHeader.Length));
        /// 
        ///         // For each category displays category string and total busy time
        ///         for (int i = 1; i &lt; record.Count; i++)
        ///         {
        ///             var categoryBusyTime = record.ElementAt(i);
        ///             Console.WriteLine(string.Format(CATEGORY_ROW_FORMAT, categoryBusyTime.Key, categoryBusyTime.Value));
        ///         }
        ///     }
        /// }
        /// </code>
        /// 
        /// For content of events file, check example of <c><see cref="GetCalendarItems(DateTime?, DateTime?, bool, int)"/></c>.
        /// 
        /// Output:
        /// <code>
        /// <![CDATA[
        /// Short Description    | Start Date Time    | Duration | Category Name      | Busy Time
        /// =====================================================================================
        ///                      | 2018/01            |          |                    |       115
        /// -------------------------------------------------------------------------------------
        ///                      |                    |          | Fun                |        40
        /// App Dev Homework     | 2018/Jan/10 10:00  |       40 | Fun                |        40
        /// -------------------------------------------------------------------------------------
        ///                      |                    |          | Work               |        75
        /// Sprint retrospective | 2018/Jan/11 10:15  |       60 | Work               |       100
        /// staff meeting        | 2018/Jan/11 19:30  |       15 | Work               |       115
        /// =====================================================================================
        ///                      | 2020/01            |          |                    |      5940
        /// -------------------------------------------------------------------------------------
        ///                      |                    |          | Birthdays          |      1440
        /// Wendy's birthday     | 2020/Jan/12 00:00  |     1440 | Birthdays          |      5875
        /// -------------------------------------------------------------------------------------
        ///                      |                    |          | Canadian Holidays  |      1440
        /// New Year's           | 2020/Jan/01 00:00  |     1440 | Canadian Holidays  |      1555
        /// -------------------------------------------------------------------------------------
        ///                      |                    |          | On call            |       180
        /// On call security     | 2020/Jan/20 11:00  |      180 | On call            |      6055
        /// -------------------------------------------------------------------------------------
        ///                      |                    |          | Vacation           |      2880
        /// Honolulu             | 2020/Jan/09 00:00  |     1440 | Vacation           |      2995
        /// Honolulu             | 2020/Jan/10 00:00  |     1440 | Vacation           |      4435
        /// 
        /// Category                 |  Busy Time (Minutes)
        /// -----------------------------------------------
        /// Work                     |                   75
        /// Fun                      |                   40
        /// On call                  |                  180
        /// Canadian Holidays        |                 1440
        /// Vacation                 |                 2880
        /// Birthdays                |                 1440
        /// ]]>
        /// </code>
        /// </example>

        public List<Dictionary<string, object>> GetCalendarDictionaryByCategoryAndMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items by month 
            // -----------------------------------------------------------------------
            List<CalendarItemsByMonth> GroupedByMonth = GetCalendarItemsByMonth(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // loop over each month
            // -----------------------------------------------------------------------
            var summary = new List<Dictionary<string, object>>();
            var totalBusyTimePerCategory = new Dictionary<String, Double>();

            foreach (var MonthGroup in GroupedByMonth)
            {
                // create record object for this month
                Dictionary<string, object> record = new Dictionary<string, object>();
                record["Month"] = MonthGroup.Month;
                record["TotalBusyTime"] = MonthGroup.TotalBusyTime;

                // break up the month items into categories
                var GroupedByCategory = MonthGroup.Items.GroupBy(c => c.Category);

                // -----------------------------------------------------------------------
                // loop over each category
                // -----------------------------------------------------------------------
                foreach (var CategoryGroup in GroupedByCategory.OrderBy(g => g.Key))
                {

                    // calculate totals for the cat/month, and create list of items
                    double totalCategoryBusyTimeForThisMonth = 0;
                    var details = new List<CalendarItem>();

                    foreach (var item in CategoryGroup)
                    {
                        totalCategoryBusyTimeForThisMonth = totalCategoryBusyTimeForThisMonth + item.DurationInMinutes;
                        details.Add(item);
                    }

                    // add new properties and values to our record object
                    record["items:" + CategoryGroup.Key] = details;
                    record[CategoryGroup.Key] = totalCategoryBusyTimeForThisMonth;

                    // keep track of totals for each category
                    if (totalBusyTimePerCategory.TryGetValue(CategoryGroup.Key, out Double currentTotalBusyTimeForCategory))
                    {
                        totalBusyTimePerCategory[CategoryGroup.Key] = currentTotalBusyTimeForCategory + totalCategoryBusyTimeForThisMonth;
                    }
                    else
                    {
                        totalBusyTimePerCategory[CategoryGroup.Key] = totalCategoryBusyTimeForThisMonth;
                    }
                }

                // add record to collection
                summary.Add(record);
            }
            // ---------------------------------------------------------------------------
            // add final record which is the totals for each category
            // ---------------------------------------------------------------------------
            Dictionary<string, object> totalsRecord = new Dictionary<string, object>();
            totalsRecord["Month"] = "TOTALS";

            foreach (var cat in categories.List())
            {
                try
                {
                    totalsRecord.Add(cat.Description, totalBusyTimePerCategory[cat.Description]);
                }
                catch { }
            }
            summary.Add(totalsRecord);


            return summary;
        }




        #endregion GetList

    }
}
