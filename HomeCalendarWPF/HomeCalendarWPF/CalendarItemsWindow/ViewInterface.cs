using Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalendarWPF.CalendarItems
{
    public interface ViewInterface
    {
        void DisplayFilterCategories(List<Category> categories);
        void DisplayCalendarItems(List<CalendarItem> calendarItems);
        void DisplayMonthSummary(List<CalendarItemsByMonth> calendarItemsByMonths);
        void DisplayCategorySummary(List<CalendarItemsByCategory> calendarItemsByCategories);
        void DisplayCategoryMonthSummary(List<Dictionary<string, object>> calendarItemsByCategoryAndMonth);
        void MenuItem_On();
        void MenuItem_Off();
        void UpdateSearchedItems(CalendarItem searchedCalendarItem);
        void DisplaySearchError(string error);
        void ScrollToTop();
    }
}
