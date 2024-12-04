using System;
using Xunit;
using System.IO;
using System.Collections.Generic;
using Calendar;
using System.Data.SQLite;
using System.Data.Common;

namespace CalendarCodeTests
{
    public class TestHomeCalendar
    {
        string testInputFile = TestConstants.testCalendarFile;


        // ========================================================================

        [Fact]
        public void HomeCalendarObject_ExistingDatabase()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            String goodDB = $"{dir}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{dir}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            int numEvents = TestConstants.numberOfEventsInFile;
            int numCategories = TestConstants.numberOfCategoriesInFile;


            // Act
            HomeCalendar homeCalendar = new HomeCalendar(messyDB);

            // Assert 
            Assert.IsType<HomeCalendar>(homeCalendar);



            Assert.Equal(numEvents, homeCalendar.events.List().Count());
            Assert.Equal(numCategories, homeCalendar.categories.List().Count());
        }

        // ========================================================================

        [Fact]
        public void HomeBudgeMethod_ReadFromDatabase_ReadsCorrectData()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            String goodDB = $"{dir}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{dir}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            int numEvents = TestConstants.numberOfEventsInFile;
            int numCategories = TestConstants.numberOfCategoriesInFile;


            // Act
            HomeCalendar homeCalendar = new HomeCalendar(messyDB);

            // Assert 
            Assert.IsType<HomeCalendar>(homeCalendar);



            Assert.Equal(numEvents, homeCalendar.events.List().Count());
            Assert.Equal(numCategories, homeCalendar.categories.List().Count());
        }


        // ========================================================================

        [Fact]
        public void HomeCalendarObject_NewDatabase()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            String newDB = $"{dir}\\messy.db";
            int defEvents = TestConstants.defaultEvents;
            int defCategories = TestConstants.defaultCategories;
            int defCategoriesTypes = TestConstants.defaultCategoriesTypes;
            string[] defCategoriesTypesDescription = TestConstants.defaultCategoriesTypesDescription;
            List<string> listCategoriesType = TestConstants.listCategoryType;
            List<string> newList = new List<string>();
            List<string> tableList = new List<string>();

            // Act
            HomeCalendar homeCalendar = new HomeCalendar(newDB, true);
            SQLiteCommand cmd = new SQLiteCommand("SELECT Description From categoryTypes", Database.dbConnection);

            SQLiteDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
                newList.Add(rdr.GetString(0));
            cmd.Dispose();

            cmd = new SQLiteCommand("SELECT name FROM sqlite_schema WHERE type='table' AND name is not 'sqlite_sequence';", Database.dbConnection);

            rdr = cmd.ExecuteReader();
            while (rdr.Read())
                tableList.Add(rdr.GetString(0));
            cmd.Dispose();

            // Assert 
            Assert.IsType<HomeCalendar>(homeCalendar);

            for (int i = 0; i < newList.Count; i++)
            {
                Assert.Equal(defCategoriesTypesDescription[i], newList[i]);
            }
            Assert.Equal(tableList, listCategoriesType);
            Assert.Equal(defEvents, homeCalendar.events.List().Count());
            Assert.Equal(defCategories, homeCalendar.categories.List().Count());
        }
    }
}

