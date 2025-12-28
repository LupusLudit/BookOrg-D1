using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookOrg.Src.Logic.Core.DBEntities.Views
{
    public class ViewBooks
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int PublicationYear { get; set; }
        public string Note { get; set; }
        public bool IsAvailable { get; set; }

        public ViewBooks(string title, decimal price, string author, string genre, int publicationYear, string note, bool isAvailable)
        {
            Title = title;
            Price = price;
            Author = author;
            Genre = genre;
            PublicationYear = publicationYear;
            Note = note;
            IsAvailable = isAvailable;
        }
    }
}
