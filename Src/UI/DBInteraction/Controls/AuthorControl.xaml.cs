using BookOrg.Src.Logic.Core.DAO;
using BookOrg.Src.Logic.Core.DBEntities;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace BookOrg.Src.UI.DBInteraction.Controls
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="AuthorControl"]/*'/>
    public partial class AuthorControl : EntityControlBase<Author>
    {
        public AuthorControl(SqlConnection connection)
        {
            InitializeComponent();
            Dao = new AuthorDAO(connection);

            Loaded += (sender, e) => LoadData();
        }

        private void AddNewAuthorClick(object sender, RoutedEventArgs e) => AddItem(new Author("New Author"));

        private void DataGridCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit && e.Row.Item is Author editedAuthor)
            {
                UpdateItem(editedAuthor);
            }
        }

        private void DeleteAuthorClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Author author)
            {
                if (MessageBox.Show($"Delete author '{author.AuthorName}'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DeleteItem(author);
                }
            }
        }
    }
}