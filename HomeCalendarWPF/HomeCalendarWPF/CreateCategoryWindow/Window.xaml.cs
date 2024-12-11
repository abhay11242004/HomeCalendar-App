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

namespace HomeCalendarWPF.CreateCategory
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class Window : System.Windows.Window, ViewInterface
    {
        private PresenterInterface _presenter;

        public Window(PresenterInterface presenter)
        {
            InitializeComponent();

            _presenter = presenter;
        }

        public void DisplayCategoryTypes(List<string> categoryTypes)
        {
            categoryType_ComboBox.ItemsSource = categoryTypes;
        }
        private void Button_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void DisplayError(string message)
        {
            System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public void DisplayCategoryIsCreated()
        {
            title_TextBox.Clear();
            System.Windows.MessageBox.Show("Category has been created", "Successful", MessageBoxButton.OK);
        }
        public void Create_Button(object sender, RoutedEventArgs e)
        {
            string title = title_TextBox.Text;
            string categoryType = categoryType_ComboBox.Text;
            _presenter.CreateCategory(title, categoryType);
        }
    }
}
