using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{
    // ====================================================================
    // CLASS: Category
    //        - An individual category for Calendar program
    //        - Valid category types: Event,AllDayEvent,Holiday
    // ====================================================================

    /// <summary>
    /// Represents an individual category for <see cref="HomeCalendar">calendar program</see>.
    /// </summary>
    /// <remarks>
    /// Valid <c><see cref="CategoryType"/></c>: Event, AllDayEvent, Holiday.
    /// </remarks>
    public class Category
    {
        // ====================================================================
        // Properties
        // ====================================================================

        /// <summary>
        /// Gets the Id of current instance.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the description of current instance.
        /// </summary>
        public String Description { get; }

        /// <summary>
        /// Gets the type of current instance.
        /// </summary>
        /// <remarks>
        /// Valid <c><see cref="CategoryType"/></c>: Event, AllDayEvent, Holiday.
        /// </remarks>
        public CategoryType Type { get; }

        /// <summary>
        /// Provides types for categories.
        /// </summary>
        public enum CategoryType
        {
            /// <summary>
            /// Represents a regular event
            /// </summary>
            Event = 1,

            /// <summary>
            /// Represents an all-day event
            /// </summary>
            AllDayEvent,

            /// <summary>
            /// Represents a holiday event
            /// </summary>
            Holiday,

            /// <summary>
            /// Represents an availability event
            /// </summary>
            Availability,
        };

        // ====================================================================
        // Constructor
        // ====================================================================

        /// <summary>
        /// Creates a new instance of <c><see cref="Category"/></c> with given parameters.
        /// </summary>
        /// <param name="id"><see cref="Id">The Category Id.</see></param>
        /// <param name="description"><see cref="Description">The description.</see></param>
        /// <param name="type"><see cref="Type">The Category type.</see></param>
        /// <example>
        /// Demonstrates usage of <c><see cref="Category"/></c> constructor.
        /// <code>
        /// // Creates a new category
        /// Category c = new Category(1, "Entertainment", Category.CategoryType.Event);
        /// 
        /// // Creates a list of categories including the category object
        /// Category[] list = new Category[] { c };
        /// </code>
        /// </example>
        public Category(int id, String description, CategoryType type = CategoryType.Event)
        {
            this.Id = id;
            this.Description = description;
            this.Type = type;
        }

        // ====================================================================
        // Copy Constructor
        // ====================================================================

        /// <summary>
        /// Creates a deep copy of given instance of <c><see cref="Category"/></c>
        /// </summary>
        /// <param name="category">The instance of <c><see cref="Category"/> to make copy of.</c></param>
        /// <example>
        /// Demonstrates usage of <c><see cref="Category"/></c> constructor for creating deep copy.
        /// <code>
        /// // Creates a new category
        /// Category c = new Category(1, "Entertainment", Category.CategoryType.Event);
        /// 
        /// // Creates a copy of the new category
        /// Category c2 = new Category(c);
        /// </code>
        /// </example>
        public Category(Category category)
        {
            this.Id = category.Id;
            this.Description = category.Description;
            this.Type = category.Type;
        }
        // ====================================================================
        // String version of object
        // ====================================================================

        /// <summary>
        /// Gets <c><see cref="Description"/></c> of current instance.
        /// </summary>
        /// <returns><see cref="Description">The description.</see></returns>
        /// <example>
        /// Demonstrates usage of <c><see cref="ToString"/></c>.
        /// <code>
        /// // Creates a new category
        /// Category c = new Category(1, "Entertainment", Category.CategoryType.Event);
        /// 
        /// // Prints the string representation / description of the category
        /// Console.WriteLine(c.ToString());
        /// </code>
        /// </example>
        public override string ToString()
        {
            return Description;
        }

    }
}

