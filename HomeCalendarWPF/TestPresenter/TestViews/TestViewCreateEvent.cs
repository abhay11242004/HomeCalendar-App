using Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeCalendarWPF;

namespace TestPresenter.TestViews
{
    internal class TestViewCreateEvent: HomeCalendarWPF.CreateEventWindow.ViewInterface
    {
        public Presenter presenter;

        public bool calledDisplayEvents;
        public bool calledDisplayCreated;
        public bool calledDisplayError;

        public TestViewCreateEvent(Presenter p)
        {
            presenter = p;
        }

        public void ShowError(string msg)
        {
            calledDisplayError = true;
        }
        public void EventCreated()
        {
            calledDisplayCreated = true;
        }

        public void DisplayCategories(List<Category> categories)
        {
            calledDisplayEvents = true;
        }

    }
}
