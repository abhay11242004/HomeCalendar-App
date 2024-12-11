using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;

namespace HomeCalendarWPF.UpdateEventWindow
{
    public interface ViewInterface
    {
        void ShowError(string msg);

        void EventUpdated();

        void DisplayCategories(List<Category> category);

    }
}
