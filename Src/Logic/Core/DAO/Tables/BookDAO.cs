using BookOrg.Src.Logic.Core.DBEntities.Tables;
using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Core.DAO.Tables
{
    public class BookDAO : TableDAOBase<Book>
    {
        public BookDAO(SqlConnection connection) : base(connection) { }

        public override Book? GetByID(int id)
        {
            SqlCommand command = CreateCommand("SELECT id, title, is_available, price FROM book WHERE id = @id");

            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Book(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetBoolean(2),
                    reader.GetDecimal(3)
                );
            }
            return null;
        }

        public override void Insert(Book entity)
        {
            SqlCommand command = CreateCommand("INSERT INTO book (title, is_available, price) VALUES (@title, @isAvail, @price); SELECT SCOPE_IDENTITY();");

            command.Parameters.AddWithValue("@title", entity.Title);
            command.Parameters.AddWithValue("@isAvail", entity.IsAvailable);
            command.Parameters.AddWithValue("@price", entity.Price);
            entity.ID = Convert.ToInt32(command.ExecuteScalar());
        }

        public override void Update(Book entity)
        {
            SqlCommand command = CreateCommand("UPDATE book SET title = @title, is_available = @isAvail, price = @price WHERE id = @id");

            command.Parameters.AddWithValue("@title", entity.Title);
            command.Parameters.AddWithValue("@isAvail", entity.IsAvailable);
            command.Parameters.AddWithValue("@price", entity.Price);
            command.Parameters.AddWithValue("@id", entity.ID);
            command.ExecuteNonQuery();
        }

        public override void Delete(Book entity)
        {
            SqlCommand command = CreateCommand("DELETE FROM book WHERE id = @id");

            command.Parameters.AddWithValue("@id", entity.ID);
            command.ExecuteNonQuery();
        }
    }
}