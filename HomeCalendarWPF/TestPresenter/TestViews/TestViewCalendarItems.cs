using Calendar;
using HomeCalendarWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPresenter
{
    public class TestViewCalendarItems : HomeCalendarWPF.CalendarItems.ViewInterface
    {
        public Presenter presenter;
        public List<string> displayedOpenedFileLocations = new List<string>();
        public List<List<Category>> displayedFilterCategories = new List<List<Category>>();
        public List<List<CalendarItem>> displayedCalendarItems = new List<List<CalendarItem>>();
        public List<List<Dictionary<string, object>>> displayedCategoryMonthSummary = new List<List<Dictionary<string, object>>>();
        public List<List<CalendarItemsByCategory>> displayedCategorySummary = new List<List<CalendarItemsByCategory>>();
        public List<List<CalendarItemsByMonth>> displayedMonthSummary = new List<List<CalendarItemsByMonth>>();


        public TestViewCalendarItems(Presenter presenter)
        {
            this.presenter = presenter;
        }

        public void DisplayFilterCategories(List<Category> categories)
        {
            displayedFilterCategories.Add(categories);
        }

        public void DisplayOpenedFileLocation(string location)
        {
            displayedOpenedFileLocations.Add(location);
        }

        public void DisplayCalendarItems(List<CalendarItem> calendarItems)
        {
            displayedCalendarItems.Add(calendarItems);
        }

        public void DisplayCategoryMonthSummary(List<Dictionary<string, object>> calendarItemsByCategoryAndMonth)
        {
            displayedCategoryMonthSummary.Add(calendarItemsByCategoryAndMonth);
        }

        public void DisplayCategorySummary(List<CalendarItemsByCategory> calendarItemsByCategories)
        {
            displayedCategorySummary.Add(calendarItemsByCategories);
        }

        public void DisplayMonthSummary(List<CalendarItemsByMonth> calendarItemsByMonths)
        {
            displayedMonthSummary.Add(calendarItemsByMonths);
        }

        public void MenuItem_On()
        {
            
        }

        public void MenuItem_Off()
        {
            
        }

        public void DisplaySearchError(string error)
        {

        }

        public void UpdateSearchedItems(CalendarItem calendarItem)
        {

        }
        public void ScrollToTop()
        {

        }
    }
}
