using BookOrg.Src.Logic.Core.DBEntities;
using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Core.DAO
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="AuthorDAO"]/*'/>
    public class AuthorDAO : DAOBase<Author>
    {
        public AuthorDAO(SqlConnection connection) : base(connection) { }

        /// <summary>
        /// Retrieves an Author entity by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the author.</param>
        /// <returns>The found Author object, or null if not found.</returns>
        public override Author? GetByID(int id)
        {
            SqlCommand command = CreateCommand("select id, author_name from author where id = @id");
            command.Parameters.AddWithValue("@id", id);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Author(reader.GetInt32(0), reader.GetString(1));
                }
            }
            return null;
        }

        /// <summary>
        /// Inserts a new Author entity into the database.
        /// </summary>
        /// <param name="author">The Author entity to insert.</param>
        public override void Insert(Author author)
        {
            SqlCommand command = CreateCommand("insert into author (author_name) values (@name); select scope_identity();");

            command.Parameters.AddWithValue("@name", author.AuthorName);
            author.ID = Convert.ToInt32(command.ExecuteScalar());
        }

        /// <summary>
        /// Updates an existing Author entity in the database.
        /// </summary>
        /// <param name="author">The Author entity to update.</param>
        public override void Update(Author author)
        {
            SqlCommand command = CreateCommand("update author set author_name = @name where id = @id");

            command.Parameters.AddWithValue("@name", author.AuthorName);
            command.Parameters.AddWithValue("@id", author.ID);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes an Author entity from the database.
        /// </summary>
        /// <param name="author">The Author entity to delete.</param>
        public override void Delete(Author author)
        {
            SqlCommand command = CreateCommand("delete from author where id = @id");

            command.Parameters.AddWithValue("@id", author.ID);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Retrieves all Author entities from the database.
        /// </summary>
        /// <returns>A list of all Author objects.</returns>
        public override List<Author> GetAll()
        {
            List<Author> authors = new List<Author>();

            SqlCommand command = CreateCommand("select id, author_name from author");

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    authors.Add(new Author(reader.GetInt32(0), reader.GetString(1)));
                }
            }

            return authors;
        }
    }
}