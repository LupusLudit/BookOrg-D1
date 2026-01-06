using BookOrg.Src.Logic.Core.DAO;
using BookOrg.Src.Logic.Core.DBEntities;

namespace BookOrg.Src.Logic.Importing
{
    /// <include file='../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="IImporter"]/*'/>
    public interface IImporter
    {
        /// <summary>
        /// Imports entities from a specified file using the provided DAO for persistence.
        /// </summary>
        /// <typeparam name="T">The type of the entity to import (must implement IDBEntity).</typeparam>
        /// <param name="filePath">The full path to the source file.</param>
        /// <param name="importable">The DAO instance capable of importing this entity type.</param>
        public void Import<T>(string filePath, IDAOImportable<T> importable) where T : IDBEntity;
    }
}