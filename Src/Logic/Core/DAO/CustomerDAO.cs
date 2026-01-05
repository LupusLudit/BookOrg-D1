using BookOrg.Src.Logic.Core.DBEntities;
using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Core.DAO
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="CustomerDAO"]/*'/>
    public class CustomerDAO : DAOBase<Customer>, IDAOImportable<Customer>
    {
        public int ColumnCount => 3;

        public CustomerDAO(SqlConnection connection) : base(connection) { }

        public Customer FromCsv(string[] values)
        {
            return new Customer(values[0].Trim(), values[1].Trim(), values[2].Trim());
        }

        public void ImportEntity(Customer customer)
        {
            Insert(customer);
        }

        /// <summary>
        /// Inserts a new Customer entity into the database.
        /// </summary>
        /// <param name="customer">The Customer entity to insert.</param>
        public override void Insert(Customer customer)
        {
            SqlCommand insertCommand = CreateCommand("insert into customer (first_name, last_name, email) values (@firstName, @lastName, @email); select scope_identity();");

            insertCommand.Parameters.AddWithValue("@firstName", customer.FirstName);
            insertCommand.Parameters.AddWithValue("@lastName", customer.LastName);
            insertCommand.Parameters.AddWithValue("@email", customer.Email);

            customer.ID = Convert.ToInt32(insertCommand.ExecuteScalar());
        }

        /// <summary>
        /// Updates an existing Customer entity in the database.
        /// </summary>
        /// <param name="customer">The Customer entity to update.</param>
        public override void Update(Customer customer)
        {
            SqlCommand updateCommand = CreateCommand("update customer set first_name = @firstName, last_name = @lastName, email = @email where id = @id");

            updateCommand.Parameters.AddWithValue("@id", customer.ID);
            updateCommand.Parameters.AddWithValue("@firstName", customer.FirstName);
            updateCommand.Parameters.AddWithValue("@lastName", customer.LastName);
            updateCommand.Parameters.AddWithValue("@email", customer.Email);
            updateCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes a Customer entity from the database.
        /// </summary>
        /// <param name="customer">The Customer entity to delete.</param>
        public override void Delete(Customer customer)
        {
            SqlCommand deleteCommand = CreateCommand("delete from customer where id = @id");

            deleteCommand.Parameters.AddWithValue("@id", customer.ID);
            deleteCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Retrieves all Customer entities from the database.
        /// </summary>
        /// <returns>A list of all Customer objects.</returns>
        public override List<Customer> GetAll()
        {
            List<Customer> customers = new List<Customer>();
            SqlCommand getCommand = CreateCommand("select id, first_name, last_name, email from customer");

            using (SqlDataReader reader = getCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    customers.Add(new Customer(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3)
                    ));
                }
            }

            return customers;
        }
    }
}