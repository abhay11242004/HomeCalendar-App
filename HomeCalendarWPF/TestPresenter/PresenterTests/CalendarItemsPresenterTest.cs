using Calendar;
using HomeCalendarWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Xml;

namespace TestPresenter
{
    public class CalendarItemsPresenterTest
    {
        [Fact]
        public void PresenterShowEventWindow_WindowOpened()
        {
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            presenter.GetDirectory();
            presenter.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            presenter.ShowEventWindow();

            Assert.Equal(1, view.calledShowCreateEvent);
            Assert.Equal(0, view.calledCloseCalendarItemsWindow);
        }

        [Fact]
        public void PresenterChangeOpenedFile_OldWindowClosedAndNewWindowOpened()
        {
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            view.calledShowDirectorySelectionWindow = 0;
            view.calledCloseCalendarItemsWindow = 0;
            view.calledShowCalendarItemsWindow = 0;

            presenter.ChangeOpenedFile();

            Assert.Equal(1, view.calledShowDirectorySelectionWindow);
            Assert.Equal(1, view.calledCloseCalendarItemsWindow);
            Assert.Equal(0, view.calledShowCalendarItemsWindow);
        }

        [Fact]
        public void PresenterChangeOpenedFile_DirectorySelectionWindowInitializedProperly()
        {
            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);
            TestViewDirectorySelection viewDirectorySelection = view.viewDirectorySelection;
            viewDirectorySelection.updatedDirectories.Clear();
            viewDirectorySelection.populatedFileNames.Clear();
            viewDirectorySelection.populatedFileExtensions.Clear();

            presenter.GetDirectory();
            presenter.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            presenter.ChangeOpenedFile();

            Database.CloseDatabaseAndReleaseFile();

            viewDirectorySelection = view.viewDirectorySelection;

            Assert.Single(viewDirectorySelection.updatedDirectories);
            Assert.Equal(TestConstants.DATABASE_DIRECTORY_PATH, viewDirectorySelection.updatedDirectories[0]);

            Assert.Single(viewDirectorySelection.populatedFileNames);
            Assert.Equal(TestConstants.DATABASE_DEFAULT_NAME_NO_EXTENSION, viewDirectorySelection.populatedFileNames[0]);

            Assert.Single(viewDirectorySelection.populatedFileExtensions);
            Assert.Equal(TestConstants.DATABASE_FILE_EXTENSION, viewDirectorySelection.populatedFileExtensions[0]);
        }

        [Fact]
        public void PresenterUpdateFilterStartDate_NoStartDate()
        {
            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            HomeCalendar calendar = new HomeCalendar(TestConstants.DATABASE_MESSY_PATH, false);
            List<CalendarItem> oldList = calendar.GetCalendarItems(null, null, false, -1);
            calendar.CloseDB();

            presenter.GetDirectory();
            presenter.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            view.viewCalendarItems.displayedCalendarItems.Clear();
            presenter.UpdateFilterStartDate(null);

            Database.CloseDatabaseAndReleaseFile();

            Assert.Single(view.viewCalendarItems.displayedCalendarItems);

            List<CalendarItem> newList = view.viewCalendarItems.displayedCalendarItems[0];
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
        public void PresenterUpdateFilterStartDate_WithStartDateNoExcludedItem()
        {
            DateTime start = new DateTime(1800, 1, 1);

            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            HomeCalendar calendar = new HomeCalendar(TestConstants.DATABASE_MESSY_PATH, false);
            List<CalendarItem> oldList = calendar.GetCalendarItems(start, null, false, -1);
            calendar.CloseDB();

            presenter.GetDirectory();
            presenter.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            view.viewCalendarItems.displayedCalendarItems.Clear();
            presenter.UpdateFilterStartDate(start);

            Database.CloseDatabaseAndReleaseFile();

            Assert.Single(view.viewCalendarItems.displayedCalendarItems);

            List<CalendarItem> newList = view.viewCalendarItems.displayedCalendarItems[0];
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
        public void PresenterUpdateFilterStartDate_WithStartDateWithExcludedItems()
        {
            DateTime start = new DateTime(2020, 1, 1);

            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            HomeCalendar calendar = new HomeCalendar(TestConstants.DATABASE_MESSY_PATH, false);
            List<CalendarItem> oldList = calendar.GetCalendarItems(start, null, false, -1);
            calendar.CloseDB();

            presenter.GetDirectory();
            presenter.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            view.viewCalendarItems.displayedCalendarItems.Clear();
            presenter.UpdateFilterStartDate(start);

            Database.CloseDatabaseAndReleaseFile();

            Assert.Single(view.viewCalendarItems.displayedCalendarItems);

            List<CalendarItem> newList = view.viewCalendarItems.displayedCalendarItems[0];
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
        public void PresenterUpdateFilterStartDate_WithoutIncludedItems()
        {
            DateTime start = new DateTime(2100, 1, 1);

            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            HomeCalendar calendar = new HomeCalendar(TestConstants.DATABASE_MESSY_PATH, false);
            List<CalendarItem> oldList = calendar.GetCalendarItems(start, null, false, -1);
            calendar.CloseDB();

            presenter.GetDirectory();
            presenter.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            view.viewCalendarItems.displayedCalendarItems.Clear();
            presenter.UpdateFilterStartDate(start);

            Database.CloseDatabaseAndReleaseFile();

            Assert.Single(view.viewCalendarItems.displayedCalendarItems);

            List<CalendarItem> newList = view.viewCalendarItems.displayedCalendarItems[0];
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
        public void PresenterUpdateFilterEndDate_NoEndDate()
        {
            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            HomeCalendar calendar = new HomeCalendar(TestConstants.DATABASE_MESSY_PATH, false);
            List<CalendarItem> oldList = calendar.GetCalendarItems(null, null, false, -1);
            calendar.CloseDB();

            presenter.GetDirectory();
            presenter.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            view.viewCalendarItems.displayedCalendarItems.Clear();
            presenter.UpdateFilterEndDate(null);

            Database.CloseDatabaseAndReleaseFile();

            Assert.Single(view.viewCalendarItems.displayedCalendarItems);

            List<CalendarItem> newList = view.viewCalendarItems.displayedCalendarItems[0];
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
        public void PresenterUpdateFilterEndDate_WithEndDateNoExcludedItem()
        {
            DateTime end = new DateTime(2100, 1, 1);

            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            HomeCalendar calendar = new HomeCalendar(TestConstants.DATABASE_MESSY_PATH, false);
            List<CalendarItem> oldList = calendar.GetCalendarItems(null, end, false, -1);
            calendar.CloseDB();

            presenter.GetDirectory();
            presenter.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            view.viewCalendarItems.displayedCalendarItems.Clear();
            presenter.UpdateFilterEndDate(end);

            Database.CloseDatabaseAndReleaseFile();

            Assert.Single(view.viewCalendarItems.displayedCalendarItems);

            List<CalendarItem> newList = view.viewCalendarItems.displayedCalendarItems[0];
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
        public void PresenterUpdateFilterEndDate_WithEndDateWithExcludedItems()
        {
            DateTime end = new DateTime(2020, 1, 1);

            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            HomeCalendar calendar = new HomeCalendar(TestConstants.DATABASE_MESSY_PATH, false);
            List<CalendarItem> oldList = calendar.GetCalendarItems(null, end, false, -1);
            calendar.CloseDB();

            presenter.GetDirectory();
            presenter.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            view.viewCalendarItems.displayedCalendarItems.Clear();
            presenter.UpdateFilterEndDate(end);

            Database.CloseDatabaseAndReleaseFile();

            Assert.Single(view.viewCalendarItems.displayedCalendarItems);

            List<CalendarItem> newList = view.viewCalendarItems.displayedCalendarItems[0];
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
        public void PresenterUpdateFilterEndDate_WithoutIncludedItems()
        {
            DateTime end = new DateTime(1800, 1, 1);

            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            HomeCalendar calendar = new HomeCalendar(TestConstants.DATABASE_MESSY_PATH, false);
            List<CalendarItem> oldList = calendar.GetCalendarItems(null, end, false, -1);
            calendar.CloseDB();

            presenter.GetDirectory();
            presenter.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            view.viewCalendarItems.displayedCalendarItems.Clear();
            presenter.UpdateFilterEndDate(end);

            Database.CloseDatabaseAndReleaseFile();

            Assert.Single(view.viewCalendarItems.displayedCalendarItems);

            List<CalendarItem> newList = view.viewCalendarItems.displayedCalendarItems[0];
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
        public void PresenterUpdateFilterCategory_AnyCategory()
        {
            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            HomeCalendar calendar = new HomeCalendar(TestConstants.DATABASE_MESSY_PATH, false);
            List<CalendarItem> oldList = calendar.GetCalendarItems(null, null, false, -1);
            calendar.CloseDB();

            presenter.GetDirectory();
            presenter.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            view.viewCalendarItems.displayedCalendarItems.Clear();
            presenter.UpdateFilterCategory(null);

            Database.CloseDatabaseAndReleaseFile();

            Assert.Single(view.viewCalendarItems.displayedCalendarItems);

            List<CalendarItem> newList = view.viewCalendarItems.displayedCalendarItems[0];
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
        public void PresenterUpdateFilterCategory_WithCategoryWithSelectedItem()
        {
            int categoryId = 2;

            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            HomeCalendar calendar = new HomeCalendar(TestConstants.DATABASE_MESSY_PATH, false);
            List<CalendarItem> oldList = calendar.GetCalendarItems(null, null, true, categoryId);
            calendar.CloseDB();

            presenter.GetDirectory();
            presenter.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            view.viewCalendarItems.displayedCalendarItems.Clear();
            presenter.UpdateFilterCategory(categoryId);

            Database.CloseDatabaseAndReleaseFile();

            Assert.Single(view.viewCalendarItems.displayedCalendarItems);

            List<CalendarItem> newList = view.viewCalendarItems.displayedCalendarItems[0];
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
        public void PresenterUpdateFilterCategory_WithoutSelectedItems()
        {
            int categoryId = 4;

            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            HomeCalendar calendar = new HomeCalendar(TestConstants.DATABASE_MESSY_PATH, false);
            List<CalendarItem> oldList = calendar.GetCalendarItems(null, null, true, categoryId);
            calendar.CloseDB();

            presenter.GetDirectory();
            presenter.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            view.viewCalendarItems.displayedCalendarItems.Clear();
            presenter.UpdateFilterCategory(categoryId);

            Database.CloseDatabaseAndReleaseFile();

            Assert.Single(view.viewCalendarItems.displayedCalendarItems);

            List<CalendarItem> newList = view.viewCalendarItems.displayedCalendarItems[0];
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
        public void PresenterUpdateSummaryByMonth_False()
        {
            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            HomeCalendar calendar = new HomeCalendar(TestConstants.DATABASE_MESSY_PATH, false);
            List<CalendarItem> oldList = calendar.GetCalendarItems(null, null, false, -1);
            calendar.CloseDB();

            presenter.GetDirectory();
            presenter.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            view.viewCalendarItems.displayedCalendarItems.Clear();
            presenter.UpdateSummaryByMonth(false);

            Database.CloseDatabaseAndReleaseFile();

            Assert.Single(view.viewCalendarItems.displayedCalendarItems);

            List<CalendarItem> newList = view.viewCalendarItems.displayedCalendarItems[0];
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
        public void PresenterUpdateSummaryByMonth_True()
        {
            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            HomeCalendar calendar = new HomeCalendar(TestConstants.DATABASE_MESSY_PATH, false);
            List<CalendarItemsByMonth> oldList = calendar.GetCalendarItemsByMonth(null, null, false, -1);
            calendar.CloseDB();

            presenter.GetDirectory();
            presenter.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            presenter.UpdateSummaryByMonth(true);

            Database.CloseDatabaseAndReleaseFile();

            Assert.Single(view.viewCalendarItems.displayedMonthSummary);

            List<CalendarItemsByMonth> newList = view.viewCalendarItems.displayedMonthSummary[0];
            Assert.Equal(oldList.Count, newList.Count);

            for (int i = 0; i < oldList.Count; i++)
            {
                CalendarItemsByMonth j = oldList[i];
                CalendarItemsByMonth k = newList[i];

                Assert.Equal(j.Month, k.Month);
                Assert.Equal(j.TotalBusyTime, k.TotalBusyTime);

                List<CalendarItem> a = j.Items;
                List<CalendarItem> b = k.Items;

                Assert.Equal(a.Count, b.Count);

                for (int x = 0; x < a.Count; x++)
                {
                    CalendarItem m = a[x];
                    CalendarItem n = b[x];

                    Assert.Equal(m.CategoryID, n.CategoryID);
                    Assert.Equal(m.EventID, n.EventID);
                    Assert.Equal(m.StartDateTime, n.StartDateTime);
                    Assert.Equal(m.Category, n.Category);
                    Assert.Equal(m.ShortDescription, n.ShortDescription);
                    Assert.Equal(m.DurationInMinutes, n.DurationInMinutes);
                    Assert.Equal(m.BusyTime, n.BusyTime);
                }
            }
        }

        [Fact]
        public void PresenterUpdateSummaryByCategory_False()
        {
            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            HomeCalendar calendar = new HomeCalendar(TestConstants.DATABASE_MESSY_PATH, false);
            List<CalendarItem> oldList = calendar.GetCalendarItems(null, null, false, -1);
            calendar.CloseDB();

            presenter.GetDirectory();
            presenter.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            view.viewCalendarItems.displayedCalendarItems.Clear();
            presenter.UpdateSummaryByCategory(false);

            Database.CloseDatabaseAndReleaseFile();

            Assert.Single(view.viewCalendarItems.displayedCalendarItems);

            List<CalendarItem> newList = view.viewCalendarItems.displayedCalendarItems[0];
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
        public void PresenterUpdateSummaryByCategory_True()
        {
            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            HomeCalendar calendar = new HomeCalendar(TestConstants.DATABASE_MESSY_PATH, false);
            List<CalendarItemsByCategory> oldList = calendar.GetCalendarItemsByCategory(null, null, false, -1);
            calendar.CloseDB();

            presenter.GetDirectory();
            presenter.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            presenter.UpdateSummaryByCategory(true);

            Database.CloseDatabaseAndReleaseFile();

            Assert.Single(view.viewCalendarItems.displayedCategorySummary);

            List<CalendarItemsByCategory> newList = view.viewCalendarItems.displayedCategorySummary[0];
            Assert.Equal(oldList.Count, newList.Count);

            for (int i = 0; i < oldList.Count; i++)
            {
                CalendarItemsByCategory j = oldList[i];
                CalendarItemsByCategory k = newList[i];

                Assert.Equal(j.Category, k.Category);
                Assert.Equal(j.TotalBusyTime, k.TotalBusyTime);

                List<CalendarItem> a = j.Items;
                List<CalendarItem> b = k.Items;

                Assert.Equal(a.Count, b.Count);

                for (int x = 0; x < a.Count; x++)
                {
                    CalendarItem m = a[x];
                    CalendarItem n = b[x];

                    Assert.Equal(m.CategoryID, n.CategoryID);
                    Assert.Equal(m.EventID, n.EventID);
                    Assert.Equal(m.StartDateTime, n.StartDateTime);
                    Assert.Equal(m.Category, n.Category);
                    Assert.Equal(m.ShortDescription, n.ShortDescription);
                    Assert.Equal(m.DurationInMinutes, n.DurationInMinutes);
                    Assert.Equal(m.BusyTime, n.BusyTime);
                }
            }
        }

        [Fact]
        public void PresenterUpdateSummaryByCategoryAndMonth_True()
        {
            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter presenter = new Presenter(view);

            HomeCalendar calendar = new HomeCalendar(TestConstants.DATABASE_MESSY_PATH, false);
            List<Dictionary<string, object>> oldDictList = calendar.GetCalendarDictionaryByCategoryAndMonth(null, null, false, -1);
            calendar.CloseDB();

            presenter.GetDirectory();
            presenter.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            presenter.UpdateSummaryByMonth(true);
            presenter.UpdateSummaryByCategory(true);

            Database.CloseDatabaseAndReleaseFile();

            Assert.Single(view.viewCalendarItems.displayedCategoryMonthSummary);

            List<Dictionary<string, object>> newDictList = view.viewCalendarItems.displayedCategoryMonthSummary[0];
            Assert.Equal(oldDictList.Count, newDictList.Count);

            for (int i = 0; i < oldDictList.Count; i++)
            {
                Dictionary<string, object> j = oldDictList[i];
                Dictionary<string, object> k = newDictList[i];

                Assert.Equal(j.Count, k.Count);

                for (int x = 0; x < j.Count; x++)
                {
                    KeyValuePair<string, object> a = j.ElementAt(x);
                    KeyValuePair<string, object> b = k.ElementAt(x);

                    if (a.Key.StartsWith("items:"))
                    {
                        Assert.IsType<List<CalendarItem>>(a.Value);
                        Assert.IsType<List<CalendarItem>>(b.Value);

                        List<CalendarItem> l1 = (List<CalendarItem>)a.Value;
                        List<CalendarItem> l2 = (List<CalendarItem>)b.Value;

                        Assert.Equal(l1.Count, l2.Count);

                        for (int y = 0; y < l1.Count; y++)
                        {
                            CalendarItem m = l1[y];
                            CalendarItem n = l2[y];

                            Assert.Equal(m.CategoryID, n.CategoryID);
                            Assert.Equal(m.EventID, n.EventID);
                            Assert.Equal(m.StartDateTime, n.StartDateTime);
                            Assert.Equal(m.Category, n.Category);
                            Assert.Equal(m.ShortDescription, n.ShortDescription);
                            Assert.Equal(m.DurationInMinutes, n.DurationInMinutes);
                            Assert.Equal(m.BusyTime, n.BusyTime);
                        }
                    }
                    else if (a.Key == "Month")
                    {
                        Assert.IsType<string>(a.Value);
                        Assert.IsType<string>(b.Value);

                        Assert.Equal((string)a.Value, (string)b.Value);
                    }
                    else // "TotalBusyTime" - double or "[Category]" - double
                    {
                        Assert.IsType<double>(a.Value);
                        Assert.IsType<double>(b.Value);

                        Assert.Equal((double)a.Value, (double)b.Value);
                    }
                }
            }
        }
    }
}
