using BookOrg.Src.Logic.Core.DAO;
using BookOrg.Src.Logic.Core.DBEntities;
using BookOrg.Src.Safety;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace BookOrg.Src.UI.DBInteraction.Controls.TableInteraction
{
    /// <include file='../../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="LoanControl"]/*'/>
    public partial class LoanControl : EntityControlBase<Loan>
    {
        private BookDAO bookDAO;
        private CustomerDAO customerDAO;

        public ObservableCollection<Loan> ReturnedItems { get; set; } = new ObservableCollection<Loan>();
        public ObservableCollection<Book> AvailableBooks { get; set; } = new ObservableCollection<Book>();
        public ObservableCollection<Customer> AllCustomers { get; set; } = new ObservableCollection<Customer>();

        public LoanControl(SqlConnection connection)
        {
            InitializeComponent();
            Dao = new LoanDAO(connection);
            bookDAO = new BookDAO(connection);
            customerDAO = new CustomerDAO(connection);

            Loaded += (sender, e) => LoadData();
        }

        /// <summary>
        /// Loads ongoing loans, returned loans, customer list, and available book list from the database safely.
        /// </summary>
        public override void LoadData()
        {
            SafeExecutor.Execute(
                () => {
                    List<Loan> ongoing = Dao.GetAll();
                    Items.Clear();
                    foreach (var loan in ongoing)
                    {
                        Items.Add(loan);
                    }

                    List<Loan> returned = ((LoanDAO)Dao).GetReturnedLoans();
                    ReturnedItems.Clear();
                    foreach (var loan in returned)
                    {
                        ReturnedItems.Add(loan);
                    }

                    List<Customer> customers = customerDAO.GetAll();
                    AllCustomers.Clear();
                    foreach (var customer in customers)
                    {
                        AllCustomers.Add(customer);
                    }

                    List<Book> books = bookDAO.GetAll();
                    AvailableBooks.Clear();
                    foreach (var book in books)
                    {
                        AvailableBooks.Add(book);
                    }
                },
                "Failed to load loan data."
            );
        }

        /// <summary>
        /// Initiates the creation of a new loan. 
        /// Automatically selects the first available book and customer to populate the initial loan record.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AddNewLoanClick(object sender, RoutedEventArgs e)
        {
            Book? firstBook = AvailableBooks.FirstOrDefault(b => b.BooksInStock > 0 && b.IsAvailable);
            Customer? firstCustomer = AllCustomers.FirstOrDefault();

            if (firstBook == null)
            {
                MessageBox.Show("No books are currently available in stock to loan.", "Cannot Create Loan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (firstCustomer == null)
            {
                MessageBox.Show("No customers found. Please add a customer first.", "Cannot Create Loan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Loan newLoan = new Loan(
                firstBook.ID,
                firstBook.Title,
                firstCustomer.ID,
                $"{firstCustomer.FirstName} {firstCustomer.LastName}",
                firstBook.Price,
                "active",
                DateTime.Now,
                DateTime.Now.AddDays(14)
             );

            AddItem(newLoan);
            LoadData();
        }

        private void BookSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is Book selectedBook && comboBox.DataContext is Loan loan)
            {
                loan.BookTitle = selectedBook.Title;
                loan.TotalLoanPrice = selectedBook.Price;
            }
        }

        private void CustomerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is Customer selectedCustomer && comboBox.DataContext is Loan loan)
            {
                loan.CustomerName = $"{selectedCustomer.FirstName} {selectedCustomer.LastName}";
            }
        }

        private void DataGridCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit && e.Row.Item is Loan loan)
            {
                if (loan.LoanDueDate <= loan.LoanStartDate)
                {
                    MessageBox.Show("The Due Date must be later than the Start Date.", "Invalid Date", MessageBoxButton.OK, MessageBoxImage.Warning);
                    LoadData();
                    return;
                }

                if (loan.LoanStatus == "returned")
                {
                    DateTime returnedDate = loan.LoanReturnedDate ?? DateTime.Now;

                    if (returnedDate < loan.LoanStartDate)
                    {
                        MessageBox.Show("The book cannot be returned before the loan start date.", "Invalid Status For Date", MessageBoxButton.OK, MessageBoxImage.Warning);
                        LoadData();
                        return;
                    }
                }

                UpdateItem(loan);
                if (loan.LoanStatus == "returned")
                {
                    LoadData();
                }
            }
        }

        private void DeleteLoanClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Loan loan)
            {
                MessageBoxResult result = MessageBox.Show($"Do you really want to delete the loan record for '{loan.BookTitle}'?" +
                    $" This action cannot be undone.", "Confirm Delete", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteItem(loan);
                    LoadData();
                }
            }
        }
    }
}