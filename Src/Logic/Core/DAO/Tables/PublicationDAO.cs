using BookOrg.Src.Logic.Core.DBEntities.Tables;
using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Core.DAO.Tables
{
    public class PublicationDAO : TableDAOBase<Publication>
    {
        public PublicationDAO(SqlConnection connection) : base(connection) { }

        public override Publication? GetByID(int id)
        {
            SqlCommand command = CreateCommand("SELECT id, book_id, author_id, genre_id, note, publication_year FROM publication WHERE id = @id");
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var book = new Book(reader.GetInt32(1), "", false, 0);
                var author = new Author(reader.GetInt32(2), "");
                var genre = new Genre(reader.GetInt32(3), "");

                return new Publication(
                    reader.GetInt32(0),
                    book, author, genre,
                    reader.IsDBNull(4) ? "" : reader.GetString(4),
                    reader.GetInt32(5)
                );
            }
            return null;
        }

        public override void Insert(Publication entity)
        {
            SqlCommand command = CreateCommand(@"INSERT INTO publication (book_id, author_id, genre_id, note, publication_year) 
                                                VALUES (@bookId, @authorId, @genreId, @note, @year); SELECT SCOPE_IDENTITY();");

            command.Parameters.AddWithValue("@bookId", entity.Book.ID);
            command.Parameters.AddWithValue("@authorId", entity.Author.ID);
            command.Parameters.AddWithValue("@genreId", entity.Genre.ID);
            command.Parameters.AddWithValue("@note", (object)entity.Note ?? DBNull.Value);
            command.Parameters.AddWithValue("@year", entity.PublicationYear);
            entity.ID = Convert.ToInt32(command.ExecuteScalar());
        }

        public override void Update(Publication entity)
        {
            SqlCommand command = CreateCommand(@"UPDATE publication SET book_id=@bookId, author_id=@authorId, genre_id=@genreId, note=@note, publication_year=@year WHERE id=@id");

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
            SqlCommand command = CreateCommand("DELETE FROM publication WHERE id = @id");
            command.Parameters.AddWithValue("@id", entity.ID);
            command.ExecuteNonQuery();
        }
    }
}