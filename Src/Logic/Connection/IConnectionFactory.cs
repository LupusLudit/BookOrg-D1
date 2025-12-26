using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Connection
{
    public interface IConnectionFactory
    {
        SqlConnection? CreateConnection();
    }
}
