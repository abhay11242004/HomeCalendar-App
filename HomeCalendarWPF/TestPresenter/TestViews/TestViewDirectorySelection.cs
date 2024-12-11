using HomeCalendarWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPresenter
{
    public class TestViewDirectorySelection : HomeCalendarWPF.DirectorySelection.ViewInterface
    {
        public Presenter presenter;

        public int calledShow = 0;
        public List<string> errorMessages = new List<string>();
        public List<string> populatedFileNames = new List<string>();
        public List<string> populatedFileExtensions = new List<string>();
        public List<string> updatedDirectories = new List<string>();

        public TestViewDirectorySelection(Presenter presenter)
        {
            this.presenter = presenter;
        }

        public void Show()
        {
            calledShow++;
        }

        public void DisplayError(string message)
        {
            errorMessages.Add(message);
        }

        public void PopulateWindow(string fileName, string fileExtension)
        {
            populatedFileNames.Add(fileName);
            populatedFileExtensions.Add(fileExtension);
        }

        public void UpdateDirectory(string directory)
        {
            updatedDirectories.Add(directory);
        }
    }
}
