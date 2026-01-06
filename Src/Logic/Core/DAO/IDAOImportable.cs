using BookOrg.Src.Logic.Core.DBEntities;

namespace BookOrg.Src.Logic.Core.DAO
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="IDAOImportable"]/*'/>
    public interface IDAOImportable<T> where T : IDBEntity
    {
        /// <summary>
        /// Represents the expected number of columns in the CSV source for this entity.
        /// </summary>
        int ColumnCount { get; }

        /// <summary>
        /// Converts an array of string values (from a CSV line) into an entity instance.
        /// </summary>
        /// <param name="values">The raw string values from the CSV file.</param>
        /// <returns>A populated instance of the entity.</returns>
        T FromCsv(string[] values);

        /// <summary>
        /// Persists the imported entity into the database.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        void ImportEntity(T entity);
    }
}