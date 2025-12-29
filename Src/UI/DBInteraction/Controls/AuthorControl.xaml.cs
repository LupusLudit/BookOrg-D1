using BookOrg.Src.Logic.Core.DAO.Tables;
using BookOrg.Src.Logic.Core.DBEntities.Tables;
using BookOrg.Src.Safety;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace BookOrg.Src.UI.DBInteraction.Controls
{
    public partial class AuthorControl : UserControl
    {
        public ObservableCollection<Author> Authors { get; set; }
        private readonly AuthorDAO authorDAO;

        public AuthorControl(SqlConnection connection)
        {
            InitializeComponent();

            Authors = new ObservableCollection<Author>();
            authorDAO = new AuthorDAO(connection);

            DataContext = this;
            Loaded += AuthorsControlLoaded;
        }

        private void AuthorsControlLoaded(object sender, RoutedEventArgs e)
        {
            LoadAuthors();
        }

        private void LoadAuthors()
        {
            SafeExecutor.Execute(
                () =>
                {
                    List<Author> loadedAuthors = authorDAO.GetAll();
                    Authors.Clear();

                    foreach (Author author in loadedAuthors)
                        Authors.Add(author);
                },
                "Failed to load authors from the database."
            );
        }

        private void DataGridCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit) return;

            if (e.Row.Item is Author editedAuthor)
            {
                UpdateAuthor(editedAuthor);
            }
        }

        private void AddNewAuthorClick(object sender, RoutedEventArgs e) => AddNewAuthor(new Author("New Author"));

        private void DeleteAuthorClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Author author)
            {
                var result = MessageBox.Show($"Delete author '{author.AuthorName}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes) return;

                DeleteAuthor(author);
            }
        }

        private void UpdateAuthor(Author author)
        {
            SafeExecutor.Execute(
                () => authorDAO.Update(author),
                "Failed to update author."
            );
        }

        private void AddNewAuthor(Author author)
        {
            SafeExecutor.Execute(
                () =>
                {
                    authorDAO.Insert(author);
                    Authors.Add(author);
                },
                "Failed to add new author."
            );
        }

        private void DeleteAuthor(Author author)
        {
            SafeExecutor.Execute(
                () =>
                {
                    authorDAO.Delete(author);
                    Authors.Remove(author);
                },
                "Failed to delete author."
            );
        }
    }
}
