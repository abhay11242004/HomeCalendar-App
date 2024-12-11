using Calendar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Configuration;
using IniParser.Model;
using IniParser;
using System.Data.Entity.Infrastructure;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Windows;
using System.Windows.Controls.Primitives;
using CalendarItem = Calendar.CalendarItem;
using System.Windows.Controls;

namespace HomeCalendarWPF
{

    public class Presenter : DirectorySelection.PresenterInterface, CreateCategory.PresenterInterface, CreateEventWindow.PresenterInterface, UpdateEventWindow.PresenterInterface, CalendarItems.PresenterInterface

    {
        private ViewInterface _view;
        private CreateCategory.ViewInterface _viewCreateCategory;
        private DirectorySelection.ViewInterface _viewDirectorySelection;
        private CalendarItems.ViewInterface _viewCalendarItems;
        private CreateEventWindow.ViewInterface _viewCreateEventWindow;
        private UpdateEventWindow.ViewInterface _viewUpdateEventWindow;

        // calendar items window filter parameters
        private DateTime? _filterStartDate = null;
        private DateTime? _filterEndDate = null;
        private int? _filterCategoryId = null;
        private bool _summaryByMonth = false;
        private bool _summaryByCategory = false;

        private const string FILE_NAME = "FileName";
        private const string FILE_EXTENSION = ".db";
        private string _directory;
        private HomeCalendar _model;

        /// <summary>
        /// Starts the home calendar app by opening a window that lets user selects location of the calendar file.
        /// </summary>
        /// <param name="view">The view of application.</param>
        public Presenter(ViewInterface view)
        {
            _view = view;

            _viewDirectorySelection = _view.ShowDirectorySelectionWindow(this);

            _directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            _viewDirectorySelection.UpdateDirectory(_directory);
            _viewDirectorySelection.PopulateWindow(FILE_NAME, FILE_EXTENSION);
        }

        /// <summary>
        /// Gets the new directory of calendar file from user and updates its display.
        /// </summary>
        public void GetDirectory()
        {
            _directory = _view.GetDirectory(_directory);

            _viewDirectorySelection.UpdateDirectory(_directory);

        }

        /// <summary>
        /// Proceeds with currently selected path, directory and name of calendar file. (Closes window for calendar file selection, opens window for displaying calendar items)
        /// </summary>
        /// <param name="fileName"></param>
        public void ProcessDirectory(string fileName, string? oldDirectory = null)
        {
            string directory;
            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0)
            {
                if (oldDirectory == null)
                {
                    directory = _directory;
                }
                else
                {
                    directory = oldDirectory;
                }
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                string fileDirectory = Path.Combine(directory, fileName + FILE_EXTENSION);
                _model = new HomeCalendar(fileDirectory, !File.Exists(Path.Combine(directory, fileName + FILE_EXTENSION)));
                SaveLastUsedFile(fileName);

                _viewCalendarItems = _view.ShowCalendarItemsWindow(this, fileDirectory);
                _viewCalendarItems.DisplayFilterCategories(_model.categories.List());
                _viewCalendarItems.DisplayCalendarItems(_model.GetCalendarItems(_filterStartDate, _filterEndDate, _filterCategoryId is not null, _filterCategoryId ?? -1));
                _view.CloseDirectorySelectionWindow();
            }
            else
            {
                _viewDirectorySelection.DisplayError("Invalid file name");
            }
        }


        /// <summary>
        /// Saves the last used file when a user enters one and hits continue.
        /// </summary>
        /// <param name="fileName">The name of the file where the database is saved.</param>
        /// <example>
        /// 
        /// When proccessing directory use the following and the file will be saved
        /// 
        /// <code>
        ///     SaveLastUsedFile(fileName);
        /// </code>

        public void SaveLastUsedFile(string fileName)
        {
            var parser = new FileIniDataParser();
            IniData data;
            if (File.Exists("config.ini"))
            {
                data = parser.ReadFile("config.ini");
            }
            else
            {
                data = new IniData();
            }

            data["Settings"]["Directory"] = _directory;
            data["Settings"]["FileName"] = fileName;

            parser.WriteFile("config.ini", data);
        }
        /// <summary>
        /// Uses the last used file after a user enters one and hits use previous file.
        /// </summary>
        /// <example>
        /// 
        /// When the user hits the use previous file button, use the code below.
        /// 
        /// <code>
        ///     private PresenterInterface _presenter;
        ///      
        ///     private void PreviousFileButton_Click(object sender, RoutedEventArgs e)
        ///     {
        ///         _presenter.UseLastSavedFile();
        ///     }
        /// </code>
        public void UseLastSavedFile()
        {
            var parser = new FileIniDataParser();

            IniData data = parser.ReadFile("config.ini");

            string directory = data["Settings"]["Directory"];
            string fileName = data["Settings"]["FileName"];

            ProcessDirectory(fileName, directory);
        }
        /// <summary>
        /// Checks to see if the user has previously used and set a valid last save file.
        /// </summary>
        /// <example>
        /// 
        /// When the window opens this code enables or disables the button to use previous file.
        /// 
        /// <code>
        ///     private PresenterInterface _presenter;
        ///      
        ///     public Window(Presenter presenter)
        ///     {
        ///         InitializeComponent();
        ///
        ///         _presenter = presenter;
        ///
        ///         btn_PrevFile.IsEnabled = _presenter.ValidLastSavedFile();
        ///     }
        /// </code>
        public bool ValidLastSavedFile()
        {
            if (File.Exists("config.ini"))
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile("config.ini");
                return data["Settings"]["Directory"] != null && data["Settings"]["FileName"] != null;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Loads the category types into the view for category creation.
        /// </summary>
        public void LoadCategoryTypes()
        {
            var categoryTypes = Enum.GetNames(typeof(Category.CategoryType));
            _viewCreateCategory.DisplayCategoryTypes(new List<string>(categoryTypes));
        }
        /// <summary>
        /// Creates a new category with the specified title and category type.
        /// </summary>
        /// <param name="title">The title of the category.</param>
        /// <param name="categoryType">The type of the category.</param>
        /// <example>
        /// <code>
        ///     string title = title_TextBox.Text;
        ///     string categoryType = categoryType_ComboBox.Text;
        ///     _presenter.CreateCategory(title, categoryType);
        /// 
        /// </code>
        /// </example>
        public void CreateCategory(string title, string categoryType)
        {
            List<Category> categories = _model.categories.List();
            bool createCategory = true;
            foreach (Category category in categories)
            {
                if (category.Description.ToLower() == title.ToLower())
                {
                    createCategory = false;
                    _viewCreateCategory.DisplayError($"Error, the title {title} already exist");
                }
            }
            if (title == "" && categoryType == "")
            {
                createCategory = false;
                _viewCreateCategory.DisplayError("Error, title and category type must be specied");
            }
            else if (categoryType == "")
            {
                createCategory = false;
                _viewCreateCategory.DisplayError("Error, category type must be specified");
            }
            else if (title == "")
            {
                createCategory = false;
                _viewCreateCategory.DisplayError("Error, title must be specied");
            }

            if (createCategory)
            {
                _model.categories.Add(title, (Category.CategoryType)Enum.Parse(typeof(Category.CategoryType), categoryType));
                _viewCreateCategory.DisplayCategoryIsCreated();
                _viewCreateEventWindow.DisplayCategories(_model.categories.List());
            }
        }

        /// <summary>
        /// Closes the window for displaying calendar items and opens the window for calendar file selection.
        /// </summary>
        public void SwitchToDirectorySelectionWindow()
        {
            _viewDirectorySelection = _view.ShowDirectorySelectionWindow(this);
            _view.CloseCalendarItemsWindow();
        }
        /// <summary>
        /// Shows the window for creating a new category.
        /// </summary>

        /// <summary>
        /// Displays the window that allows for category creation.
        /// </summary>
        public void ShowCategoryWindow()
        {
            _viewCreateCategory = _view.ShowCreateCategoryWindow(this);
            LoadCategoryTypes();
        }

        /// <summary>
        /// Displays the window that allows for event creation.
        /// </summary>
        public void ShowEventWindow()
        {
            _viewCreateEventWindow = _view.ShowCreateEventWindow(this);
            GetCategoryList();

        }

        /// <summary>
        /// Displays the window that allows for event creation.
        /// </summary>
        /// <param name="id">The id of the event to be updated.</param>
        /// <example>
        /// <code>
        ///     int id = event.Id
        ///     _presenter.ShowUpdateEventWindow(id);
        /// 
        /// </code>
        /// </example>
        public void ShowUpdateEventWindow(int id)
        {
            foreach (Event element in _model.events.List())
            {
                if (element.Id == id)
                {
                    _viewUpdateEventWindow = _view.ShowUpdateEventWindow(this, element);
                    _viewUpdateEventWindow.DisplayCategories(_model.categories.List());
                }
            }

        }

        /// <summary>
        /// Display the category list inside the combobox
        /// </summary>
        public void GetCategoryList()
        {
            _viewCreateEventWindow.DisplayCategories(_model.categories.List());

        }

        /// <summary>
        /// Creates a new event.
        /// </summary>
        /// <param name="startDate">The start time/date of the event.</param>
        /// <param name="duration">The duration of the event.</param>
        /// <param name="category">The category for the new event.</param>
        /// <param name="details">The description of the event.</param>
        /// <example>
        /// <code>
        ///     string startDate = "05/18/2005";
        ///     Category category = new Category("test", Category.CategoryType.Event)
        ///     _presenter.CreateEvent(startDate, 222, category, "test");
        /// 
        /// </code>
        /// </example>
        public void CreateEvent(string startDate, string duration, Category category, string details)
        {
            
            try
            {
                _model.events.Add(DateTime.Parse(startDate), category.Id, double.Parse(duration), details);
                UpdateCalendarItemsDisplay();
                _viewCreateEventWindow.EventCreated();
            }
            catch (Exception ex)
            {
                _viewCreateEventWindow.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Closes the window for displaying calendar items and opens the window for calendar file selection.
        /// </summary>
        public void ChangeOpenedFile()
        {
            _viewDirectorySelection = _view.ShowDirectorySelectionWindow(this);

            _viewDirectorySelection.UpdateDirectory(_directory);
            _viewDirectorySelection.PopulateWindow(FILE_NAME, FILE_EXTENSION);


            _view.CloseCalendarItemsWindow();
        }

        /// <summary>
        /// Updates the minimum start date of calendar items and displays the new list of calendar items.
        /// </summary>
        /// <param name="start">The new minimum start date of calendar items.</param>
        public void UpdateFilterStartDate(DateTime? start)
        {
            _filterStartDate = start;
            _viewCalendarItems.MenuItem_On();
            UpdateCalendarItemsDisplay();
        }

        /// <summary>
        /// Updates the maximum start date of calendar items and displays the new list of calendar items.
        /// </summary>
        /// <param name="end">The new maximum start date of calendar items.</param>
        public void UpdateFilterEndDate(DateTime? end)
        {
            _filterEndDate = end;
            _viewCalendarItems.MenuItem_On();
            UpdateCalendarItemsDisplay();
        }

        /// <summary>
        /// Updates the list of calendar items filtered by the new category if provided, or any category otherwise.
        /// </summary>
        /// <param name="categoryId">The category to filter calendar items by if it's not null.</param>
        public void UpdateFilterCategory(int? categoryId)
        {
            _filterCategoryId = categoryId;
            _viewCalendarItems.MenuItem_On();
            UpdateCalendarItemsDisplay();
        }

        /// <summary>
        /// Updates the way of displaying the calendar items (summary by month).
        /// </summary>
        /// <param name="summaryByMonth">If true, displays summary of calendar items by month.</param>
        public void UpdateSummaryByMonth(bool summaryByMonth)
        {
            _summaryByMonth = summaryByMonth;
            _viewCalendarItems.MenuItem_Off();
            UpdateCalendarItemsDisplay();
        }

        /// <summary>
        /// Updates the way of displaying the calendar items (summary by category).
        /// </summary>
        /// <param name="summaryByCategory">If true, displays summary of calendar items by category.</param>
        public void UpdateSummaryByCategory(bool summaryByCategory)
        {
            _summaryByCategory = summaryByCategory;
            _viewCalendarItems.MenuItem_Off();
            UpdateCalendarItemsDisplay();
        }


        private void UpdateCalendarItemsDisplay()
        {
            if (_summaryByMonth)
            {
                if (_summaryByCategory)
                {
                    // summary by month and category
                    _viewCalendarItems.DisplayCategoryMonthSummary(_model.GetCalendarDictionaryByCategoryAndMonth(_filterStartDate, _filterEndDate, _filterCategoryId is not null, _filterCategoryId ?? -1));
                }
                else
                {
                    // summary by month
                    _viewCalendarItems.DisplayMonthSummary(_model.GetCalendarItemsByMonth(_filterStartDate, _filterEndDate, _filterCategoryId is not null, _filterCategoryId ?? -1));
                }
            }
            else
            {
                if (_summaryByCategory)
                {
                    // summary by category
                    _viewCalendarItems.DisplayCategorySummary(_model.GetCalendarItemsByCategory(_filterStartDate, _filterEndDate, _filterCategoryId is not null, _filterCategoryId ?? -1));
                }
                else
                {
                    // calendar items
                    _viewCalendarItems.DisplayCalendarItems(_model.GetCalendarItems(_filterStartDate, _filterEndDate, _filterCategoryId is not null, _filterCategoryId ?? -1));
                }
            }
        }

        /// <summary>
        /// Updates the details of an existing event.
        /// </summary>
        /// <param name="id">The ID of the event to update.</param>
        /// <param name="startDate">The start date of the event.</param>
        /// <param name="duration">The duration of the event.</param>
        /// <param name="category">The category of the event.</param>
        /// <param name="details">The details of the event.</param>
        public void UpdateEvent(int id, string startDate, string duration, Category category, string details)
        {
            try
            {
                _model.events.UpdateProperties(id, DateTime.Parse(startDate), category.Id, double.Parse(duration), details);
                UpdateCalendarItemsDisplay();
                _viewUpdateEventWindow.EventUpdated();
            }
            catch (Exception ex)
            {
                _viewUpdateEventWindow.ShowError(ex.Message);
            }
        }
        /// <summary>
        /// Deletes the event with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the event to delete.</param>
        public void DeleteEvent(int id)
        {
            try
            {
                _model.events.Delete(id);
                UpdateCalendarItemsDisplay();
            }
            catch (Exception ex)
            {
                _viewUpdateEventWindow.ShowError(ex.Message);
            }

        }

        /// <summary>
        /// Searches through a list of currently displayed calendar items to find the first one that contains or matches the search string then displays it in the view
        /// </summary>
        /// <code>
        ///     //XAML
        ///     <StackPanel x:Name="stk_search" Visibility="Collapsed" Orientation="Horizontal" TextBlock.Foreground="{StaticResource darkForeground}">
        ///         <TextBlock Text = "Search:" Margin="15,0,10,0" HorizontalAlignment="Center" VerticalAlignment="Center"/>
        ///         <TextBox x:Name="txb_search" Width="100"></TextBox>
        ///         <Button Margin = "15,0,10,0" Content="Search" Click="ButtonSearchClick"></Button>
        ///     </StackPanel>
        ///
        ///   //windowx.xaml.cs
        ///   private List<CalendarItem> _calendarItems;
        ///   private int selectedRowIndex;
        ///   //set calendar items
        ///   
        ///     selectedRowIndex = datagridCalendarItems.SelectedIndex + 1;
        ///
        ///     public void UpdateSearchedItems(CalendarItem searchedItems)
        ///     {
        ///         for (int i = selectedRowIndex; i<datagridCalendarItems.Items.Count; i++)
        ///         {
        ///             datagridCalendarItems.ScrollIntoView(datagridCalendarItems.Items[i]);
        ///             CalendarItem item = datagridCalendarItems.Items[i] as CalendarItem;
        ///
        ///             if (searchedItems == item)
        ///             {
        ///                 datagridCalendarItems.SelectedIndex = i;
        ///                 selectedRowIndex = i;
        ///             }
        ///         }
        ///             datagridCalendarItems.ScrollIntoView(datagridCalendarItems.Items[selectedRowIndex]);
        ///     }
        ///   
        ///   // when button is clicked
        ///   _presenter.GetSearchedCalendarItemsList(txb_search.text,_calendarItems,selectedRowIndex);
        /// </code>
        /// <param name="search">The string to search for</param>
        /// <param name="calendarItems">A list of all calendar items that are being displayed</param>
        /// <param name="selectedRowIndex">The index the user has currently selected</param>
        public void GetSearchedCalendarItemsList(string search, List<CalendarItem> calendarItems, int selectedRowIndex)
        {
            List<CalendarItem> itemsToBeSearched = new List<CalendarItem>();
            for (int i = selectedRowIndex; i < calendarItems.Count; i++)
            {
                itemsToBeSearched.Add(calendarItems[i]);
            }
            CalendarItem? searchedCalendarItem = null;
            if (search == "")
            {
                _viewCalendarItems.DisplaySearchError("Please enter atleast 1 character");
            }
            else
            {

                foreach (CalendarItem item in itemsToBeSearched)
                {
                    if (item.ShortDescription.ToLower().Contains(search.ToLower()))
                    {
                        searchedCalendarItem = item;
                        break;
                    }
                    else if (item.DurationInMinutes.ToString() == search.ToLower())
                    {
                        searchedCalendarItem = item;
                        break;
                    }

                }
                if (searchedCalendarItem != null)
                {
                    _viewCalendarItems.UpdateSearchedItems(searchedCalendarItem);

                }
                else
                {
                    _viewCalendarItems.DisplaySearchError("No Items Found");
                    _viewCalendarItems.ScrollToTop();
                }
            }
        }
    }
}
