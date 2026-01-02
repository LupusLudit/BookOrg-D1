namespace BookOrg.Src.Logic.Core.DBEntities
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="Author"]/*'/>
    public class Author : IDBEntity
    {
        private int id;
        private string authorName;

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
        public string AuthorName
        {
            get => authorName;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    authorName = value;
                }
            }
        }

        public Author(int id, string authorName)
        {
            ID = id;
            AuthorName = authorName;
        }

        public Author(string authorName): this(0, authorName) { }
    }
}
