using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Calendar;
using HomeCalendarWPF.UpdateEventWindow;

namespace HomeCalendarWPF
{
    /// <summary>
    /// Interaction logic for UpdateEvent.xaml
    /// </summary>
    public partial class UpdateEvent : Window, UpdateEventWindow.ViewInterface
    {
        private PresenterInterface _presenter;
        private Event _event;
        public UpdateEvent(PresenterInterface presenter, Event event_)
        {
            InitializeComponent();
            _presenter = presenter;
            PopulateHoursAndMinutes();
            _event = event_;
            DisplayEventInfo();
        }
        public void DisplayEventInfo()
        {
            TextBox_StartDate.SelectedDate = _event.StartDateTime;
            ComboBox_Category.SelectedItem = _event.Category;
            TextBox_Details.Text = _event.Details;
            TextBox_Duration.Text = _event.DurationInMinutes.ToString();     
        }
        public void DisplayCategories(List<Category> categories)
        {
            ComboBox_Category.ItemsSource = categories;
        }
        public void ShowError(string msg)
        {
            System.Windows.MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void EventUpdated()
        {
            System.Windows.MessageBox.Show("Event has been updated", "Successful", MessageBoxButton.OK);
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button_Update_Click(object sender, RoutedEventArgs e)
        {
            Category category = ComboBox_Category.SelectedItem as Category;
            _presenter.UpdateEvent(_event.Id, TextBox_StartDate.Text + " " + ComboBox_StartTimeHours.Text + ":" + ComboBox_StartTimeMinutes.Text, TextBox_Duration.Text, category, TextBox_Details.Text);
            
        }
        private void TextBox_StartDate_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PopulateHoursAndMinutes()
        {
            for (int i = 0; i < 24; i++) 
            {
                ComboBox_StartTimeHours.Items.Insert(i, (i + 1).ToString());
            }
            for (int i = 0; i < 60; i++)
            {
                if (i < 10)
                {
                    ComboBox_StartTimeMinutes.Items.Insert(i, "0" + i.ToString());
                }
                else
                {
                    ComboBox_StartTimeMinutes.Items.Insert(i, i.ToString());
                }
            }
        }

        private void Button_Add_Category_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ShowCategoryWindow();
        }
    }
}
