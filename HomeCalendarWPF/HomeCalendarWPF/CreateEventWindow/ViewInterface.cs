using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;

namespace HomeCalendarWPF.CreateEventWindow
{
    public interface ViewInterface
    {
        void ShowError(string msg);

        void EventCreated();

        void DisplayCategories(List<Category> category);

    }
}
