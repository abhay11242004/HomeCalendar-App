using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalendarWPF.DirectorySelection
{
    public interface PresenterInterface
    {
        void GetDirectory();
        void ProcessDirectory(string fileName, string? oldDirectory = null);
        void UseLastSavedFile();
        bool ValidLastSavedFile();
    }
}
