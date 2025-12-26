using Microsoft.Data.SqlClient;
using System.Configuration;
using BookOrg.Src.Safety;

namespace BookOrg.Src.Logic.Connection
{
    public class SqlServerConnectionFactory : IConnectionFactory
    {
        public SqlServerConnectionFactory() { }

        public SqlConnection? CreateConnection()
        {
            SqlConnection? connection = null;

            SafeExecutor.Execute(() =>
            {
                string? dataSource = ConfigurationManager.AppSettings["DataSource"];
                string? database = ConfigurationManager.AppSettings["Database"];
                string? login = ConfigurationManager.AppSettings["Login"];
                string? password = ConfigurationManager.AppSettings["Password"];

                ValidateConfig(dataSource, database, login, password);

                string connectionString =
                    $"Server={dataSource};" +
                    $"Database={database};" +
                    $"User Id={login};" +
                    $"Password={password};" +
                    "TrustServerCertificate=True;";

                var connectionAttempt = new SqlConnection(connectionString);
                connectionAttempt.Open();

                connection = connectionAttempt;

            }, "Failed to connect to the database");

            return connection;
        }

        private void ValidateConfig(string? ds, string? db, string? user, string? pwd)
        {
            if (string.IsNullOrWhiteSpace(ds) || string.IsNullOrWhiteSpace(db) ||
                string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pwd))
            {
                throw new ApplicationException("Database configuration is missing or invalid.");
            }
        }
    }
}