using BookOrg.Src.Logic.Core.DBEntities.Tables;
using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Core.DAO.Tables
{
    public class AuthorDAO : TableDAOBase<Author>
    {
        public AuthorDAO(SqlConnection connection) : base(connection) { }

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

        public override void Insert(Author entity)
        {
            SqlCommand command = CreateCommand("insert into author (author_name) values (@name); select scope_identity();");

            command.Parameters.AddWithValue("@name", entity.AuthorName);
            entity.ID = Convert.ToInt32(command.ExecuteScalar());
        }

        public override void Update(Author entity)
        {
            SqlCommand command = CreateCommand("update author set author_name = @name where id = @id");

            command.Parameters.AddWithValue("@name", entity.AuthorName);
            command.Parameters.AddWithValue("@id", entity.ID);
            command.ExecuteNonQuery();
        }

        public override void Delete(Author entity)
        {
            SqlCommand command = CreateCommand("delete from author where id = @id");

            command.Parameters.AddWithValue("@id", entity.ID);
            command.ExecuteNonQuery();
        }

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
