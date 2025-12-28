using BookOrg.Src.Logic.Core.DAO.Views;
using BookOrg.Src.Logic.Core.DBEntities.Views;
using BookOrg.Src.Safety;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace BookOrg.Src.UI.DBInteraction.Controls
{
    public partial class BooksControl : UserControl
    {
        public ObservableCollection<ViewBooks> Books { get; set; }
        private readonly ViewBooksDAO booksViewDAO;

        public BooksControl(ViewBooksDAO booksViewDAO)
        {
            InitializeComponent();
            this.booksViewDAO = booksViewDAO;

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
            SafeExecutor.Execute(() =>
            {
                var bookList = booksViewDAO.GetAllBooks();

                Books.Clear();
                foreach (var book in bookList)
                {
                    Books.Add(book);
                }
            }, "Failed to load books from the database.");
        }
    }
}