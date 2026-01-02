using System.Windows;
using BookOrg.Src.Logic.Connection;
using BookOrg.Src.UI.DBInteraction;

namespace BookOrg.Src.UI.ConnectionWindow
{
    /// <include file='../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="ConnectionWindow"]/*'/>
    public partial class ConnectionWindow : Window
    {
        public readonly string ConfigPath = "App.config";
        private readonly IConnectionFactory connectionFactory;

        public ConnectionWindow()
        {
            InitializeComponent();
            connectionFactory = new SqlServerConnectionFactory();

            this.Loaded += ConnectionWindowLoaded;
        }

        private async void ConnectionWindowLoaded(object sender, RoutedEventArgs e)
        {
            await AttemptConnection();
        }

        private async void ReloadClick(object sender, RoutedEventArgs e)
        {
            await AttemptConnection();
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async Task AttemptConnection()
        {
            StatusText.Text = "Connecting to the database...";
            ConnectionProgressBar.Visibility = Visibility.Visible;
            ActionButtons.Visibility = Visibility.Collapsed;

            var connection = await Task.Run(() => connectionFactory.CreateConnection());

            if (connection != null)
            {
                StatusText.Text = "Connection successful!";
                ConnectionProgressBar.Visibility = Visibility.Collapsed;

                await Task.Delay(500);

                MainWindow mainWindow = new MainWindow(connection);
                mainWindow.Show();

                this.Close();
            }
            else
            {
                StatusText.Text = "Connection failed.";
                ConnectionProgressBar.Visibility = Visibility.Collapsed;
                ActionButtons.Visibility = Visibility.Visible;
            }
        }
    }
}