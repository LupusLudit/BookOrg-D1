using System.Text.RegularExpressions;

namespace BookOrg.Src.Logic.Core.DBEntities
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="Customer"]/*'/>
    public class Customer : IDBEntity
    {
        private int id;
        private string firstName;
        private string lastName;
        private string email;
        public int ID
        {
            get => id;
            set
            {
                if (value >= 0)
                {
                    id = value;
                }
            }
        }

        public string FirstName
        {
            get => firstName;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    firstName = value;
                }
            }
        }

        public string LastName
        {
            get => lastName;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    lastName = value;
                }
            }
        }

        public string Email
        {
            get => email;
            set
            {
                if (!string.IsNullOrEmpty(value) && Regex.IsMatch(value, @"^[a-zA-Z0-9._+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                {
                    email = value;
                }
            }
        }

        public Customer(int id, string firstName, string lastName, string email)
        {
            ID = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }

        public Customer(string firstName, string lastName, string email)
            : this(0, firstName, lastName, email) { }
    }
}
