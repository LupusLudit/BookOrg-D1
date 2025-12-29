using BookOrg.Src.Logic.Core.DBEntities.Tables;
using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Core.DAO.Tables
{
    public class GenreDAO : TableDAOBase<Genre>
    {
        public GenreDAO(SqlConnection connection) : base(connection) { }

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

        public override void Insert(Genre genre)
        {
            SqlCommand command = CreateCommand("insert into genre (genre_name) values (@name); select scope_identity();");

            command.Parameters.AddWithValue("@name", genre.GenreName);
            genre.ID = Convert.ToInt32(command.ExecuteScalar());
        }

        public override void Update(Genre genre)
        {
            SqlCommand command = CreateCommand("update genre set genre_name = @name where id = @id");

            command.Parameters.AddWithValue("@name", genre.GenreName);
            command.Parameters.AddWithValue("@id", genre.ID);
            command.ExecuteNonQuery();
        }

        public override void Delete(Genre genre)
        {
            SqlCommand command = CreateCommand("delete from genre where id = @id");

            command.Parameters.AddWithValue("@id", genre.ID);
            command.ExecuteNonQuery();
        }

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
