using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookOrg.Src.Logic.Core.DBEntities
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="Loan"]/*'/>
    public class Loan : IDBEntity, INotifyPropertyChanged
    {
        private int id;
        private int bookId;
        private int customerId;
        private string bookTitle;
        private string customerName;
        private decimal totalLoanPrice;
        private string loanStatus;
        private DateTime loanStartDate;
        private DateTime loanDueDate;
        private DateTime? loanReturnedDate;

        public int ID
        {
            get => id;
            set 
            {
                if (value >= 0)
                {
                    id = value;
                    OnPropertyChanged();
                }
            }
        }

        public int BookID
        {
            get => bookId;
            set
            {
                if (value >= 0)
                {
                    bookId = value;
                    OnPropertyChanged();
                }
            }
        }

        public int CustomerID
        {
            get => customerId;
            set
            {
                if (value >= 0)
                {
                    customerId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string BookTitle
        {
            get => bookTitle;
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 100)
                {
                    bookTitle = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CustomerName
        {
            get => customerName;
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 100)
                {
                    customerName = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal TotalLoanPrice
        {
            get => totalLoanPrice;
            set
            {
                if (value >= 0)
                {
                    totalLoanPrice = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LoanStatus
        {
            get => loanStatus;
            set
            {
                if (loanStatus != value && value.Length <= 10)
                {
                    loanStatus = value;

                    if (loanStatus == "overdue" && LoanDueDate >= DateTime.Now)
                    {
                        LoanDueDate = DateTime.Now.AddDays(-1);
                    }
                    OnPropertyChanged();
                }
            }
        }

        public DateTime LoanStartDate
        {
            get => loanStartDate;
            set
            {
                loanStartDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime LoanDueDate
        {
            get => loanDueDate;
            set
            {
                if (loanDueDate != value)
                {
                    loanDueDate = value;

                    if (loanDueDate < DateTime.Now && LoanStatus == "active")
                    {
                        LoanStatus = "overdue";
                    }
                    else if (loanDueDate >= DateTime.Now && LoanStatus == "overdue")
                    {
                        LoanStatus = "active";
                    }

                    OnPropertyChanged();
                }
            }
        }

        public DateTime? LoanReturnedDate
        {
            get => loanReturnedDate;
            set 
            {
                loanReturnedDate = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public Loan(int id, int bookId, string bookTitle, int customerId, string customerName, decimal price, string status, DateTime start, DateTime due, DateTime? returned)
        {
            ID = id;
            BookID = bookId;
            BookTitle = bookTitle;
            CustomerID = customerId;
            CustomerName = customerName;
            TotalLoanPrice = price;
            LoanStatus = status;
            LoanStartDate = start;
            LoanDueDate = due;
            LoanReturnedDate = returned;
        }

        public Loan(int bookId, string bookTitle, int customerId, string customerName, decimal price, string status, DateTime start, DateTime due)
            : this(0, bookId, bookTitle, customerId, customerName, price, status, start, due, null) { }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}