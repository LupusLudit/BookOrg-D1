using BookOrg.Src.Logic.Core.DBEntities;

namespace BookOrg.Src.Logic.Core.DAO
{
    public interface IDAOImportable<T> where T: IDBEntity
    {
        int ColumnCount { get; }

        T FromCsv(string[] values);

        void ImportEntity(T entity);
    }
}
