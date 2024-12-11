using Calendar;
using HomeCalendarWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestPresenter.TestViews;

namespace TestPresenter
{
    internal class TestView : ViewInterface
    {
        public TestViewDirectorySelection? viewDirectorySelection;
        public TestViewCalendarItems? viewCalendarItems;
        public TestViewCreateCategory? viewDisplayCategory;
        public TestViewCreateEvent? viewDisplayEvent;

        public int calledCloseCalendarItemsWindow = 0;
        public int calledCloseDirectorySelecionWindow = 0;
        public int calledShowDirectorySelectionWindow = 0;
        public int calledShowCalendarItemsWindow = 0;
        public int calledShowCreateCategory = 0;
        public int calledShowCreateEvent = 0;
        public List<string> initialDirectories = new List<string>();

        public void CloseCalendarItemsWindow()
        {
            calledCloseCalendarItemsWindow++;
        }

        public void CloseDirectorySelectionWindow()
        {
            calledCloseDirectorySelecionWindow++;
        }

        public string GetDirectory(string initialDirectory)
        {
            initialDirectories.Add(initialDirectory);

            return TestConstants.DATABASE_DIRECTORY_PATH;
        }

        public HomeCalendarWPF.CalendarItems.ViewInterface ShowCalendarItemsWindow(Presenter presenter, string directory)
        {
            calledShowCalendarItemsWindow++;

            viewCalendarItems = new TestViewCalendarItems(presenter);
            return viewCalendarItems;
        }

        public HomeCalendarWPF.CreateCategory.ViewInterface ShowCreateCategoryWindow(Presenter presenter)
        {
            calledShowCreateCategory++;

            viewDisplayCategory = new TestViewCreateCategory(presenter);
            return viewDisplayCategory;
        }

        public HomeCalendarWPF.CreateEventWindow.ViewInterface ShowCreateEventWindow(Presenter presenter)
        {
            calledShowCreateEvent++;
            viewDisplayEvent = new TestViewCreateEvent(presenter);
            return viewDisplayEvent;
        }

        public HomeCalendarWPF.DirectorySelection.ViewInterface ShowDirectorySelectionWindow(Presenter presenter)
        {
            calledShowDirectorySelectionWindow++;

            viewDirectorySelection = new TestViewDirectorySelection(presenter);
            return viewDirectorySelection;
        }

        public HomeCalendarWPF.UpdateEventWindow.ViewInterface ShowUpdateEventWindow(Presenter presenter, Event @event)
        {
            throw new NotImplementedException();
        }
    }
}
