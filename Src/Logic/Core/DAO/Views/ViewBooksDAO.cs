using BookOrg.Src.Logic.Core.DBEntities.Views;
using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Core.DAO.Views
{
    public class ViewBooksDAO
    {
        private readonly SqlConnection connection;

        public ViewBooksDAO(SqlConnection connection)
        {
            this.connection = connection;
        }

        public List<ViewBooks> GetAllBooks()
        {
            var books = new List<ViewBooks>();
            string query = "SELECT title, price, author_name, genre_name, publication_year, note, is_available FROM view_books";

            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    books.Add(new ViewBooks(
                        reader.GetString(0),
                        reader.GetDecimal(1),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetInt32(4),
                        reader.IsDBNull(5) ? "" : reader.GetString(5),
                        reader.GetBoolean(6)
                    ));
                }
            }
            return books;
        }
    }
}
