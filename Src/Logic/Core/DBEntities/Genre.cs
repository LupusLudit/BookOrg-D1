namespace BookOrg.Src.Logic.Core.DBEntities
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="Genre"]/*'/>
    public class Genre: IDBEntity
    {
        private int id;
        private string genreName;

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
        public string GenreName
        {
            get => genreName;
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 50)
                {
                    genreName = value;
                }
            }
        }

        public Genre(int id, string genreName)
        {
            ID = id;
            GenreName = genreName;
        }

        public Genre(string genreName) : this(0, genreName) { }
    }
}
