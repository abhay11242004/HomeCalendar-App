using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{
    // ====================================================================
    // CLASS: Event
    //        - An individual event for calendar program
    // ====================================================================

    /// <summary>
    /// Represents an individual event for <see cref="HomeCalendar">calendar program</see>.
    /// </summary>
    public class Event
    {
        // ====================================================================
        // Properties
        // ====================================================================

        /// <summary>
        /// Gets the Id of current instance.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the start date and time of current instance.
        /// </summary>
        public DateTime StartDateTime { get; }

        /// <summary>
        /// Gets the duration of current instance measured in minutes.
        /// </summary>
        public Double DurationInMinutes { get; }

        /// <summary>
        /// Gets the details of current instance.
        /// </summary>
        public String Details { get; }

        /// <summary>
        /// Gets the category Id of current instance.
        /// </summary>
        public int Category { get; }

        // ====================================================================
        // Constructor
        //    NB: there is no verification the event category exists in the
        //        categories object
        // ====================================================================

        /// <summary>
        /// Creates a new instance of <c><see cref="Event"/></c> with given parameters.
        /// </summary>
        /// <param name="id"><see cref="Id">The Event Id.</see></param>
        /// <param name="date"><see cref="StartDateTime">The start date and time.</see></param>
        /// <param name="category"><see cref="Category">The category Id.</see></param>
        /// <param name="duration"><see cref="DurationInMinutes">The duration measured in minutes.</see></param>
        /// <param name="details"><see cref="Details">The details.</see></param>
        /// <example>
        /// Demonstrates usage of <c><see cref="Event"/></c> constructor.
        /// <code>
        /// // Creates a new event
        /// Event e = new Event(1, DateTime.Now, 6, 60, "Cleaning");
        /// 
        /// // Creates a collection of events including the event object
        /// Events evts = new Events();
        /// evts.Add(e);
        /// 
        /// // Saves the collection of events to file
        /// evts.SaveToFile();
        /// </code>
        /// </example>
        public Event(int id, DateTime date, int category, Double duration, String details)
        {
            this.Id = id;
            this.StartDateTime = date;
            this.Category = category;
            this.DurationInMinutes = duration;
            this.Details = details;
        }

        // ====================================================================
        // Copy constructor - does a deep copy
        // ====================================================================

        /// <summary>
        /// Creates a deep copy of given instance of <c><see cref="Event"/></c>.
        /// </summary>
        /// <param name="obj">The instance of <c><see cref="Event"/></c> to make copy of.</param>
        /// <example>
        /// Demonstrates usage of <c><see cref="Event"/></c> constructor for creating deep copy.
        /// <code>
        /// // Creates a new event
        /// Event e1 = new Event(1, DateTime.Now, 6, 60, "Sweeping");
        /// 
        /// // Creates a copy of the new event
        /// Event e2 = new Event(e1);
        /// 
        /// // Updates details of the new event
        /// e2.Details = "Mopping";
        /// 
        /// // Creates a collection of events including both event objects
        /// Events evts = new Events();
        /// evts.Add(e1);
        /// evts.Add(e2);
        /// 
        /// // Saves the collection of events to file
        /// evts.SaveToFile();
        /// </code>
        /// </example>
        public Event(Event obj)
        {
            this.Id = obj.Id;
            this.StartDateTime = obj.StartDateTime;
            this.Category = obj.Category;
            this.DurationInMinutes = obj.DurationInMinutes;
            this.Details = obj.Details;

        }
    }
}
