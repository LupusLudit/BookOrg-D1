using BookOrg.Src.Logic.Core.DAO.Tables;
using BookOrg.Src.Logic.Core.DAO.Views;
using BookOrg.Src.Logic.Core.DBEntities.Tables;
using BookOrg.Src.Safety;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookOrg.Src.UI.DBInteraction.Controls
{
    /// <summary>
    /// Interaction logic for GenreControl.xaml
    /// </summary>
    public partial class GenreControl : UserControl
    {
        public ObservableCollection<Genre> Genres { get; set; }
        private readonly GenreDAO genreDAO;
        public GenreControl(SqlConnection connection)
        {
            InitializeComponent();

            Genres = new ObservableCollection<Genre>();
            genreDAO = new GenreDAO(connection);

            this.DataContext = this;
            this.Loaded += GenresControlLoaded;
        }

        private void GenresControlLoaded(object sender, RoutedEventArgs e)
        {
            LoadGenres();
        }

        private void DataGridCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

            if (e.EditAction != DataGridEditAction.Commit) return;

            if (e.Row.Item is Genre editedGenre)
            {
                UpdateGenre(editedGenre);
            }
        }

        private void LoadGenres()
        {
            SafeExecutor.Execute(
                () =>
                {
                    List<Genre> loadedGenres = genreDAO.GetAll();

                    Genres.Clear();
                    foreach (Genre genre in loadedGenres)
                    {
                        Genres.Add(genre);
                    }
                },
                "Failed to load genres from the database."
            );
        }

        // TODO: Update later - no redundancy or create abstract Control base/IControl (each control has update, add, delete, getAll)
        private void AddNewGenreClick(object sender, RoutedEventArgs e) => AddNewGenre(new Genre("New Genre"));

        private void DeleteGenreClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Genre genre)
            {
                var result = MessageBox.Show($"Delete genre '{genre.GenreName}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes) return;

                DeleteGenre(genre);
            }
        }

        private void UpdateGenre(Genre genre)
        {
            SafeExecutor.Execute(
                () => genreDAO.Update(genre),
                "Failed to update genre."
            );
        }

        private void AddNewGenre(Genre genre)
        {
            SafeExecutor.Execute(
                () =>
                {
                    genreDAO.Insert(genre);
                    Genres.Add(genre);
                },
                "Failed to add new genre."
            );
        }

        private void DeleteGenre(Genre genre)
        {
            SafeExecutor.Execute(
                () =>
                {
                    genreDAO.Delete(genre);
                    Genres.Remove(genre);
                },
                "Failed to delete genre."
            );
        }
    }
}
