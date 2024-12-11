using HomeCalendarWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestPresenter.TestViews;

namespace TestPresenter
{
    public class TestCreatedCateogry
    {
        [Fact]
        public void TestLoadCategoryType()
        {
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            p.ShowCategoryWindow();
            p.LoadCategoryTypes();
            Assert.True(view.viewDisplayCategory.calledDisplayCategoryTypes);
        }
        [Fact]
        public void TestValidInputCategory()
        {
            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            p.GetDirectory();
            p.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            p.ShowCategoryWindow();
            p.ShowEventWindow();

            p.CreateCategory("R", "Event");

            Assert.False(view.viewDisplayCategory.calledDisplayError);
            Assert.True(view.viewDisplayCategory.calledDisplayCategoryIsCreated);
        }
        [Fact]
        public void TestInvalidInputCategory()
        {
            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            p.GetDirectory();
            p.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            p.ShowCategoryWindow();

            p.CreateCategory("", "Event");

            Assert.True(view.viewDisplayCategory.calledDisplayError);
            Assert.False(view.viewDisplayCategory.calledDisplayCategoryIsCreated);
        }
        [Fact]
        public void TestInputSameCategory()
        {
            Directory.CreateDirectory(TestConstants.DATABASE_MESSY_DIRECTORY);
            File.Copy(TestConstants.DATABASE_CLEAN_PATH, TestConstants.DATABASE_MESSY_PATH, true);
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            p.GetDirectory();
            p.ProcessDirectory(TestConstants.DATABASE_MESSY_NAME);
            p.ShowCategoryWindow();
            p.ShowEventWindow();

            p.CreateCategory("Party", "Event");
            p.CreateCategory("Party", "Event");

            Assert.True(view.viewDisplayCategory.calledDisplayError);
        }

    }
}
