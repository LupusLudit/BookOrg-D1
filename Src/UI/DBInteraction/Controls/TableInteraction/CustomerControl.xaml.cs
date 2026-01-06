using BookOrg.Src.Logic.Core.DAO;
using BookOrg.Src.Logic.Core.DBEntities;
using BookOrg.Src.Logic.Importing;
using BookOrg.Src.Safety;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace BookOrg.Src.UI.DBInteraction.Controls.TableInteraction
{
    /// <include file='../../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="CustomerControl"]/*'/>
    public partial class CustomerControl : EntityControlBase<Customer>
    {
        public CustomerControl(SqlConnection connection)
        {
            InitializeComponent();
            Dao = new CustomerDAO(connection);

            Loaded += (sender, e) => LoadData();
        }

        private void AddNewCustomerClick(object sender, RoutedEventArgs e) => AddItem(new Customer("First Name", "Last Name", "name@email.com"));

        private void DataGridCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit && e.Row.Item is Customer editedCustomer)
            {
                UpdateItem(editedCustomer);
            }
        }

        private void DeleteCustomerClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Customer customer)
            {
                MessageBoxResult result = MessageBox.Show($"Do your really want to delete customer '{customer.FirstName} {customer.LastName}'?" +
                    $" THIS WILL AFFECT DIFERENT DATABASE ENTITES WHICH RELAY ON THIS CUSTOMER. This action cannot be undone.", "Confirm", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteItem(customer);
                }
            }
        }

        /// <summary>
        /// Handles the customer data import from CSV file when the Import button is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ImportClick(object sender, RoutedEventArgs e)
        {
            SafeExecutor.Execute(
                () =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Filter = "CSV files (*.csv)|*.csv",
                        Title = "Import customers from CSV"
                    };

                    if (openFileDialog.ShowDialog() == true)
                    {
                        CsvImporter importer = new CsvImporter();
                        importer.Import(openFileDialog.FileName, (CustomerDAO)Dao);
                        LoadData();
                    }
                },
                "Failed to import customers from CSV."
            );
        }
    }
}
