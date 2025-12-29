using BookOrg.Src.Logic.Core.DBEntities.Tables;
using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Core.DAO.Tables
{
    public class PublicationDAO : TableDAOBase<Publication>
    {
        public PublicationDAO(SqlConnection connection) : base(connection) { }

        public override Publication? GetByID(int id)
        {
            SqlCommand command = CreateCommand("select id, book_id, author_id, genre_id, note, publication_year from publication where id = @id");
            command.Parameters.AddWithValue("@id", id);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    Book book = new Book(reader.GetInt32(1), "", false, 0);
                    Author author = new Author(reader.GetInt32(2), "");
                    Genre genre = new Genre(reader.GetInt32(3), "");

                    return new Publication(
                        reader.GetInt32(0),
                        book, author, genre,
                        reader.IsDBNull(4) ? "" : reader.GetString(4),
                        reader.GetInt32(5)
                    );
                }
            }
            return null;
        }

        public override void Insert(Publication entity)
        {
            string insertQuery = @"
                insert into publication (book_id, author_id, genre_id, note, publication_year)
                values (@bookId, @authorId, @genreId, @note, @year);
                select scope_identity();";

            SqlCommand command = CreateCommand(insertQuery);

            command.Parameters.AddWithValue("@bookId", entity.Book.ID);
            command.Parameters.AddWithValue("@authorId", entity.Author.ID);
            command.Parameters.AddWithValue("@genreId", entity.Genre.ID);
            command.Parameters.AddWithValue("@note", (object)entity.Note ?? DBNull.Value);
            command.Parameters.AddWithValue("@year", entity.PublicationYear);
            entity.ID = Convert.ToInt32(command.ExecuteScalar());
        }

        public override void Update(Publication entity)
        {
            string updateQuery = @"
                update publication
                set book_id = @bookId,author_id = @authorId, genre_id = @genreId, note = @note, publication_year = @year
                where id = @id";

            SqlCommand command = CreateCommand(updateQuery);

            command.Parameters.AddWithValue("@bookId", entity.Book.ID);
            command.Parameters.AddWithValue("@authorId", entity.Author.ID);
            command.Parameters.AddWithValue("@genreId", entity.Genre.ID);
            command.Parameters.AddWithValue("@note", (object)entity.Note ?? DBNull.Value);
            command.Parameters.AddWithValue("@year", entity.PublicationYear);
            command.Parameters.AddWithValue("@id", entity.ID);
            command.ExecuteNonQuery();
        }

        public override void Delete(Publication entity)
        {
            SqlCommand command = CreateCommand("delete from publication where id = @id");

            command.Parameters.AddWithValue("@id", entity.ID);
            command.ExecuteNonQuery();
        }

        public override List<Publication> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
