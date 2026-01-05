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
    /// <include file='../../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="GenreControl"]/*'/>
    public partial class GenreControl : EntityControlBase<Genre>
    {
        public GenreControl(SqlConnection connection)
        {
            InitializeComponent();
            Dao = new GenreDAO(connection);
            Loaded += (sender, e) => LoadData();
        }

        private void AddNewGenreClick(object sender, RoutedEventArgs e) => AddItem(new Genre("New Genre"));

        private void DataGridCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit && e.Row.Item is Genre editedGenre)
            {
                UpdateItem(editedGenre);
            }
        }

        private void DeleteGenreClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Genre genre)
            {
                MessageBoxResult result = MessageBox.Show($"Do your really want to delete genre '{genre.GenreName}'?" +
                    $" THIS WILL AFFECT DIFERENT DATABASE ENTITES WHICH RELAY ON THIS GENRE. This action cannot be undone.", "Confirm", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteItem(genre);
                }
            }
        }

        private void ImportClick(object sender, RoutedEventArgs e)
        {
            SafeExecutor.Execute(
                () =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Filter = "CSV files (*.csv)|*.csv",
                        Title = "Import genres from CSV"
                    };

                    if (openFileDialog.ShowDialog() == true)
                    {
                        CsvImporter importer = new CsvImporter();
                        importer.Import(openFileDialog.FileName, (GenreDAO)Dao);
                        LoadData();
                    }
                },
                "Failed to import genres from CSV."
            );
        }
    }
}