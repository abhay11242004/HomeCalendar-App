using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalendarWPF.CreateCategory
{
    public interface PresenterInterface
    {
        void LoadCategoryTypes();
        void CreateCategory(string title, string categoryType);
    }
}
