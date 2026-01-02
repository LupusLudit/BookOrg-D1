using BookOrg.Src.Logic.Core.DBEntities;
using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Core.DAO
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="BookDAO"]/*'/>
    public class BookDAO : DAOBase<Book>
    {
        public BookDAO(SqlConnection connection) : base(connection) { }

        /// <summary>
        /// Inserts a new Book entity into the database, including its relations (Authors and Genres).
        /// </summary>
        /// <param name="book">The Book entity to insert.</param>
        public override void Insert(Book book)
        {
            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    string insertQuery = "insert into book (title, is_available, price, books_in_stock) values (@title, @isAvailable, @price, @stock); select scope_identity();";

                    SqlCommand command = new SqlCommand(insertQuery, connection, transaction);
                    command.Parameters.AddWithValue("@title", book.Title);
                    command.Parameters.AddWithValue("@isAvailable", book.IsAvailable);
                    command.Parameters.AddWithValue("@price", book.Price);
                    command.Parameters.AddWithValue("@stock", book.BooksInStock);
                    book.ID = Convert.ToInt32(command.ExecuteScalar());

                    SyncRelations(book, transaction);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Updates an existing Book entity in the database, including its relations.
        /// </summary>
        /// <param name="book">The Book entity to update.</param>
        public override void Update(Book book)
        {
            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    string updateQuery = "update book set title = @title, is_available = @isAvailable, price = @price, books_in_stock = @stock where id = @id";

                    SqlCommand command = new SqlCommand(updateQuery, connection, transaction);
                    command.Parameters.AddWithValue("@title", book.Title);
                    command.Parameters.AddWithValue("@isAvailable", book.IsAvailable);
                    command.Parameters.AddWithValue("@price", book.Price);
                    command.Parameters.AddWithValue("@stock", book.BooksInStock);
                    command.Parameters.AddWithValue("@id", book.ID);
                    command.ExecuteNonQuery();

                    new SqlCommand($"delete from contribution where book_id = {book.ID}", connection, transaction).ExecuteNonQuery();
                    new SqlCommand($"delete from classification where book_id = {book.ID}", connection, transaction).ExecuteNonQuery();

                    SyncRelations(book, transaction);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Synchronizes the many-to-many relations (Authors and Genres) for a book.
        /// </summary>
        /// <param name="book">The Book entity containing the relations.</param>
        /// <param name="transaction">The active SQL transaction.</param>
        private void SyncRelations(Book book, SqlTransaction transaction)
        {
            foreach (var author in book.Authors)
            {
                SqlCommand command = new SqlCommand("insert into contribution (book_id, author_id) values (@bookID, @authorID)", connection, transaction);
                command.Parameters.AddWithValue("@bookID", book.ID);
                command.Parameters.AddWithValue("@authorID", author.ID);
                command.ExecuteNonQuery();
            }
            foreach (var genre in book.Genres)
            {
                SqlCommand command = new SqlCommand("insert into classification (book_id, genre_id) values (@bookID, @genreID)", connection, transaction);
                command.Parameters.AddWithValue("@bookID", book.ID);
                command.Parameters.AddWithValue("@genreID", genre.ID);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Retrieves all Book entities from the database, including their relations.
        /// </summary>
        /// <returns>A list of Book objects.</returns>
        public override List<Book> GetAll()
        {
            List<Book> books = new List<Book>();
            SqlCommand command = CreateCommand("select id, title, is_available, price, books_in_stock from book");
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    books.Add(new Book(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetBoolean(2),
                        reader.GetDecimal(3),
                        reader.GetInt32(4))
                    );
                }
            }
            foreach (Book book in books)
            {
                LoadRelations(book);
            }
            return books;
        }

        /// <summary>
        /// Loads the authors and genres for a specific book.
        /// </summary>
        /// <param name="book">The Book entity to populate.</param>
        private void LoadRelations(Book book)
        {
            string loadAuthorsQuery = @$"
                                       select author.id, author.author_name
                                       from author
                                       inner join contribution on author.id = contribution.author_id
                                       where contribution.book_id = {book.ID}";

            SqlCommand authorCommand = CreateCommand(loadAuthorsQuery);
            using (SqlDataReader reader = authorCommand.ExecuteReader())
            {
                while (reader.Read()) book.Authors.Add(new Author(reader.GetInt32(0), reader.GetString(1)));
            }

            string loadGenresQuery = @$"
                                       select genre.id, genre.genre_name
                                       from genre
                                       inner join classification on genre.id = classification.genre_id
                                       where classification.book_id = {book.ID}";

            SqlCommand genreCommand = CreateCommand(loadGenresQuery);
            using (SqlDataReader reader = genreCommand.ExecuteReader())
            {
                while (reader.Read()) book.Genres.Add(new Genre(reader.GetInt32(0), reader.GetString(1)));
            }
        }

        /// <summary>
        /// Deletes a Book entity from the database.
        /// </summary>
        /// <param name="book">The Book entity to delete.</param>
        public override void Delete(Book book)
        {
            SqlCommand command = CreateCommand("delete from book where id = @id");
            command.Parameters.AddWithValue("@id", book.ID);
            command.ExecuteNonQuery();
        }

        public override Book? GetByID(int id) => throw new NotImplementedException();
    }
}