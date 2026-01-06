using BookOrg.Src.Logic.Core.DAO;
using BookOrg.Src.Logic.Core.DBEntities;
using BookOrg.Src.Logic.Importing;
using BookOrg.Src.Safety;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace BookOrg.Src.UI.DBInteraction.Controls.TableInteraction
{
    /// <include file='../../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="AuthorControl"]/*'/>
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
                MessageBoxResult result = MessageBox.Show($"Do your really want to delete author '{author.AuthorName}'?" +
                    $" THIS WILL AFFECT DIFERENT DATABASE ENTITES WHICH RELAY ON THIS AUTHOR. This action cannot be undone.", "Confirm", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteItem(author);
                }
            }
        }

        /// <summary>
        /// Handles the author data import from CSV file when the Import button is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ImportClick(object sender, RoutedEventArgs e)
        {
            SafeExecutor.Execute(
                () =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Filter = "CSV files (*.csv)|*.csv",
                        Title = "Import author from CSV"
                    };

                    if (openFileDialog.ShowDialog() == true)
                    {
                        CsvImporter importer = new CsvImporter();
                        importer.Import(openFileDialog.FileName, (AuthorDAO)Dao);
                        LoadData();
                    }
                },
                "Failed to import author from CSV."
            );
        }
    }
}