using BookOrg.Src.Logic.Core.DBEntities;
using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Core.DAO
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="GenreDAO"]/*'/>
    public class GenreDAO : DAOBase<Genre>
    {
        public GenreDAO(SqlConnection connection) : base(connection) { }

        /// <summary>
        /// Retrieves a Genre entity by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the genre.</param>
        /// <returns>The found Genre object, or null if not found.</returns>
        public override Genre? GetByID(int id)
        {
            SqlCommand command = CreateCommand("select id, genre_name from genre where id = @id");
            command.Parameters.AddWithValue("@id", id);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Genre(reader.GetInt32(0), reader.GetString(1));
                }
            }
            return null;
        }

        /// <summary>
        /// Inserts a new Genre entity into the database.
        /// </summary>
        /// <param name="genre">The Genre entity to insert.</param>
        public override void Insert(Genre genre)
        {
            SqlCommand command = CreateCommand("insert into genre (genre_name) values (@name); select scope_identity();");

            command.Parameters.AddWithValue("@name", genre.GenreName);
            genre.ID = Convert.ToInt32(command.ExecuteScalar());
        }

        /// <summary>
        /// Updates an existing Genre entity in the database.
        /// </summary>
        /// <param name="genre">The Genre entity to update.</param>
        public override void Update(Genre genre)
        {
            SqlCommand command = CreateCommand("update genre set genre_name = @name where id = @id");

            command.Parameters.AddWithValue("@name", genre.GenreName);
            command.Parameters.AddWithValue("@id", genre.ID);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes a Genre entity from the database.
        /// </summary>
        /// <param name="genre">The Genre entity to delete.</param>
        public override void Delete(Genre genre)
        {
            SqlCommand command = CreateCommand("delete from genre where id = @id");

            command.Parameters.AddWithValue("@id", genre.ID);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Retrieves all Genre entities from the database.
        /// </summary>
        /// <returns>A list of all Genre objects.</returns>
        public override List<Genre> GetAll()
        {
            List<Genre> genres = new List<Genre>();

            SqlCommand command = CreateCommand("select id, genre_name from genre");

            using (SqlDataReader reader = command.ExecuteReader())
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