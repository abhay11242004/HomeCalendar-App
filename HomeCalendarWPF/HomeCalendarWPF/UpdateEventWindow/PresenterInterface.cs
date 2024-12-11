using Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalendarWPF.UpdateEventWindow
{
    public interface PresenterInterface
    {
        void GetCategoryList();
        void UpdateEvent(int id, string startDate, string duration, Category category, string details);
        void DeleteEvent(int id);
        void ShowUpdateEventWindow(int id);
        void ShowCategoryWindow();

    }
}
