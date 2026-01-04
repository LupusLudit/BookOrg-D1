using BookOrg.Src.Logic.Core.DBEntities;
using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Core.DAO
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="DAOBase"]/*'/>
    public abstract class DAOBase<T> where T : IDBEntity
    {
        protected SqlConnection connection;

        public DAOBase(SqlConnection connection)
        {
            this.connection = connection;
        }
        public abstract void Insert(T entity);
        public abstract void Update(T entity);
        public abstract void Delete(T entity);
        public abstract List<T> GetAll();

        /// <summary>
        /// Creates a new SqlCommand using the current connection.
        /// </summary>
        /// <param name="query">The SQL query string.</param>
        /// <returns>A configured SqlCommand object.</returns>
        protected SqlCommand CreateCommand(string query)
        {
            return new SqlCommand(query, connection);
        }
    }
}