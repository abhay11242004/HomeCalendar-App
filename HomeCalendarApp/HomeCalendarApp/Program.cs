using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using static System.Console;

namespace Calendar
{
    internal class Program
    {
        const ConsoleColor ERROR_COLOR = ConsoleColor.Red;
        const ConsoleColor OKAY_COLOR = ConsoleColor.Green;
        const ConsoleColor HIGHLIGHT_FOREGROUND_COLOR = ConsoleColor.Black;
        const ConsoleColor HIGHLIGHT_BACKGROUND_COLOR = ConsoleColor.Gray;
        const ConsoleColor HIGHLIGHT_BACKGROUND_COLOR2 = ConsoleColor.DarkGray;
        const ConsoleKey CONFIRMATION_KEY = ConsoleKey.Y;
        const char SEPARATE_CHAR = '-';

        // ways of displaying calendar items
        enum DisplayMethod
        {
            ByStartDate = 1,
            ByMonth,
            ByCategory,
            ByCategoryAndMonth
        }

        static void Main(string[] args)
        {
            const string DATABASE_FILE_LOCATION = @"calendar.db";
            const int PAUSE_IN_MILLISECONDS = 750;

            WriteLine("HomeCalendar App Test\n");

            HomeCalendar homeCalendar;

            try
            {
                // try creating HomeCalendar object and reading categories and events from file

                WriteLine("Reading calendar data");

                homeCalendar = new HomeCalendar(DATABASE_FILE_LOCATION);

                WriteInColor("Calendar data read successfully\n", OKAY_COLOR);

                WriteLine("\n\nFetching parameters for displaying calendar items");

                WriteLine(@"
The program will display details of a list of calendar items.
Calendar items outside the date range formed by minimum date and maximum date will not be included.
Minimum date and maximum date are optional and can be disabled if left empty
");

                PauseWithoutReadingKeyInput(PAUSE_IN_MILLISECONDS);

                DateTime?[] dateRange = new DateTime?[2];

                do
                {
                    dateRange[0] = GetUserInput("Enter minimum date of calendar items (e.g. \"1999/12/31\"), or leave it empty to disable: ", "Invalid date format, try again: ", typeof(DateTime), true) as DateTime?;
                    dateRange[1] = GetUserInput("Enter maximum date of calendar items (e.g. \"1999/12/31\"), or leave it empty to disable: ", "Invalid date format, try again: ", typeof(DateTime), true) as DateTime?;

                    if (dateRange[0] > dateRange[1])
                    {
                        WriteInColor("Maximum date entered is smaller than minimum date.\n", ERROR_COLOR);

                        if (GetYesOrNoAnswer("Swap them and continue? (Y/N) "))
                        {
                            DateTime? temp = dateRange[0];
                            dateRange[0] = dateRange[1];
                            dateRange[1] = temp;

                            WriteInColor("Swapped, continuing . . .\n", OKAY_COLOR);

                            break;
                        }
                        else
                            WriteLine("minimum date and maximum date need to be re-entered");
                    }
                }
                while (dateRange[0] > dateRange[1]);

                List<Category> categories = homeCalendar.categories.List();

                WriteLine("\nList of categories for calendar items:\n");

                DisplayCategoryTable(categories);

                PauseWithoutReadingKeyInput(PAUSE_IN_MILLISECONDS);

                bool filterFlag;
                int categoryID;

                WriteLine("\nThe list of calendar items can be filtered to include one selected category only.");
                int? rawCategoryID;

                while (true)
                {
                    rawCategoryID = GetUserInput("Enter Id of category to be selected, or leave it empty to disable filtering and select all: ", "Only integer is accepted, try again: ", typeof(int), true) as int?;

                    if (rawCategoryID == null)
                    {
                        filterFlag = false;
                        categoryID = -1;

                        break;
                    }
                    else if (IdInCategories(categories, categoryID = rawCategoryID.Value))
                    {
                        filterFlag = true;

                        break;
                    }
                    else
                    {
                        WriteInColor("Given Id doesn't match any category, try again\n", ERROR_COLOR);
                    }
                }

                WriteLine("\nList of calendar items display method:\n");

                DisplayCalendarItemsDisplayMethod();

                DisplayMethod displayMethod;

                while (true)
                {
                    int? intDisplayMethod = GetUserInput($"Select a display method by entering its number (e.g. to select {(DisplayMethod)1}, enter 1): ", "Only integer is accepted, try again: ", typeof(int)) as int?;

                    try
                    {
                        displayMethod = (DisplayMethod)intDisplayMethod.Value;

                        break;
                    }
                    catch
                    {
                        WriteInColor("Given number doesn't match any display method, try again\n", ERROR_COLOR);
                    }
                }


                WriteInColor("\nParameters fetched successfully\n", OKAY_COLOR);

                WriteLine("\nDisplaying calendar items\n");

                DisplayCalendarItems(homeCalendar, dateRange, filterFlag, categoryID, displayMethod);

                WriteInColor("\nCalendar items displayed\n", OKAY_COLOR);
            }
            catch (Exception e)
            {
                WriteInColor($"Program crashed - {e.Message}\n", ERROR_COLOR);
            }
            finally
            {
                Write("\n\n\nPress any key to exit . . . ");

                PauseWithoutReadingKeyInput(PAUSE_IN_MILLISECONDS);

                ReadKey(true);
            }
        }

        static string GetCalendarItemHeader()
        {
            StringBuilder header = new StringBuilder(GetCalendarItemRow("Category", "Event", "Short Description", "Start Date & Time", "Duration", "Category Name", "Busy Time"));
            header.AppendLine();
            header.Append(GetCalendarItemRow("   ID   ", " ID  ", "", "", "(Minutes)", "", "(Minutes)"));

            return header.ToString();
        }

        static string GetCalendarItemRow(CalendarItem calendarItem)
        {
            return GetCalendarItemRow(calendarItem.CategoryID.ToString(), calendarItem.EventID.ToString(), calendarItem.ShortDescription ?? string.Empty, calendarItem.StartDateTime.ToString(), calendarItem.DurationInMinutes.ToString(), calendarItem.Category ?? string.Empty, calendarItem.BusyTime.ToString());
        }

        static string GetCalendarItemRow(string categoryId, string eventId, string shortDescription, string startDateTime, string durationInMinutes, string category, string busyTime)
        {
            return string.Format("{0, 8} | {1, 5} | {2, -24} | {3, -24} | {4, 9} | {5, -18} | {6, 11}", categoryId, eventId, shortDescription, startDateTime, durationInMinutes, category, busyTime);
        }

        static void DisplayCalendarItems(HomeCalendar homeCalendar, DateTime?[] dateRange, bool filterFlag, int categoryId, DisplayMethod displayMethod)
        {
            string header = GetCalendarItemHeader();
            string separator = new string(SEPARATE_CHAR, header.Length / 2);

            switch (displayMethod)
            {
                case DisplayMethod.ByStartDate:
                    List<CalendarItem> calendarItems = homeCalendar.GetCalendarItems(dateRange[0], dateRange[1], filterFlag, categoryId);

                    WriteLine(header);
                    WriteLine(separator);

                    foreach (CalendarItem calendarItem in calendarItems)
                        WriteLine(GetCalendarItemRow(calendarItem));

                    break;

                case DisplayMethod.ByMonth:
                    List<CalendarItemsByMonth> calendarItemsByMonthList = homeCalendar.GetCalendarItemsByMonth(dateRange[0], dateRange[1], filterFlag, categoryId);

                    WriteLine(header);

                    foreach (CalendarItemsByMonth calendarItemsByMonth in calendarItemsByMonthList)
                    {
                        WriteLine(separator);

                        ForegroundColor = HIGHLIGHT_FOREGROUND_COLOR;
                        BackgroundColor = HIGHLIGHT_BACKGROUND_COLOR;
                        Write(GetCalendarItemRow("", "", "", calendarItemsByMonth.Month, "", "", calendarItemsByMonth.TotalBusyTime.ToString()));
                        ResetColor();
                        WriteLine();

                        foreach (CalendarItem calendarItem in calendarItemsByMonth.Items)
                            WriteLine(GetCalendarItemRow(calendarItem));
                    }

                    break;

                case DisplayMethod.ByCategory:
                    List<CalendarItemsByCategory> calendarItemsByCategoryList = homeCalendar.GetCalendarItemsByCategory(dateRange[0], dateRange[1], filterFlag, categoryId);

                    WriteLine(header);

                    foreach (CalendarItemsByCategory calendarItemsByCategory in calendarItemsByCategoryList)
                    {
                        WriteLine(separator);

                        ForegroundColor = HIGHLIGHT_FOREGROUND_COLOR;
                        BackgroundColor = HIGHLIGHT_BACKGROUND_COLOR;
                        Write(GetCalendarItemRow("", "", "", "", "", calendarItemsByCategory.Category, calendarItemsByCategory.TotalBusyTime.ToString()));
                        ResetColor();
                        WriteLine();

                        foreach (CalendarItem calendarItem in calendarItemsByCategory.Items)
                            WriteLine(GetCalendarItemRow(calendarItem));
                    }

                    break;

                case DisplayMethod.ByCategoryAndMonth:
                    List<Dictionary<string, object>> calendarDictionaryByCategoryAndMonth = homeCalendar.GetCalendarDictionaryByCategoryAndMonth(dateRange[0], dateRange[1], filterFlag, categoryId);

                    WriteLine(header);

                    foreach (Dictionary<string, object> monthlyRecord in calendarDictionaryByCategoryAndMonth)
                    {
                        string month = monthlyRecord["Month"].ToString();

                        if (month != "TOTALS")
                        {
                            WriteLine(separator);

                            ForegroundColor = HIGHLIGHT_FOREGROUND_COLOR;
                            BackgroundColor = HIGHLIGHT_BACKGROUND_COLOR;
                            Write(GetCalendarItemRow("", "", "", month, "", "", monthlyRecord["TotalBusyTime"].ToString()));
                            ResetColor();
                            WriteLine();

                            for (int i = 2; i < monthlyRecord.Count; i += 2)
                            {
                                KeyValuePair<string, object> categoryRecord = monthlyRecord.ElementAt(i + 1);

                                ForegroundColor = HIGHLIGHT_FOREGROUND_COLOR;
                                BackgroundColor = HIGHLIGHT_BACKGROUND_COLOR2;
                                Write(GetCalendarItemRow("", "", "", "", "", categoryRecord.Key, categoryRecord.Value.ToString()));
                                ResetColor();
                                WriteLine();

                                foreach (CalendarItem calendarItem in (List<CalendarItem>)monthlyRecord.ElementAt(i).Value)
                                    WriteLine(GetCalendarItemRow(calendarItem));
                            }
                        }
                        else
                        {
                            WriteLine();

                            string smallHeader = GetCategoryTableRow("Category", "Busy Time (Minutes)");

                            WriteLine(smallHeader);
                            WriteLine(new string(SEPARATE_CHAR, smallHeader.Length));

                            for (int i = 1; i < monthlyRecord.Count; i++)
                            {
                                KeyValuePair<string, object> categoryBusyTime = monthlyRecord.ElementAt(i);
                                WriteLine(GetCategoryTableRow(categoryBusyTime.Key, categoryBusyTime.Value.ToString()));
                            }
                        }
                    }

                    break;
            }
        }

        static string GetCategoryTableRow(string description, string busyTime)
        {
            return string.Format("{0, -24} | {1, 20}", description, busyTime);
        }

        static void DisplayCalendarItemsDisplayMethod()
        {
            foreach (DisplayMethod method in Enum.GetValues<DisplayMethod>())
                WriteLine("{0}. {1}", (int)method, method);
        }

        static bool IdInCategories(List<Category> categories, int id)
        {
            foreach (Category category in categories)
            {
                if (category.Id == id)
                    return true;
            }

            return false;
        }

        static void DisplayCategoryTable(List<Category> categories)
        {
            string header = GetCategoryTableRow("Id", "Description", "Type");

            WriteLine(header);

            WriteLine(new string(SEPARATE_CHAR, header.Length));

            foreach (Category category in categories)
                WriteLine(GetCategoryTableRow(category.Id.ToString(), category.Description, category.Type.ToString()));
        }

        static string GetCategoryTableRow(string id, string description, string type)
        {
            return string.Format("{0, 4} | {1, -24} | {2, -16}", id, description, type);
        }

        static bool GetYesOrNoAnswer(string prompt)
        {
            Write(prompt);

            ConsoleKey input = ReadKey().Key;

            WriteLine();

            return input == CONFIRMATION_KEY;
        }

        static object? GetUserInput(string prompt, string error, Type outputType, bool isOptional = false)
        {
            Write(prompt);

            try
            {
                string? input = ReadLine();

                if (isOptional && input == string.Empty)
                    return null;

                return Convert.ChangeType(input, outputType);
            }
            catch
            {
                while (true)
                {
                    WriteInColor(error, ERROR_COLOR);

                    try
                    {
                        string? input = ReadLine();

                        if (isOptional && input == string.Empty)
                            return null;

                        return Convert.ChangeType(input, outputType);
                    }
                    catch { }
                }
            }
        }

        static void PauseWithoutReadingKeyInput(int pauseInMilliseconds)
        {
            Thread.Sleep(pauseInMilliseconds);

            while (KeyAvailable)
                ReadKey(true);
        }

        static void WriteInColor(string text, ConsoleColor foregroundColor, ConsoleColor? backgroundColor = null)
        {
            ForegroundColor = foregroundColor;

            if (backgroundColor != null)
                BackgroundColor = (ConsoleColor)backgroundColor;

            Write(text);

            ResetColor();
        }
    }
}
