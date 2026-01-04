using BookOrg.Src.Logic.Core.DBEntities;
using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Core.DAO
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="AuthorDAO"]/*'/>
    public class AuthorDAO : DAOBase<Author>
    {
        public AuthorDAO(SqlConnection connection) : base(connection) { }

        /// <summary>
        /// Inserts a new Author entity into the database.
        /// </summary>
        /// <param name="author">The Author entity to be inserted.</param>
        public override void Insert(Author author)
        {
            SqlCommand insertCommand = CreateCommand("insert into author (author_name) values (@name); select scope_identity();");

            insertCommand.Parameters.AddWithValue("@name", author.AuthorName);
            author.ID = Convert.ToInt32(insertCommand.ExecuteScalar());
        }

        /// <summary>
        /// Updates an existing Author entity in the database.
        /// </summary>
        /// <param name="author">The Author entity to be updated.</param>
        public override void Update(Author author)
        {
            SqlCommand updateCommand = CreateCommand("update author set author_name = @name where id = @id");

            updateCommand.Parameters.AddWithValue("@name", author.AuthorName);
            updateCommand.Parameters.AddWithValue("@id", author.ID);
            updateCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes an Author entity from the database.
        /// </summary>
        /// <param name="author">The Author entity to be deleted.</param>
        public override void Delete(Author author)
        {
            SqlCommand deleteCommand = CreateCommand("delete from author where id = @id");

            deleteCommand.Parameters.AddWithValue("@id", author.ID);
            deleteCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Retrieves all Author entities from the database.
        /// </summary>
        /// <returns>A list of all Author objects.</returns>
        public override List<Author> GetAll()
        {
            List<Author> authors = new List<Author>();

            SqlCommand getCommand = CreateCommand("select id, author_name from author");

            using (SqlDataReader reader = getCommand.ExecuteReader())
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