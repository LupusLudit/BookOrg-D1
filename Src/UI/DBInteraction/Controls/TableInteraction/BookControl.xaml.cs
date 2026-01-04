using BookOrg.Src.Logic.Core.DAO;
using BookOrg.Src.Logic.Core.DBEntities;
using BookOrg.Src.Safety;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace BookOrg.Src.UI.DBInteraction.Controls.TableInteraction
{
    /// <include file='../../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="BookControl"]/*'/>
    public partial class BookControl : EntityControlBase<Book>
    {
        private AuthorDAO authorDAO;
        private GenreDAO genreDAO;
        private Book selectedBook;

        public ObservableCollection<Author> AllAuthors { get; set; } = new ObservableCollection<Author>();
        public ObservableCollection<Genre> AllGenres { get; set; } = new ObservableCollection<Genre>();

        public Book SelectedBook
        {
            get => selectedBook;
            set { selectedBook = value; OnPropertyChanged(); }
        }

        public BookControl(SqlConnection connection)
        {
            InitializeComponent();
            Dao = new BookDAO(connection);
            authorDAO = new AuthorDAO(connection);
            genreDAO = new GenreDAO(connection);

            Loaded += (sender, e) => LoadData();
        }

        /// <summary>
        /// Loads books, authors, and genres from the database safely.
        /// </summary>
        public override void LoadData()
        {
            SafeExecutor.Execute(
                () => {
                    List<Book> loadedBooks = Dao.GetAll();
                    Items.Clear();
                    foreach (var book in loadedBooks)
                    {
                        Items.Add(book);
                    }

                    List<Author> loadedAuthors = authorDAO.GetAll();
                    AllAuthors.Clear();
                    foreach (var author in loadedAuthors)
                    {
                        AllAuthors.Add(author);
                    }

                    List<Genre> loadedGenres = genreDAO.GetAll();
                    AllGenres.Clear();
                    foreach (var genre in loadedGenres)
                    {
                        AllGenres.Add(genre);
                    }
                },
                "Failed to load library data."
            );
        }

        private void AddNewBookClick(object sender, RoutedEventArgs e) => AddItem(new Book("New Book", false, 0, 0));

        private void DataGridCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit && e.Row.Item is Book book)
            {
                UpdateItem(book);
            }
        }

        private void DeleteBookClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Book book)
            {
                MessageBoxResult result = MessageBox.Show($"Do your really want to delete book '{book.Title}'?" +
                    $" THIS WILL AFFECT DIFERENT DATABASE ENTITES WHICH RELAY ON THIS BOOK. This action cannot be undone.", "Confirm", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteItem(book);
                }
            }
        }

        private void RemoveAuthorClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Author author && button.Tag is Book book)
            {
                book.Authors.Remove(author);
                UpdateItem(book);
            }
        }

        private void RemoveGenreClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Genre genre && button.Tag is Book book)
            {
                book.Genres.Remove(genre);
                UpdateItem(book);
            }
        }

        private void AddAuthorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is Author selectedAuthor)
            {
                Book book = (Book)comboBox.DataContext;

                if (book != null && !book.Authors.Any(a => a.ID == selectedAuthor.ID))
                {
                    book.Authors.Add(selectedAuthor);
                    UpdateItem(book);
                }

                comboBox.SelectedIndex = -1;
            }
        }

        private void AddGenreSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is Genre selectedGenre)
            {
                Book book = (Book)comboBox.DataContext;

                if (book != null && !book.Genres.Any(g => g.ID == selectedGenre.ID))
                {
                    book.Genres.Add(selectedGenre);
                    UpdateItem(book);
                }

                comboBox.SelectedIndex = -1;
            }
        }
    }
}