using BookOrg.Src.Logic.Core.DAO;
using BookOrg.Src.Logic.Core.DBEntities;
using BookOrg.Src.Safety;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace BookOrg.Src.UI.DBInteraction.Controls.TableInteraction
{
    /// <include file='../../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="EntityControlBase"]/*'/>
    public class EntityControlBase<T> : UserControl, INotifyPropertyChanged where T : class, IDBEntity
    {
        protected DAOBase<T> Dao { get; set; }

        public ObservableCollection<T> Items { get; set; } = new ObservableCollection<T>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public EntityControlBase()
        {
            DataContext = this;
        }

        /// <summary>
        /// Loads data from the database into the Items collection safely.
        /// </summary>
        public virtual void LoadData()
        {
            SafeExecutor.Execute(
                () =>
                {
                    var loadedItems = Dao.GetAll();
                    Items.Clear();
                    foreach (var item in loadedItems)
                    {
                        Items.Add(item);
                    }
                },
                "Failed to load data from the database."
            );
        }

        /// <summary>
        /// Adds a new item to the database and the UI collection safely.
        /// </summary>
        /// <param name="item">The item to add.</param>
        protected virtual void AddItem(T item)
        {
            SafeExecutor.Execute(
                () =>
                {
                    Dao.Insert(item);
                    Items.Add(item);
                },
                "Failed to add item."
            );
        }

        /// <summary>
        /// Updates an existing item in the database safely.
        /// </summary>
        /// <param name="item">The item to update.</param>
        protected virtual void UpdateItem(T item)
        {
            SafeExecutor.Execute(
                () => Dao.Update(item),
                "Failed to update item."
            );
        }

        /// <summary>
        /// Deletes an item from the database and the UI collection safely.
        /// </summary>
        /// <param name="item">The item to delete.</param>
        protected virtual void DeleteItem(T item)
        {
            SafeExecutor.Execute(
                () =>
                {
                    Dao.Delete(item);
                    Items.Remove(item);
                },
                "Failed to delete item."
            );
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}