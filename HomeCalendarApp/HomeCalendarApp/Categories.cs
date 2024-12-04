using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Data.SQLite;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.Intrinsics.Arm;
using System.Xml.Linq;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{
    // ====================================================================
    // CLASS: categories
    //        - A collection of category items,
    //        - Read / write to file
    //        - etc
    // ====================================================================

    /// <summary>
    /// A collection of <c><see cref="Category"/></c> objects for <see cref="HomeCalendar">calendar program</see>.
    /// </summary>
    /// <remarks>
    /// The list of <c><see cref="Category"/></c> objects can be read from or written to a file specified.
    /// </remarks>
    public class Categories
    {
        private SQLiteConnection _connection;

        // ====================================================================
        // Constructor
        // ====================================================================

        /// <summary>
        /// Creates a new instance of <c><see cref="Categories"/></c> using a <param name="dbConnection">.
        /// </summary>
        /// <param name="dbConnection">A connection to the database</param>
        /// <param name="isNewDB">If true, it's a new database and if false, it's not a new database.</param>
        /// <example>
        /// Demonstrates usage of <c><see cref="Categories"/></c> constructor.
        /// <code>
        /// // Creates a new categories object
        /// Categories categories = new Categories(dbConnection, isNewDB);
        ///
        /// // Prints descriptions of all default category objects
        /// foreach (Category category in categories.List())
        ///     Console.WriteLine(category);
        /// </code>
        /// </example>
        public Categories(SQLiteConnection dbConnection, bool isNewDB)
        {
            _connection = dbConnection;

            if (isNewDB)
            {
                SetCategoryTypesToDefaults();
                SetCategoriesToDefaults();
            }
        }

        // ====================================================================
        // get a specific category from the list where the id is the one specified
        // ====================================================================

        /// <summary>
        /// Gets <c><see cref="Category"/></c> object with <paramref name="i">given Id</paramref>.
        /// </summary>
        /// <param name="i">The Id of <c><see cref="Category"/></c> object to get.</param>
        /// <returns>The <c><see cref="Category"/></c> object with <paramref name="i">given Id</paramref>.</returns>
        /// <exception cref="SQLiteException">Thrown when no <c><see cref="Category"/></c> object in the list has <paramref name="i">given Id</paramref>.</exception>
        /// <example>
        /// Demonstrates usage of <c><see cref="GetCategoryFromId(int)"/></c>.
        /// <code>
        /// // Creates a new categories object
        /// Categories categories = new Categories(dbConnection, isNewDB);
        /// 
        /// // Prints description of category object with Id of 2
        /// Console.WriteLine(categories.GetCategoryFromId(2));
        /// </code>
        /// </example>
        public Category GetCategoryFromId(int i)
        {
            Category newCategory = null;

            SQLiteCommand cmd = new SQLiteCommand(_connection);

            cmd.CommandText = "SELECT Description, TypeId FROM categories WHERE Id = @i;";
            cmd.Parameters.AddWithValue("i", i);

            SQLiteDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                newCategory = (new Category(i, rdr.GetString(0), (Category.CategoryType)rdr.GetInt32(1)));
            }

            if (newCategory == null)
            {
                throw new SQLiteException("Cannot find category with id " + i.ToString());
            }

            return newCategory;
        }

        private void SetCategoryTypesToDefaults()
        {
            SQLiteCommand cmd;

            foreach (int i in Enum.GetValues(typeof(Category.CategoryType)))
            {
                cmd = new SQLiteCommand(_connection);

                cmd.CommandText = "INSERT INTO categoryTypes(Id, Description) VALUES (@id, @description)";

                cmd.Parameters.AddWithValue("id", i);
                cmd.Parameters.AddWithValue("description", ((Category.CategoryType)i).ToString());

                cmd.ExecuteNonQuery();

                cmd.Dispose();
            }
        }

        // ====================================================================
        // set categories to default
        // ====================================================================

        /// <summary>
        /// Sets the list of <c><see cref="Category"/></c> objects to default value.
        /// </summary>
        /// <example>
        /// Demonstrates usage of <c><see cref="SetCategoriesToDefaults"/></c>.
        /// <code>
        /// // Creates a new categories object
        /// Categories categories = new Categories(dbConnection, isNewDB);
        /// 
        /// // Populates list of category objects with content from default file
        /// categories.ReadFromFile(null);
        /// 
        /// // Sets list of category objects to default value
        /// categories.SetCategoriesToDefaults();
        /// </code>
        /// </example>
        public void SetCategoriesToDefaults()
        {
            // ---------------------------------------------------------------
            // reset any current categories,
            // ---------------------------------------------------------------
            SQLiteCommand cmd = new SQLiteCommand("DELETE FROM categories", _connection);

            cmd.ExecuteNonQuery();

            cmd.Dispose();

            // ---------------------------------------------------------------
            // Add Defaults
            // ---------------------------------------------------------------
            Add("School", Category.CategoryType.Event);
            Add("Personal", Category.CategoryType.Event);
            Add("VideoGames", Category.CategoryType.Event);
            Add("Medical", Category.CategoryType.Event);
            Add("Sleep", Category.CategoryType.Event);
            Add("Vacation", Category.CategoryType.AllDayEvent);
            Add("Travel days", Category.CategoryType.AllDayEvent);
            Add("Canadian Holidays", Category.CategoryType.Holiday);
            Add("US Holidays", Category.CategoryType.Holiday);
        }

        /// <summary>
        /// Adds a <c><see cref="Category"/></c> object with given parameters and auto-generated Id to the database.
        /// </summary>
        /// <param name="desc">The description of <c><see cref="Category"/></c>.</param>
        /// <param name="type">The type of <c><see cref="Category"/></c>.</param>
        /// <example>
        /// Demonstrates usage of <c><see cref="Add(string, Category.CategoryType)"/></c>.
        /// <code>
        /// // Creates a new categories object
        /// Categories categories = new Categories(dbConnection, isNewDB);
        /// 
        /// // Adds a new category object to the database
        /// categories.Add("Entertainment", Category.CategoryType.Event);
        /// </code>
        /// </example>
        public void Add(String desc, Category.CategoryType type)
        {

            SQLiteCommand cmd = new SQLiteCommand(_connection);

            cmd.CommandText = $"INSERT INTO categories(Description, TypeId) VALUES (@description, @typeId);";

            cmd.Parameters.AddWithValue("description", desc);
            cmd.Parameters.AddWithValue("typeId", (int)type);

            cmd.ExecuteNonQuery();


            cmd.Dispose();
        }

        // ====================================================================
        // Delete category
        // ====================================================================

        /// <summary>
        /// Deletes <c><see cref="Category"/></c> with <paramref name="Id"/> from the database.
        /// </summary>
        /// <param name="Id">The Id of <c><see cref="Category"/></c> object to be deleted from the database.</param>
        /// <exception cref="SQLiteException">Thrown when <paramref name="id"/> of category is not found in the database</exception>
        /// <example>
        /// Demonstrates usage of <c><see cref="Delete(int)"/></c>.
        /// <code>
        /// // Creates a new categories object
        /// Categories categories = new Categories(dbConnection, isNewDB);
        /// 
        /// // Removes category object with Id of 2 from the database
        /// categories.Delete(2);
        /// </code>
        /// </example>

        public void Delete(int id)
        {

            SQLiteCommand cmd = new SQLiteCommand(_connection);

            cmd.CommandText = "DELETE FROM events WHERE CategoryId = @id";

            cmd.Parameters.AddWithValue("id", id);

            cmd.ExecuteNonQuery();

            cmd.CommandText = "DELETE FROM categories WHERE Id = @id";

            cmd.Parameters.AddWithValue("id", id);

            if(cmd.ExecuteNonQuery() < 1)
            {
                throw new SQLiteException("Error, invalid category id");
            }

            cmd.Dispose();
        }

        // ====================================================================
        // Return list of categories
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================

        /// <summary>
        /// Retrieves categories from the database and copy it's content inside a list of <c><see cref="Category"/></c> objects.
        /// </summary>
        /// <returns>A list of <c><see cref="Category"/></c> objects </returns>
        /// <example>
        /// Demonstrates usage of <c><see cref="List()"/></c>.
        /// <code>
        /// // Creates a new categories object
        /// Categories categories = new Categories(dbConnection, isNewDB);
        ///
        /// // Prints descriptions of all category objects
        /// foreach (Category category in categories.List())
        ///     Console.WriteLine(category);
        /// </code>
        /// </example>
        public List<Category> List()
        {
            List<Category> newList = new List<Category>();

            string txt = "SELECT Id, Description, TypeId FROM categories ORDER BY Id;";

            SQLiteCommand cmd = new SQLiteCommand(txt, _connection);

            SQLiteDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
                newList.Add(new Category(rdr.GetInt32(0), rdr.GetString(1), (Category.CategoryType)rdr.GetInt32(2)));

            return newList;
        }

        /// <summary>
        /// Updates a <c><see cref="Category"/></c> stored in database with given properties.
        /// </summary>
        /// <param name="id">The Id of <c><see cref="Category"/></c> to update in database.</param>
        /// <param name="description">The new description of <c><see cref="Category"/></c> to update.</param>
        /// <param name="type">The new type of <c><see cref="Category"/></c> to update.</param>
        /// <exception cref="SQLiteException">Thrown when <paramref name="id"/> of category is not found in the database</exception>
        public void UpdateProperties(int id, string description, Category.CategoryType type)
        {
            SQLiteCommand cmd = new SQLiteCommand(_connection);

            cmd.CommandText = "UPDATE categories SET Description = @description, TypeId = @typeId WHERE Id = @id";

            cmd.Parameters.AddWithValue("description", description);
            cmd.Parameters.AddWithValue("typeId", (int)type);
            cmd.Parameters.AddWithValue("id", id);

            if(cmd.ExecuteNonQuery() < 1)
            {
                throw new SQLiteException("Error, invalid category id");
            }

            cmd.Dispose();

        }
    }
}

