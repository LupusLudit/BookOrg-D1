using BookOrg.Src.Logic.Core.DBEntities;
using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Core.DAO
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="LoanDAO"]/*'/>
    public class LoanDAO : DAOBase<Loan>
    {
        public LoanDAO(SqlConnection connection) : base(connection) { }

        /// <summary>
        /// Inserts a new Loan entity into the database.
        /// </summary>
        /// <param name="loan">The Loan entity to be inserted.</param>
        public override void Insert(Loan loan)
        {
            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    ModifyBookStock(loan.BookID, -1, transaction);

                    string insertQuery = @"insert into loan (book_id, customer_id, total_loan_price, loan_status, loan_start_date, loan_due_date, loan_returned_date) 
                                           values (@bookId, @customerId, @price, @status, @startDate, @dueDate, @returnDate); select scope_identity();";

                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection, transaction);
                    insertCommand.Parameters.AddWithValue("@bookId", loan.BookID);
                    insertCommand.Parameters.AddWithValue("@customerId", loan.CustomerID);
                    insertCommand.Parameters.AddWithValue("@price", loan.TotalLoanPrice);
                    insertCommand.Parameters.AddWithValue("@status", loan.LoanStatus);
                    insertCommand.Parameters.AddWithValue("@startDate", loan.LoanStartDate);
                    insertCommand.Parameters.AddWithValue("@dueDate", loan.LoanDueDate);
                    insertCommand.Parameters.AddWithValue("@returnDate", (object)loan.LoanReturnedDate ?? DBNull.Value);

                    loan.ID = Convert.ToInt32(insertCommand.ExecuteScalar());
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
        /// Updates an existing Loan entity in the database.
        /// </summary>
        /// <param name="loan">The Loan entity to be updated.</param>
        /// <exception cref="System.Exception">Loan not found</exception>
        /// <remarks>
        /// Before updating, the method checks if the book associated with the loan has changed.
        /// If it has, it adjusts the stock of both the old and new books accordingly.
        /// The method also checks if the loan status has changed to "returned".
        /// If yes, it sets the returned date to the current date and increases the book stock by one.
        /// </remarks>
        public override void Update(Loan loan)
        {
            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    SqlCommand getOldCommand = new SqlCommand("select book_id, loan_status from loan where id = @id", connection, transaction);
                    getOldCommand.Parameters.AddWithValue("@id", loan.ID);

                    int oldBookId = 0;
                    string oldStatus;

                    using (SqlDataReader reader = getOldCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            oldBookId = reader.GetInt32(0);
                            oldStatus = reader.GetString(1);
                        }
                        else throw new Exception("Loan not found");
                    }

                    if (oldBookId != loan.BookID)
                    {
                        ModifyBookStock(oldBookId, 1, transaction);
                        ModifyBookStock(loan.BookID, -1, transaction);
                    }

                    if (loan.LoanStatus == "returned" && oldStatus != "returned")
                    {
                        loan.LoanReturnedDate = DateTime.Now;
                        ModifyBookStock(loan.BookID, 1, transaction);
                    }

                    string updateQuery = @"update loan set book_id = @bookId, customer_id = @customerId, loan_status = @status, 
                                           loan_start_date = @startDate, loan_due_date = @dueDate, loan_returned_date = @returnDate, total_loan_price = @price 
                                           where id = @id";

                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection, transaction);
                    updateCommand.Parameters.AddWithValue("@bookId", loan.BookID);
                    updateCommand.Parameters.AddWithValue("@customerId", loan.CustomerID);
                    updateCommand.Parameters.AddWithValue("@status", loan.LoanStatus);
                    updateCommand.Parameters.AddWithValue("@startDate", loan.LoanStartDate);
                    updateCommand.Parameters.AddWithValue("@dueDate", loan.LoanDueDate);
                    updateCommand.Parameters.AddWithValue("@returnDate", (object)loan.LoanReturnedDate ?? DBNull.Value);
                    updateCommand.Parameters.AddWithValue("@price", loan.TotalLoanPrice);
                    updateCommand.Parameters.AddWithValue("@id", loan.ID);
                    updateCommand.ExecuteNonQuery();

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
        /// Deletes a Loan entity from the database.
        /// </summary>
        /// <param name="loan">The Loan entity to be deleted.</param>
        public override void Delete(Loan loan)
        {
            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    ModifyBookStock(loan.BookID, 1, transaction);

                    SqlCommand delCmd = new SqlCommand("delete from loan where id = @id", connection, transaction);
                    delCmd.Parameters.AddWithValue("@id", loan.ID);
                    delCmd.ExecuteNonQuery();

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
        /// Retrieves all (ongoing) loans using a predefined view.
        /// </summary>
        /// <returns>
        /// List of Loan objects representing ongoing loans.
        /// </returns>
        public override List<Loan> GetAll()
        {
            return GetLoansFromView("select * from view_ongoing_loans");
        }

        /// <summary>
        /// Retrieves the returned loans using a predefined view.
        /// </summary>
        /// <returns>
        /// List of Loan objects representing returned loans.
        /// </returns>
        public List<Loan> GetReturnedLoans()
        {
            return GetLoansFromView("select * from view_returned_loans");
        }

        /// <summary>
        /// Centralized method to update book stock and availability.
        /// Validates stock constraints before updating.
        /// </summary>
        /// <param name="bookId">The ID of the book to update.</param>
        /// <param name="quantityChange">The amount to change stock by (negative to decrease, positive to increase).</param>
        /// <param name="transaction">The active transaction.</param>
        private void ModifyBookStock(int bookId, int quantityChange, SqlTransaction transaction)
        {
            if (quantityChange < 0)
            {
                SqlCommand checkCommand = new SqlCommand("select books_in_stock from book where id = @id", connection, transaction);
                checkCommand.Parameters.AddWithValue("@id", bookId);
                int? inStock = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (inStock == null) 
                {
                    throw new InvalidOperationException($"Book with ID {bookId} not found.");
                }
                if (inStock + quantityChange < 0)
                {
                    throw new InvalidOperationException("Insufficient stock available for this book.");
                }
            }

            SqlCommand updateStockCmd = new SqlCommand("update book set books_in_stock = books_in_stock + @change where id = @id", connection, transaction);
            updateStockCmd.Parameters.AddWithValue("@change", quantityChange);
            updateStockCmd.Parameters.AddWithValue("@id", bookId);
            updateStockCmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Gets the specific loan information from a predefined view.
        /// </summary>
        /// <param name="query">The (view) query to be executed.</param>
        /// <returns>
        /// List of Loan objects retrieved from the view.
        /// </returns>
        private List<Loan> GetLoansFromView(string query)
        {
            List<Loan> loans = new();
            SqlCommand command = CreateCommand(query);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    loans.Add(new Loan(
                        reader.GetInt32(0),
                        reader.GetInt32(1),
                        reader.GetString(2),
                        reader.GetInt32(3),
                        reader.GetString(4),
                        reader.GetDecimal(5),
                        reader.GetString(6),
                        reader.GetDateTime(7),
                        reader.GetDateTime(8),
                        reader.IsDBNull(9) ? null : reader.GetDateTime(9)
                    ));
                }
            }
            return loans;
        }

    }
}