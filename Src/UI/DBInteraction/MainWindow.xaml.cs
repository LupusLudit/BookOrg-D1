using BookOrg.Src.Logic.Connection;
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
        }
    }
}