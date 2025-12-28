using System;

namespace BookOrg.Src.Logic.Core.DBEntities.Tables
{
    public class Book : IDBTable
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public bool IsAvailable { get; set; }

        private decimal price;
        public decimal Price
        {
            get => price;
            set
            {
                if (value < 0)
                { 
                    throw new ArgumentException("Price cannot be negative.");
                }
                price = value;
            }
        }

        public Book(int id, string title, bool isAvailable, decimal price)
        {
            ID = id;
            Title = title;
            IsAvailable = isAvailable;
            Price = price;
        }

        public Book(string title, bool isAvailable, decimal price)
            : this(0, title, isAvailable, price) { }
    }
}