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
using HomeCalendarWPF.CreateEventWindow;

namespace HomeCalendarWPF
{
    /// <summary>
    /// Interaction logic for CreateEvent.xaml
    /// </summary>
    public partial class CreateEvent : Window, CreateEventWindow.ViewInterface
    {
        private PresenterInterface _presenter;
        public CreateEvent(PresenterInterface presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            PopulateHoursAndMinutes();

        }

        public void DisplayCategories(List<Category> categories)
        {
            ComboBox_Category.ItemsSource = categories;
        }
        public void ShowError(string msg)
        {
            System.Windows.MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void EventCreated()
        {
            System.Windows.MessageBox.Show("Event has been created", "Successful", MessageBoxButton.OK);
        }

        private void Button_Create_Click(object sender, RoutedEventArgs e)
        {
            Category category = ComboBox_Category.SelectedItem as Category;
            _presenter.CreateEvent(TextBox_StartDate.Text + " " + ComboBox_StartTimeHours.Text + ":" + ComboBox_StartTimeMinutes.Text, TextBox_Duration.Text, category, TextBox_Details.Text);
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
