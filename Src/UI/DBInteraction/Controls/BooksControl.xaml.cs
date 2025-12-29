using BookOrg.Src.Logic.Core.DAO.Views;
using BookOrg.Src.Logic.Core.DBEntities.Views;
using BookOrg.Src.Safety;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace BookOrg.Src.UI.DBInteraction.Controls
{
    public partial class BooksControl : UserControl
    {
        public ObservableCollection<ViewBooks> Books { get; set; }
        private readonly ViewBooksDAO viewBooksDAO;

        public BooksControl(SqlConnection connection)
        {
            InitializeComponent();

            viewBooksDAO = new ViewBooksDAO(connection);
            Books = new ObservableCollection<ViewBooks>();

            this.DataContext = this;
            this.Loaded += BooksControlLoaded;
        }

        private void BooksControlLoaded(object sender, RoutedEventArgs e)
        {
            LoadBooks();
        }

        private void LoadBooks()
        {
            SafeExecutor.Execute(
                () =>
                {
                    var loadedBooks = viewBooksDAO.GetAllBooks();

                    Books.Clear();
                    foreach (var book in loadedBooks)
                    {
                        Books.Add(book);
                    }
                },
                "Failed to load books from the database."
            );
        }
    }
}