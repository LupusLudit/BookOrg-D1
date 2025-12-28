namespace BookOrg.Src.Logic.Core.DBEntities.Tables
{
    public class Author : IDBTable
    {
        public int ID { get; set; }
        public string AuthorName { get; set; }

        public Author(int id, string authorName)
        {
            ID = id;
            AuthorName = authorName;
        }

        public Author(string authorName)
        {
            ID = 0;
            AuthorName = authorName;
        }
    }
}
