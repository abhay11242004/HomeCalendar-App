using Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CalendarItem = Calendar.CalendarItem;
using Binding = System.Windows.Data.Binding;
using CheckBox = System.Windows.Controls.CheckBox;
using System.Windows.Forms;

namespace HomeCalendarWPF.CalendarItems
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class Window : System.Windows.Window, ViewInterface
    {
        private PresenterInterface _presenter;
        private List<CalendarItem> _calendarItems;
        private int selectedRowIndex = 0;

        public Window(PresenterInterface presenter)
        {
            _presenter = presenter;

            InitializeComponent();

            comboboxCategories.SelectionChanged += comboboxCategories_SelectionChanged;

        }

        private void ButtonAddEvent_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ShowEventWindow();
        }

        private void CheckboxMinStartDate_Checked(object sender, RoutedEventArgs e)
        {
            minStartDatePicker.IsEnabled = true;
            minStartDatePicker.SelectedDate = DateTime.Now.Date;

            _presenter.UpdateFilterStartDate(minStartDatePicker.SelectedDate);
        }

        private void CheckboxMinStartDate_Unchecked(object sender, RoutedEventArgs e)
        {
            minStartDatePicker.IsEnabled = false;
            minStartDatePicker.SelectedDate = null;

            _presenter.UpdateFilterStartDate(null);
        }

        private void CheckboxMaxStartDate_Checked(object sender, RoutedEventArgs e)
        {
            maxStartDatePicker.IsEnabled = true;
            maxStartDatePicker.SelectedDate = DateTime.Now.Date.AddDays(1).AddMilliseconds(-1);

            _presenter.UpdateFilterEndDate(maxStartDatePicker.SelectedDate);
        }

        private void CheckboxMaxStartDate_Unchecked(object sender, RoutedEventArgs e)
        {
            maxStartDatePicker.IsEnabled = false;
            maxStartDatePicker.SelectedDate = null;

            _presenter.UpdateFilterEndDate(null);
        }

        private void ButtonChangeOpenedFile_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ChangeOpenedFile();
        }

        public void DisplayFilterCategories(List<Category> categories)
        {
            foreach (Category category in categories)
                comboboxCategories.Items.Add(category);
        }

        public void DisplayCalendarItems(List<CalendarItem> calendarItems)
        {
            datagridCalendarItems.Columns.Clear();

            datagridCalendarItems.ItemsSource = calendarItems;
            _calendarItems = calendarItems;
            if (_calendarItems.Count > 0)
            {
                stk_search.Visibility = Visibility.Visible;
            }
            else
            {
                stk_search.Visibility = Visibility.Collapsed;
            }

            datagridCalendarItems.Columns.Add(new DataGridTextColumn() { Header = "Start Date", Binding = new Binding("StartDateTime") { StringFormat = "yyyy/MM/dd" } });
            datagridCalendarItems.Columns.Add(new DataGridTextColumn() { Header = "Start Time", Binding = new Binding("StartDateTime") { StringFormat = "HH:mm:ss" } });
            datagridCalendarItems.Columns.Add(new DataGridTextColumn() { Header = "Category", Binding = new Binding("Category") });
            datagridCalendarItems.Columns.Add(new DataGridTextColumn() { Header = "Description", Binding = new Binding("ShortDescription") });
            datagridCalendarItems.Columns.Add(new DataGridTextColumn() { Header = "Duration", Binding = new Binding("DurationInMinutes") });
            datagridCalendarItems.Columns.Add(new DataGridTextColumn() { Header = "Busy Time", Binding = new Binding("BusyTime") });
        }

        public void DisplayMonthSummary(List<CalendarItemsByMonth> calendarItemsByMonths)
        {
            stk_search.Visibility = Visibility.Collapsed;
            datagridCalendarItems.Columns.Clear();
            datagridCalendarItems.ItemsSource = calendarItemsByMonths;

            datagridCalendarItems.Columns.Add(new DataGridTextColumn() { Header = "Month", Binding = new Binding("Month") });
            datagridCalendarItems.Columns.Add(new DataGridTextColumn() { Header = "Total Busy Time", Binding = new Binding("TotalBusyTime") });
        }

        public void DisplayCategorySummary(List<CalendarItemsByCategory> calendarItemsByCategories)
        {
            stk_search.Visibility = Visibility.Collapsed;
            datagridCalendarItems.Columns.Clear();
            datagridCalendarItems.ItemsSource = calendarItemsByCategories;

            datagridCalendarItems.Columns.Add(new DataGridTextColumn() { Header = "Category", Binding = new Binding("Category") });
            datagridCalendarItems.Columns.Add(new DataGridTextColumn() { Header = "Total Busy Time", Binding = new Binding("TotalBusyTime") });
        }

        public void DisplayCategoryMonthSummary(List<Dictionary<string, object>> calendarItemsByCategoryAndMonth)
        {
            stk_search.Visibility = Visibility.Collapsed;
            datagridCalendarItems.Columns.Clear();
            datagridCalendarItems.ItemsSource = calendarItemsByCategoryAndMonth;

            datagridCalendarItems.Columns.Add(new DataGridTextColumn() { Header = "Month", Binding = new Binding("[Month]") });
            
            foreach (string key in calendarItemsByCategoryAndMonth.Last().Keys)
            {
                // if key is a category
                if (key != "Month" && key != "TotalBusyTime")
                {
                    datagridCalendarItems.Columns.Add(new DataGridTextColumn() { Header = key, Binding = new Binding($"[{key}]") });
                }
            }

            datagridCalendarItems.Columns.Add(new DataGridTextColumn() { Header = "Total Busy Time", Binding = new Binding("[TotalBusyTime]") });
        }

        private void comboboxCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Category? filterCategory = comboboxCategories.SelectedItem as Category;
            int? filterCategoryId = filterCategory?.Id;

            _presenter.UpdateFilterCategory(filterCategoryId);
        }

        private void MinStartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            minStartDatePicker.SelectedDate = minStartDatePicker.SelectedDate?.Date;

            _presenter.UpdateFilterStartDate(minStartDatePicker.SelectedDate);
        }

        private void MaxStartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            maxStartDatePicker.SelectedDate = maxStartDatePicker.SelectedDate?.Date.AddDays(1).AddMilliseconds(-1);

            _presenter.UpdateFilterEndDate(maxStartDatePicker.SelectedDate);
        }


        private void CheckBoxByMonth_Click(object sender, RoutedEventArgs e)
        {
            _presenter.UpdateSummaryByMonth(((CheckBox)sender).IsChecked ?? false);
        }

        private void CheckBoxByCategory_Click(object sender, RoutedEventArgs e)
        {
            _presenter.UpdateSummaryByCategory(((CheckBox)sender).IsChecked ?? false);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            CalendarItem calendarItem = datagridCalendarItems.SelectedItem as CalendarItem;
            _presenter.DeleteEvent(calendarItem.EventID);
            System.Windows.MessageBox.Show("Event has been deleted", "Successful", MessageBoxButton.OK);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            CalendarItem calendarItem = datagridCalendarItems.SelectedItem as CalendarItem;
            _presenter.ShowUpdateEventWindow(calendarItem.EventID);
        }

        public void MenuItem_On()
        {
            MenuItem_Delete.IsEnabled = true;
            MenuItem_Update.IsEnabled = true;
        }

        public void MenuItem_Off()
        {
            MenuItem_Delete.IsEnabled = false;
            MenuItem_Update.IsEnabled = false;
        }

        private void datagridCalendarItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CalendarItem calendarItem = datagridCalendarItems.SelectedItem as CalendarItem;
            if(calendarItem == null)
            {
                System.Windows.MessageBox.Show("There was a problem when retreiving your event", "Successful", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                _presenter.ShowUpdateEventWindow(calendarItem.EventID);
            }
            
        }
        private void ButtonSearchClick(object sender, RoutedEventArgs e)
        {
            selectedRowIndex = datagridCalendarItems.SelectedIndex+1;
            if (selectedRowIndex < 0)
                selectedRowIndex = 0;
            _presenter.GetSearchedCalendarItemsList(txb_search.Text, _calendarItems, selectedRowIndex);
        }

        public void UpdateSearchedItems(CalendarItem searchedItems)
        {
            for (int i = selectedRowIndex; i < datagridCalendarItems.Items.Count; i++)
            {
                datagridCalendarItems.ScrollIntoView(datagridCalendarItems.Items[i]);
                CalendarItem item = datagridCalendarItems.Items[i] as CalendarItem;

                if (searchedItems == item)
                {
                    datagridCalendarItems.SelectedIndex = i;
                    selectedRowIndex = i;
                }
            }
            datagridCalendarItems.ScrollIntoView(datagridCalendarItems.Items[selectedRowIndex]);
        }

        public void DisplaySearchError(string error)
        {
            System.Windows.MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        }

        public void ScrollToTop()
        {
            datagridCalendarItems.ScrollIntoView(datagridCalendarItems.Items[0]);
            selectedRowIndex = -1;
            datagridCalendarItems.SelectedIndex = selectedRowIndex;
        }

    }
}
