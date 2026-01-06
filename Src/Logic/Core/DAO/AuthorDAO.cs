using BookOrg.Src.Logic.Core.DBEntities;
using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Core.DAO
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="AuthorDAO"]/*'/>
    public class AuthorDAO : DAOBase<Author>, IDAOImportable<Author>
    {
        public int ColumnCount => 1;

        public AuthorDAO(SqlConnection connection) : base(connection) { }

        /// <summary>
        /// Creates an Author object from a CSV row.
        /// </summary>
        /// <param name="values">Array containing the author name.</param>
        /// <returns>A new Author instance.</returns>
        public Author FromCsv(string[] values)
        {
            return new Author(values[0].Trim());
        }

        /// <summary>
        /// Inserts the imported author into the database.
        /// </summary>
        /// <param name="author">The author to import.</param>
        public void ImportEntity(Author author)
        {
            Insert(author);
        }

        /// <summary>
        /// Inserts a new Author entity into the database.
        /// </summary>
        /// <param name="author">The Author entity to be inserted.</param>
        /// <remarks>
        /// To make sure that duplicate authors are not added (unique constraint),
        /// in the query we check if an author with the same name already exists or not before insertion.
        /// </remarks>
        public override void Insert(Author author)
        {
            string insertQuery = @"insert into author (author_name)
                                   select @name
                                   where not exists (select 1 from author where author_name = @name);
                                   select id from author where author_name = @name;";

            SqlCommand insertCommand = CreateCommand(insertQuery);

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