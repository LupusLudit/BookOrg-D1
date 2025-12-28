using BookOrg.Src.Logic.Core.DAO.Views;
using BookOrg.Src.UI.DBInteraction.Controls;
using Microsoft.Data.SqlClient;
using System.Windows;

namespace BookOrg.Src.UI.DBInteraction
{
    public partial class MainWindow : Window
    {
        private readonly SqlConnection connection;
        private readonly ViewBooksDAO booksViewDAO;

        public MainWindow(SqlConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            this.booksViewDAO = new ViewBooksDAO(connection);

            this.DataContext = new { CurrentControl = new BooksControl(booksViewDAO) };
        }
    }
}