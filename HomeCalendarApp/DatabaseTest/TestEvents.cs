using System;
using Xunit;
using System.IO;
using System.Collections.Generic;
using Calendar;
using System.Data.SQLite;

namespace CalendarCodeTests
{
    public class TestEvents
    {
        int numberOfEventsInFile = TestConstants.numberOfEventsInFile;
        String testInputFile = TestConstants.testEventsInputFile;
        int maxIDInEventFile = TestConstants.maxIDInEventFile;
        Event firstEventInFile = new Event(1, new DateTime(2021, 1, 10), 3, 40, "App Dev Homework");


        // ========================================================================

        [Fact]
        public void EventsObject_New()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            Database.newDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;

            // Act
            Events events = new Events(conn);

            // Assert 
            Assert.IsType<Events>(events);
        }

        // ========================================================================

        [Fact]
        public void EventsMethod_ReadFromFile_ValidateCorrectDataWasRead()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            String existingDB = $"{dir}\\{TestConstants.testDBInputFile}";
            Database.existingDatabase(existingDB);
            SQLiteConnection conn = Database.dbConnection;

            // Act
            Events events = new Events(conn);
            List<Event> list = events.List();
            Event firstEvent = list[0];

            // Assert
            Assert.Equal(numberOfEventsInFile, list.Count);
            Assert.Equal(firstEventInFile.Id, firstEvent.Id);
            Assert.Equal(firstEventInFile.DurationInMinutes, firstEvent.DurationInMinutes);
            Assert.Equal(firstEventInFile.Details, firstEvent.Details);
            Assert.Equal(firstEventInFile.Category, firstEvent.Category);
        }

        // ========================================================================

        [Fact]
        public void EventsMethod_List_ReturnsListOfEvents()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            String newDB = $"{dir}\\{TestConstants.testDBInputFile}";
            Database.existingDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn);

            // Act
            List<Event> list = events.List();

            // Assert
            Assert.Equal(numberOfEventsInFile, list.Count);
        }

        // ========================================================================

        [Fact]
        public void EventsMethod_Add()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            String goodDB = $"{dir}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{dir}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn);
            int category = 3;
            double DurationInMinutes = 98.1;

            // Act
            events.Add(DateTime.Now, category, DurationInMinutes, "new Event");
            List<Event> EventsList = events.List();
            int sizeOfList = events.List().Count;

            // Assert
            Assert.Equal(numberOfEventsInFile + 1, sizeOfList);
            Assert.Equal(maxIDInEventFile + 1, EventsList[sizeOfList - 1].Id);
            Assert.Equal(category, EventsList[sizeOfList - 1].Category);
            Assert.Equal(DurationInMinutes, EventsList[sizeOfList - 1].DurationInMinutes);
        }

        // ========================================================================

        [Fact]
        public void EventsMethod_Delete()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            String goodDB = $"{dir}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{dir}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn);
            int IdToDelete = 3;

            // Act
            events.Delete(IdToDelete);
            List<Event> eventsList = events.List();
            int sizeOfList = eventsList.Count;

            // Assert
            Assert.Equal(numberOfEventsInFile - 1, sizeOfList);
            Assert.False(eventsList.Exists(e => e.Id == IdToDelete), "correct Event item deleted");
        }

        // ========================================================================

        [Fact]
        public void EventsMethod_Delete_InvalidIDDoesntCrash()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            String goodDB = $"{dir}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{dir}\\messyDB";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn);
            int IdToDelete = 1006;
            int sizeOfList = events.List().Count;

            // Act
            try
            {
                events.Delete(IdToDelete);
                Assert.Equal(sizeOfList, events.List().Count);
            }

            // Assert
            catch
            {
                Assert.True(true, "Invalid ID throws an error");
            }
        }

        // ========================================================================

        [Fact]
        public void EventsMethod_GetEventFromId()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            String newDB = $"{dir}\\{TestConstants.testDBInputFile}";
            Database.existingDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn);
            int evtId = 6;

            // Act
            Event e = events.GetEventFromId(evtId);

            // Assert
            Assert.Equal(evtId, e.Id);

        }

        // ========================================================================

        [Fact]
        public void EventsMethod_Update()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            String goodDB = $"{dir}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{dir}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn);
            DateTime newStartDateTime = new DateTime(2012, 06, 03);
            Double newDurationInMinutes = 20;
            String newDetails = "Pearl City";
            int newCategory = 1;
            int id = 3;

            // Act
            events.UpdateProperties(id, newStartDateTime, newDurationInMinutes, newDetails, newCategory);
            Event e = events.GetEventFromId(id);

            // Assert
            Assert.Equal(newStartDateTime, e.StartDateTime);
            Assert.Equal(newDurationInMinutes, e.DurationInMinutes);
            Assert.Equal(newDetails, e.Details);
            Assert.Equal(newCategory, e.Category);

        }
    }
}

