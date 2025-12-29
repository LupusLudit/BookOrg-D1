using BookOrg.Src.Logic.Core.DBEntities.Tables;
using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Core.DAO.Tables
{
    public class BookDAO : TableDAOBase<Book>
    {
        public BookDAO(SqlConnection connection) : base(connection) { }

        public override Book? GetByID(int id)
        {
            SqlCommand command = CreateCommand("select id, title, is_available, price from book where id = @id");

            command.Parameters.AddWithValue("@id", id);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Book(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetBoolean(2),
                        reader.GetDecimal(3)
                    );
                }
            }
            return null;
        }

        public override void Insert(Book entity)
        {
            SqlCommand command = CreateCommand("insert into book (title, is_available, price) values (@title, @isAvailable, @price); select scope_identity();");

            command.Parameters.AddWithValue("@title", entity.Title);
            command.Parameters.AddWithValue("@isAvailable", entity.IsAvailable);
            command.Parameters.AddWithValue("@price", entity.Price);
            entity.ID = Convert.ToInt32(command.ExecuteScalar());
        }

        public override void Update(Book entity)
        {
            SqlCommand command = CreateCommand("update book set title = @title, is_available = @isAvailable, price = @price where id = @id");

            command.Parameters.AddWithValue("@title", entity.Title);
            command.Parameters.AddWithValue("@isAvailable", entity.IsAvailable);
            command.Parameters.AddWithValue("@price", entity.Price);
            command.Parameters.AddWithValue("@id", entity.ID);
            command.ExecuteNonQuery();
        }

        public override void Delete(Book entity)
        {
            SqlCommand command = CreateCommand("delete from book where id = @id");

            command.Parameters.AddWithValue("@id", entity.ID);
            command.ExecuteNonQuery();
        }

        public override List<Book> GetAll()
        {
            List<Book> books = new List<Book>();

            SqlCommand command = CreateCommand("select id, title, is_available, price from book");

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    books.Add(new Book(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetBoolean(2),
                        reader.GetDecimal(3)
                    ));
                }
            }

            return books;
        }
    }
}
