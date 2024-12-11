using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalendarWPF.DirectorySelection
{
    public interface ViewInterface
    {
        void Show();
        void UpdateDirectory(string directory);
        void DisplayError(string message);
        void PopulateWindow(string fileName, string fileExtension);
    }
}
