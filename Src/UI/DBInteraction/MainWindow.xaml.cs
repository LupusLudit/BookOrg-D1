using BookOrg.Src.UI.DBInteraction.Controls;
using Microsoft.Data.SqlClient;
using System.Windows;

namespace BookOrg.Src.UI.DBInteraction
{
    public partial class MainWindow : Window
    {
        private readonly SqlConnection connection;

        public MainWindow(SqlConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            // Debugging
            this.DataContext = new { CurrentControl = new AuthorControl(connection) };
        }
    }
}