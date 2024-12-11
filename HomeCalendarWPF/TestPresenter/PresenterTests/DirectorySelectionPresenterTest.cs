using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;
using HomeCalendarWPF;
using HomeCalendarWPF.DirectorySelection;
using IniParser;

namespace TestPresenter
{
    public class DirectorySelectionPresenterTest
    {
        [Fact]
        public void PresenterConstructor_UpdateDirectoryCorrectly()
        {
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            Assert.Single(view.viewDirectorySelection.updatedDirectories);
            Assert.Equal(TestConstants.DATABASE_DEFAULT_DIRECTORY_LOCATION, view.viewDirectorySelection.updatedDirectories[0]);
        }

        [Fact]
        public void PresenterConstructor_PopulateWindowCorrectly()
        {
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            TestViewDirectorySelection viewDirectorySelection = view.viewDirectorySelection;

            Assert.Single(viewDirectorySelection.populatedFileNames);
            Assert.Single(viewDirectorySelection.populatedFileExtensions);
            Assert.Equal(TestConstants.DATABASE_DEFAULT_NAME_NO_EXTENSION, viewDirectorySelection.populatedFileNames[0]);
            Assert.Equal(TestConstants.DATABASE_FILE_EXTENSION, viewDirectorySelection.populatedFileExtensions[0]);
        }

        [Fact]
        public void PresenterGetDirectory_CorrectDefaultDirectory()
        {
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            presenter.GetDirectory();

            Assert.Single(view.initialDirectories);
            Assert.Equal(TestConstants.DATABASE_DEFAULT_DIRECTORY_LOCATION, view.initialDirectories[0]);
        }

        [Fact]
        public void PresenterGetDirectory_DirectoryUpdatedCorrectly()
        {
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            presenter.GetDirectory();
            presenter.GetDirectory();

            Assert.Equal(2, view.initialDirectories.Count);
            Assert.Equal(TestConstants.DATABASE_DIRECTORY_PATH, view.initialDirectories[1]);
        }

        [Fact]
        public void PresenterGetDirectory_DirectoryUpdated()
        {
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);
            TestViewDirectorySelection viewDirectorySelection = view.viewDirectorySelection;
            viewDirectorySelection.updatedDirectories.Clear();

            presenter.GetDirectory();

            Assert.Single(viewDirectorySelection.updatedDirectories);
            Assert.Equal(TestConstants.DATABASE_DIRECTORY_PATH, viewDirectorySelection.updatedDirectories[0]);
        }

        [Fact]
        public void PresenterProcessDirectory_InvalidFileNameErrorShown()
        {
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            presenter.ProcessDirectory("/xyz");

            Assert.Single(view.viewDirectorySelection.errorMessages);
            Assert.Contains("invalid", view.viewDirectorySelection.errorMessages[0].ToLower());
        }

        [Fact]
        public void PresenterProcessDirectory_CreatesNewDatabase()
        {
            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            presenter.GetDirectory();
            presenter.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);

            Assert.True(File.Exists(TestConstants.DATABASE_MESSY_PATH));
        }

        [Fact]
        public void PresenterProcessDirectory_MaintainsExistingDatabase()
        {
            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            HomeCalendar calendar = new HomeCalendar(TestConstants.DATABASE_MESSY_PATH, false);
            List<CalendarItem> oldList = calendar.GetCalendarItems(null, null, false, -1);
            calendar.CloseDB();

            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            presenter.GetDirectory();
            presenter.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            Database.CloseDatabaseAndReleaseFile();

            calendar = new HomeCalendar(TestConstants.DATABASE_MESSY_PATH, false);
            List<CalendarItem> newList = calendar.GetCalendarItems(null, null, false, -1);

            Assert.Equal(oldList.Count, newList.Count);

            for (int i = 0; i < oldList.Count; i++)
            {
                CalendarItem j = oldList[i];
                CalendarItem k = newList[i];
                Assert.Equal(j.CategoryID, k.CategoryID);
                Assert.Equal(j.EventID, k.EventID);
                Assert.Equal(j.StartDateTime, k.StartDateTime);
                Assert.Equal(j.Category, k.Category);
                Assert.Equal(j.ShortDescription, k.ShortDescription);
                Assert.Equal(j.DurationInMinutes, k.DurationInMinutes);
                Assert.Equal(j.BusyTime, k.BusyTime);
            }
        }

        [Fact]
        public void PresenterSavePreviousFile_FileIsSaved()
        {
            TestView view = new TestView();

            Presenter presenter = new Presenter(view);

            var parser = new FileIniDataParser();
            string fileName = "test_filename";

            presenter.SaveLastUsedFile(fileName);


            var newData = parser.ReadFile("config.ini");
            Assert.Equal(fileName, newData["Settings"]["FileName"]);
        }

        [Fact]
        public void PresenterSavePreviousFile_SavedFileExists()
        {
            TestView view = new TestView();

            Presenter presenter = new Presenter(view);
            string fileName = "test_filename";
            presenter.SaveLastUsedFile(fileName);
            bool isValid = presenter.ValidLastSavedFile();

            Assert.True(isValid);
        }

    }
}
