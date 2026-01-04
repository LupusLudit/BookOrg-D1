using BookOrg.Src.Logic.Core.DBEntities;
using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Core.DAO
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="BookDAO"]/*'/>
    public class BookDAO : DAOBase<Book>
    {
        public BookDAO(SqlConnection connection) : base(connection) { }

        public override void Insert(Book book)
        {
            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    string insertQuery = "insert into book (title, is_available, price, books_in_stock) values (@title, @isAvailable, @price, @stock); select scope_identity();";

                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection, transaction);
                    insertCommand.Parameters.AddWithValue("@title", book.Title);
                    insertCommand.Parameters.AddWithValue("@isAvailable", book.IsAvailable);
                    insertCommand.Parameters.AddWithValue("@price", book.Price);
                    insertCommand.Parameters.AddWithValue("@stock", book.BooksInStock);
                    book.ID = Convert.ToInt32(insertCommand.ExecuteScalar());

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

        public override void Update(Book book)
        {
            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    string updateQuery = "update book set title = @title, is_available = @isAvailable, price = @price, books_in_stock = @stock where id = @id";

                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection, transaction);
                    updateCommand.Parameters.AddWithValue("@title", book.Title);
                    updateCommand.Parameters.AddWithValue("@isAvailable", book.IsAvailable);
                    updateCommand.Parameters.AddWithValue("@price", book.Price);
                    updateCommand.Parameters.AddWithValue("@stock", book.BooksInStock);
                    updateCommand.Parameters.AddWithValue("@id", book.ID);
                    updateCommand.ExecuteNonQuery();

                    ClearRelations(book.ID, transaction);
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

        public override void Delete(Book book)
        {
            SqlCommand deleteCommand = CreateCommand("delete from book where id = @id");
            deleteCommand.Parameters.AddWithValue("@id", book.ID);
            deleteCommand.ExecuteNonQuery();
        }

        public override List<Book> GetAll()
        {
            List<Book> books = new List<Book>();
            SqlCommand getCommand = CreateCommand("select id, title, is_available, price, books_in_stock from book");
            using (SqlDataReader reader = getCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    books.Add(new Book(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetBoolean(2),
                        reader.GetDecimal(3),
                        reader.GetInt32(4)
                    ));
                }
            }
            foreach (Book book in books)
            {
                LoadRelations(book);
            }
            return books;
        }

        private void LoadRelations(Book book)
        {
            string authorQuery = @"select author.id, author.author_name
                                   from author
                                   inner join contribution on author.id = contribution.author_id
                                   where contribution.book_id = @bookId";

            SqlCommand authorCommand = CreateCommand(authorQuery);
            authorCommand.Parameters.AddWithValue("@bookId", book.ID);

            using (SqlDataReader reader = authorCommand.ExecuteReader())
            {
                while (reader.Read()) book.Authors.Add(new Author(reader.GetInt32(0), reader.GetString(1)));
            }

            string genreQuery = @"select genre.id, genre.genre_name
                                  from genre
                                  inner join classification on genre.id = classification.genre_id
                                  where classification.book_id = @bookId";

            SqlCommand genreCommand = CreateCommand(genreQuery);
            genreCommand.Parameters.AddWithValue("@bookId", book.ID);

            using (SqlDataReader reader = genreCommand.ExecuteReader())
            {
                while (reader.Read()) book.Genres.Add(new Genre(reader.GetInt32(0), reader.GetString(1)));
            }
        }

        /// <summary>
        /// Clears existing relations (authors and genres) for a book during an update.
        /// </summary>
        /// <param name="bookId">The book ID.</param>
        /// <param name="transaction">The transaction to which the commands will be added.</param>
        private void ClearRelations(int bookId, SqlTransaction transaction)
        {
            SqlCommand deleteContributionCommand = new SqlCommand("delete from contribution where book_id = @bookId", connection, transaction);
            deleteContributionCommand.Parameters.AddWithValue("@bookId", bookId);
            deleteContributionCommand.ExecuteNonQuery();

            SqlCommand deleteClassificationCommand = new SqlCommand("delete from classification where book_id = @bookId", connection, transaction);
            deleteClassificationCommand.Parameters.AddWithValue("@bookId", bookId);
            deleteClassificationCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Synchronizes the many-to-many relations (Authors and Genres) for a book.
        /// </summary>
        /// <param name="book">The book to be synchronized.</param>
        /// <param name="transaction">The transaction to which the commands will be added.</param>
        private void SyncRelations(Book book, SqlTransaction transaction)
        {
            if (book.Authors.Count > 0)
            {
                string authorQuery = "insert into contribution (book_id, author_id) values (@bookId, @authorId)";
                foreach (Author author in book.Authors)
                {
                    SqlCommand authorCommand = new SqlCommand(authorQuery, connection, transaction);
                    authorCommand.Parameters.AddWithValue("@bookId", book.ID);
                    authorCommand.Parameters.AddWithValue("@authorId", author.ID);
                    authorCommand.ExecuteNonQuery();
                }
            }

            if (book.Genres.Count > 0)
            {
                string genreQuery = "insert into classification (book_id, genre_id) values (@bookId, @genreId)";
                foreach (Genre genre in book.Genres)
                {
                    SqlCommand genreCommand = new SqlCommand(genreQuery, connection, transaction);
                    genreCommand.Parameters.AddWithValue("@bookId", book.ID);
                    genreCommand.Parameters.AddWithValue("@genreId", genre.ID);
                    genreCommand.ExecuteNonQuery();
                }
            }
        }
    }
}