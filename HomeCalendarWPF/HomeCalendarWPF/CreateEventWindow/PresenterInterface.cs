using Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalendarWPF.CreateEventWindow
{
    public interface PresenterInterface
    {
        void GetCategoryList();
        void CreateEvent(string startDate, string duration, Category category, string details);
        void ShowCategoryWindow();

    }
}
