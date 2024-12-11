using Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalendarWPF.CalendarItems
{
    public interface PresenterInterface
    {
        void ShowEventWindow();
        void ChangeOpenedFile();
        void UpdateFilterStartDate(DateTime? start);
        void UpdateFilterEndDate(DateTime? end);
        void UpdateFilterCategory(int? categoryId);
        void UpdateSummaryByMonth(bool summaryByMonth);
        void UpdateSummaryByCategory(bool summaryByCategory);
        void ShowUpdateEventWindow(int id);
        void DeleteEvent(int id);
        void GetSearchedCalendarItemsList(string search, List<CalendarItem> calendarItems, int selectedRowIndex);
    }
}
