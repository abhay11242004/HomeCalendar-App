using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalendarWPF.CreateCategory
{
    public interface ViewInterface
    {
        void DisplayError(string message);
        void DisplayCategoryTypes(List<string> categoryTypes);
        void DisplayCategoryIsCreated();

    }
}
