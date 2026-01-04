using BookOrg.Src.UI.DBInteraction.Controls.Menus;
using BookOrg.Src.UI.DBInteraction.Controls.TableInteraction;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace BookOrg.Src.UI.DBInteraction
{
    /// <include file='../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="MainWindow"]/*'/>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly SqlConnection connection;
        private UserControl currentControl;
        private Visibility backButtonVisibility;

        public event PropertyChangedEventHandler? PropertyChanged;

        public UserControl CurrentControl
        {
            get => currentControl;
            set { currentControl = value; OnPropertyChanged(); }
        }

        public Visibility BackButtonVisibility
        {
            get => backButtonVisibility;
            set { backButtonVisibility = value; OnPropertyChanged(); }
        }

        public MainWindow(SqlConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            this.DataContext = this;

            LoadMenu();
        }

        private void LoadMenu()
        {
            CurrentControl = new MainMenuControl(Navigate);
            BackButtonVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Navigates to the specified destination control based on the menu selection.
        /// </summary>
        /// <param name="destination">The tag from the menu button.</param>
        private void Navigate(string destination)
        {
            UserControl? newView = null;
            string newTitle = "";

            switch (destination)
            {
                case "Author":
                    newView = new AuthorControl(connection);
                    break;
                case "Book":
                    newView = new BookControl(connection);
                    break;
                case "Customer":
                    newView = new CustomerControl(connection);
                    break;
                case "Genre":
                    newView = new GenreControl(connection);
                    break;
                case "Loan":
                    newView = new LoanControl(connection);
                    break;
            }

            if (newView != null)
            {
                CurrentControl = newView;
                BackButtonVisibility = Visibility.Visible;
            }
        }

        private void BackToMenuClick(object sender, RoutedEventArgs e)
        {
            LoadMenu();
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}