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
    // CLASS: CalendarItem
    //        A single calendar item, includes a Category and an Event
    // ====================================================================

    /// <summary>
    /// Represents a calendar item with <c><see cref="Category"/></c> and <c><see cref="Event"/></c>.
    /// </summary>
    public class CalendarItem
    {
        /// <summary>
        /// Gets or sets the category Id of current instance.
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// Gets or sets the event Id of current instance.
        /// </summary>
        public int EventID { get; set; }

        /// <summary>
        /// Gets or sets the start date and time of current instance.
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the category description of current instance.
        /// </summary>
        public String? Category { get; set; }

        /// <summary>
        /// Gets or sets the event details of current instance.
        /// </summary>
        public String? ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the duration of event of current instance measured in minutes.
        /// </summary>
        public Double DurationInMinutes { get; set; }

        /// <summary>
        /// Gets or sets the total <c><see cref="DurationInMinutes"/></c> of selected <c><see cref="CalendarItem"/></c> objects that started before or at the same time as current instance (self included).
        /// </summary>
        public Double BusyTime { get; set; }

    }

    /// <summary>
    /// Represents a group of <c><see cref="CalendarItem"/></c> objects that started in the same month with <c><see cref="Month"/></c> and <c><see cref="TotalBusyTime"/></c>.
    /// </summary>
    public class CalendarItemsByMonth
    {
        /// <summary>
        /// Gets or sets the start month of all <c><see cref="CalendarItem"/></c> objects in the list of current instance.
        /// </summary>
        public String? Month { get; set; }

        /// <summary>
        /// Gets or sets the list of <c><see cref="CalendarItem"/></c> objects that started in this month.
        /// </summary>
        public List<CalendarItem>? Items { get; set; }

        /// <summary>
        /// Gets or sets the sum of busy time of all <c><see cref="CalendarItem"/></c> objects in the list of current instance.
        /// </summary>
        public Double TotalBusyTime { get; set; }
    }

    /// <summary>
    /// Represents a group of <c><see cref="CalendarItem"/></c> objects of the same category with <c><see cref="Category"/></c> and <c><see cref="TotalBusyTime"/></c>.
    /// </summary>
    public class CalendarItemsByCategory
    {
        /// <summary>
        /// Gets or sets the category of all <c><see cref="CalendarItem"/></c> objects in the list of current instance.
        /// </summary>
        public String? Category { get; set; }

        /// <summary>
        /// Gets or sets the list of <c><see cref="CalendarItem"/></c> objects of this category.
        /// </summary>
        public List<CalendarItem>? Items { get; set; }

        /// <summary>
        /// Gets or sets the sum of busy time of all <c><see cref="CalendarItem"/></c> objects in the list of current instance.
        /// </summary>
        public Double TotalBusyTime { get; set; }

    }


}
