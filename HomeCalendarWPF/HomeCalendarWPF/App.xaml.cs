using System.Configuration;
using System.Data;
using System.IO;
using System.Media;
using System.Windows;
using HomeCalendarWPF;
using System.Windows.Media.Animation;
using HomeCalendarWPF.DirectorySelection;
using Calendar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;
using System.Windows.Controls;

namespace HomeCalendarWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application, ViewInterface
    {
        private DirectorySelection.Window _directorySelectionWindow;
        private CalendarItems.Window _calendarItemsWindow;
        private CreateCategory.Window _createCategoryWindow;
        private CreateEvent _createEventWindow;
        private UpdateEvent _updateEventWindow;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Presenter presenter = new Presenter(this);
        }


        #region ViewInterface

        public DirectorySelection.ViewInterface ShowDirectorySelectionWindow(Presenter presenter)
        {
            _directorySelectionWindow = new DirectorySelection.Window(presenter);

            _directorySelectionWindow.Show();

            return _directorySelectionWindow;
        }
        public void CloseDirectorySelectionWindow()
        {
            _directorySelectionWindow.Close();
        }
        public CalendarItems.ViewInterface ShowCalendarItemsWindow(Presenter presenter, string directory)
        {
            _calendarItemsWindow = new CalendarItems.Window(presenter);

            _calendarItemsWindow.Show();
            _calendarItemsWindow.directoryLocation.Text = directory;

            return _calendarItemsWindow;
        }

        public void CloseCalendarItemsWindow()
        {
            _calendarItemsWindow.Close();
        }

        public string GetDirectory(string initialDirectory)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog()
            {
                InitialDirectory = initialDirectory
            };

            return (dialog.ShowDialog() == DialogResult.OK)? dialog.SelectedPath : initialDirectory;
        }

        public CreateCategory.ViewInterface ShowCreateCategoryWindow(Presenter presenter)
        {
            _createCategoryWindow = new CreateCategory.Window(presenter);

            _createCategoryWindow.Show();

            return _createCategoryWindow;
        }

        public CreateEventWindow.ViewInterface ShowCreateEventWindow(Presenter presenter)
        {
            _createEventWindow = new CreateEvent(presenter);

            _createEventWindow.Show();

            return _createEventWindow;
        }

        public UpdateEventWindow.ViewInterface ShowUpdateEventWindow(Presenter presenter, Event @event)
        {
            _updateEventWindow = new UpdateEvent(presenter, @event);

            _updateEventWindow.Show();

            return _updateEventWindow;
        }

        #endregion
    }

}
