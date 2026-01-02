using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookOrg.Src.Logic.Core.DBEntities
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="Book"]/*'/>
    public class Book : IDBEntity, INotifyPropertyChanged
    {
        private int id;
        private string title;
        private bool isAvailable;
        private int booksInStock;
        private decimal price;

        public int ID
        {
            get => id;
            set
            {
                if (value >= 0)
                {
                    id = value;
                }
            }
        }

        public string Title
        {
            get => title;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    title = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsAvailable
        {
            get => isAvailable;
            set
            {
                if (!(value && booksInStock == 0))
                {
                    isAvailable = value;
                    OnPropertyChanged();
                }
            }
        }

        public int BooksInStock
        {
            get => booksInStock;
            set
            {
                if (value >= 0)
                {

                    booksInStock = value;

                    if (booksInStock == 0 && IsAvailable)
                    {
                        IsAvailable = false;
                    }

                    OnPropertyChanged();
                }
            }
        }

        public decimal Price
        {
            get => price;
            set
            {
                if (value >= 0)
                {
                    price = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Author> Authors { get; set; }
        public ObservableCollection<Genre> Genres { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public Book(int id, string title, bool isAvailable, decimal price, int booksInStock)
        {
            ID = id;
            Title = title;
            BooksInStock = booksInStock;
            IsAvailable = isAvailable;
            Price = price;
            Authors = new ObservableCollection<Author>();
            Genres = new ObservableCollection<Genre>();
        }

        public Book(string title, bool isAvailable, decimal price, int booksInStock)
            : this(0, title, isAvailable, price, booksInStock) { }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}