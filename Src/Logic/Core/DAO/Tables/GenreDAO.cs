using BookOrg.Src.Logic.Core.DBEntities.Tables;
using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Core.DAO.Tables
{
    public class GenreDAO : TableDAOBase<Genre>
    {
        public GenreDAO(SqlConnection connection) : base(connection) { }

        public override Genre? GetByID(int id)
        {
            SqlCommand command = CreateCommand("SELECT id, genre_name FROM genre WHERE id = @id");
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Genre(reader.GetInt32(0), reader.GetString(1));
            }
            return null;
        }

        public override void Insert(Genre entity)
        {
            SqlCommand command = CreateCommand("INSERT INTO genre (genre_name) VALUES (@name); SELECT SCOPE_IDENTITY();");

            command.Parameters.AddWithValue("@name", entity.GenreName);
            entity.ID = Convert.ToInt32(command.ExecuteScalar());
        }

        public override void Update(Genre entity)
        {
            SqlCommand command = CreateCommand("UPDATE genre SET genre_name = @name WHERE id = @id");

            command.Parameters.AddWithValue("@name", entity.GenreName);
            command.Parameters.AddWithValue("@id", entity.ID);
            command.ExecuteNonQuery();
        }

        public override void Delete(Genre entity)
        {
            SqlCommand command = CreateCommand("DELETE FROM genre WHERE id = @id");

            command.Parameters.AddWithValue("@id", entity.ID);
            command.ExecuteNonQuery();
        }
    }
}