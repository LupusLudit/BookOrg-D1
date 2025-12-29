using BookOrg.Src.Logic.Core.DBEntities.Tables;
using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Core.DAO.Tables
{
    public abstract class TableDAOBase<T> where T : IDBTable
    {
        protected SqlConnection connection;

        public TableDAOBase(SqlConnection connection)
        {
            this.connection = connection;
        }

        public abstract T? GetByID(int id);
        public abstract void Insert(T entity);
        public abstract void Update(T entity);
        public abstract void Delete(T entity);
        public abstract List<T> GetAll();

        protected SqlCommand CreateCommand(string query)
        {
            return new SqlCommand(query, connection);
        }
    }
}