using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;

namespace HomeCalendarWPF
{
    public interface ViewInterface
    {
        DirectorySelection.ViewInterface ShowDirectorySelectionWindow(Presenter presenter);
        void CloseDirectorySelectionWindow();
        CalendarItems.ViewInterface ShowCalendarItemsWindow(Presenter presenter, string directory);
        void CloseCalendarItemsWindow();

        string GetDirectory(string initialDirectory);
        CreateCategory.ViewInterface ShowCreateCategoryWindow(Presenter presenter);

        CreateEventWindow.ViewInterface ShowCreateEventWindow(Presenter presenter);
        UpdateEventWindow.ViewInterface ShowUpdateEventWindow(Presenter presenter, Event @event);
    }
}
