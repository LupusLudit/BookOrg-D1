using BookOrg.Src.Logic.Core.DBEntities;
using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Core.DAO
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="GenreDAO"]/*'/>
    public class GenreDAO : DAOBase<Genre>, IDAOImportable<Genre>
    {
        public int ColumnCount => 1;

        public GenreDAO(SqlConnection connection) : base(connection) { }

        /// <summary>
        /// Creates a Genre object from a CSV row.
        /// </summary>
        /// <param name="values">Array containing the genre name.</param>
        /// <returns>A new Genre instance.</returns>
        public Genre FromCsv(string[] values)
        {
            return new Genre(values[0].Trim());
        }

        /// <summary>
        /// Inserts the imported genre into the database.
        /// </summary>
        /// <param name="genre">The genre to import.</param>
        public void ImportEntity(Genre genre)
        {
            Insert(genre);
        }

        /// <summary>
        /// Inserts a new Genre entity into the database.
        /// </summary>
        /// <param name="genre">The Genre entity to be inserted.</param>
        /// <remarks>
        /// To make sure that duplicate genres are not added (unique constraint),
        /// in the query we check if a genre with the same name already exists or not before insertion.
        /// </remarks>
        public override void Insert(Genre genre)
        {
            string insertQuery = @"insert into genre (genre_name)
                                   select @name
                                   where not exists (select 1 from genre where genre_name = @name);
                                   select id from genre where genre_name = @name;";

            SqlCommand insertCommand = CreateCommand(insertQuery);

            insertCommand.Parameters.AddWithValue("@name", genre.GenreName);
            genre.ID = Convert.ToInt32(insertCommand.ExecuteScalar());
        }

        /// <summary>
        /// Updates an existing Genre entity in the database.
        /// </summary>
        /// <param name="genre">The Genre entity to be updated.</param>
        public override void Update(Genre genre)
        {
            SqlCommand updateCommand = CreateCommand("update genre set genre_name = @name where id = @id");

            updateCommand.Parameters.AddWithValue("@name", genre.GenreName);
            updateCommand.Parameters.AddWithValue("@id", genre.ID);
            updateCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes a Genre entity from the database.
        /// </summary>
        /// <param name="genre">The Genre entity to be deleted.</param>
        public override void Delete(Genre genre)
        {
            SqlCommand deleteCommand = CreateCommand("delete from genre where id = @id");

            deleteCommand.Parameters.AddWithValue("@id", genre.ID);
            deleteCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Retrieves all Genre entities from the database.
        /// </summary>
        /// <returns>A list of all Genre objects.</returns>
        public override List<Genre> GetAll()
        {
            List<Genre> genres = new List<Genre>();

            SqlCommand getCommand = CreateCommand("select id, genre_name from genre");

            using (SqlDataReader reader = getCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    genres.Add(new Genre(reader.GetInt32(0), reader.GetString(1)));
                }
            }

            return genres;
        }
    }
}