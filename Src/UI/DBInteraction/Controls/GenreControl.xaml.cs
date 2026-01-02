using BookOrg.Src.Logic.Core.DAO;
using BookOrg.Src.Logic.Core.DBEntities;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace BookOrg.Src.UI.DBInteraction.Controls
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="GenreControl"]/*'/>
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
                if (MessageBox.Show($"Delete genre '{genre.GenreName}'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DeleteItem(genre);
                }
            }
        }
    }
}