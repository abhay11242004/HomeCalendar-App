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
using HomeCalendarWPF.DirectorySelection;

namespace HomeCalendarWPF.DirectorySelection
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class Window : System.Windows.Window, ViewInterface
    {
        private PresenterInterface _presenter;
        private bool _errorShowed;


        public Window(Presenter presenter)
        {
            InitializeComponent();

            _presenter = presenter;

            btn_PrevFile.IsEnabled = _presenter.ValidLastSavedFile();
        }

        private void ButtonDirectory_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _presenter.GetDirectory();
        }

        #region interface

        public void UpdateDirectory(string directory)
        {
            buttonDirectory.Content = directory;
        }

        #endregion

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ProcessDirectory(txb_fileName.Text);
        }

        async public void DisplayError(string message)
        {
            if (_errorShowed)
            {
                txb_ErrorMessage.Text = string.Empty;
                await Task.Delay(100);
            }

            txb_ErrorMessage.Text = message;
            _errorShowed = true;
        }

        public void PopulateWindow(string fileName, string fileExtension)
        {
            txb_fileName.Text = fileName;
            txb_fileExtension.Text = fileExtension;
        }

        private void PreviousFileButton_Click(object sender, RoutedEventArgs e)
        {
            _presenter.UseLastSavedFile();
        }
    }
}
