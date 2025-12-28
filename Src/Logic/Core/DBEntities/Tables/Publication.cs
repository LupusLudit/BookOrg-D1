using System;

namespace BookOrg.Src.Logic.Core.DBEntities.Tables
{
    public class Publication : IDBTable
    {
        public int ID { get; set; }
        public Book Book { get; set; }
        public Author Author { get; set; }
        public Genre Genre { get; set; }
        public string Note { get; set; }

        private int publicationYear;
        public int PublicationYear
        {
            get => publicationYear;
            set
            {
                if (value < 1450 || value > DateTime.Now.Year)
                {
                    throw new ArgumentException($"Publication year must be between 1450 and {DateTime.Now.Year}.");
                }
                publicationYear = value;
            }
        }

        public Publication(int id, Book book, Author author, Genre genre, string note, int publicationYear)
        {
            ID = id;
            Book = book;
            Author = author;
            Genre = genre;
            Note = note;
            PublicationYear = publicationYear;
        }

        public Publication(Book book, Author author, Genre genre, string note, int publicationYear)
            : this(0, book, author, genre, note, publicationYear) { }
    }
}