using HomeCalendarWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPresenter.TestViews
{
    public class TestViewCreateCategory : HomeCalendarWPF.CreateCategory.ViewInterface
    {
        public Presenter presenter;

        public bool calledDisplayCategoryTypes;
        public bool calledDisplayCategoryIsCreated;
        public bool calledDisplayError;

        public TestViewCreateCategory(Presenter presenter)
        {
            this.presenter = presenter;
        }

        public void DisplayCategoryIsCreated()
        {
            calledDisplayCategoryIsCreated = true;
        }

        public void DisplayCategoryTypes(List<string> categoryTypes)
        {
            calledDisplayCategoryTypes = true;
        }

        public void DisplayError(string message)
        {
            calledDisplayError = true;
        }
    }
}
