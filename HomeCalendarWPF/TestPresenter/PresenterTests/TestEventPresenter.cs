using HomeCalendarWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;
using System.Windows.Forms;
using TestPresenter.TestViews;
using IniParser;


namespace TestPresenter
{
    public class TestCreatedEvent
    {

        [Fact]
        public void TestConstructor()
        {
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            TestViewCreateEvent viewEvent = new TestViewCreateEvent(p);
            Assert.IsType<TestViewCreateEvent>(viewEvent);
        }
        [Fact]
        public void TestLoadCategories()
        {
            List<Category> categories = new List<Category>();
            categories.Add(new Category(59,"new",Category.CategoryType.Event));
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            TestViewCreateEvent viewEvent = new TestViewCreateEvent(p);
            viewEvent.DisplayCategories(categories);
            Assert.True(viewEvent.calledDisplayEvents);
        }
        [Fact]
        public void TestValidInputEvent()
        {
            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            p.GetDirectory();
            p.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            p.ShowEventWindow();

            Category category = new Category(1,"rrrr", Category.CategoryType.Event);
            
            try
            {
                p.CreateEvent("12,12,2024", "2000",category , "new");
                Assert.False(view.viewDisplayEvent.calledDisplayError);
                Assert.True(view.viewDisplayEvent.calledDisplayCreated);
            }
            catch
            {
                
                Assert.Fail("Invalid input");
            }

        }
        [Fact]
        public void TestInvalidInputEvent()
        {
            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            p.GetDirectory();
            p.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            p.ShowEventWindow();

            try
            {
                p.CreateEvent("12,12,2024", "2000r", new Category(19, "rrrr", Category.CategoryType.Event), "new");
                Assert.Fail("Invalid input");
            }
            catch(Exception ex)
            {
                Assert.True(view.viewDisplayEvent.calledDisplayError);
                Assert.False(view.viewDisplayEvent.calledDisplayCreated);
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
