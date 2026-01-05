using BookOrg.Src.Logic.Core.DAO;
using BookOrg.Src.Logic.Core.DBEntities;

namespace BookOrg.Src.Logic.Importing
{
    public interface IImporter
    {
        public void Import<T>(string filePath, IDAOImportable<T> importable) where T : IDBEntity;
    }
}