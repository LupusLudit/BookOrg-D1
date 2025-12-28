namespace BookOrg.Src.Logic.Core.DBEntities.Tables
{
    public class Genre: IDBTable
    {
        public int ID { get; set; }
        public string GenreName { get; set; }

        public Genre(int id, string genreName)
        {
            ID = id;
            GenreName = genreName;
        }

        public Genre(string genreName)
        {
            ID = 0;
            GenreName = genreName;
        }
    }
}
