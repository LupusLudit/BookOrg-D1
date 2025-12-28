using BookOrg.Src.Logic.Core.DBEntities.Tables;
using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Core.DAO.Tables
{
    public class AuthorDAO : TableDAOBase<Author>
    {
        public AuthorDAO(SqlConnection connection) : base(connection) { }

        public override Author? GetByID(int id)
        {
            SqlCommand command = CreateCommand("SELECT id, author_name FROM author WHERE id = @id");
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read()) return new Author(reader.GetInt32(0), reader.GetString(1));
            return null;
        }

        public override void Insert(Author entity)
        {
            SqlCommand command = CreateCommand("INSERT INTO author (author_name) VALUES (@name); SELECT SCOPE_IDENTITY();");

            command.Parameters.AddWithValue("@name", entity.AuthorName);
            entity.ID = Convert.ToInt32(command.ExecuteScalar());
        }

        public override void Update(Author entity)
        {
            SqlCommand command = CreateCommand("UPDATE author SET author_name = @name WHERE id = @id");

            command.Parameters.AddWithValue("@name", entity.AuthorName);
            command.Parameters.AddWithValue("@id", entity.ID);
            command.ExecuteNonQuery();
        }

        public override void Delete(Author entity)
        {
            SqlCommand command = CreateCommand("DELETE FROM author WHERE id = @id");

            command.Parameters.AddWithValue("@id", entity.ID);
            command.ExecuteNonQuery();
        }
    }
}