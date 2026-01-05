using BookOrg.Src.Logic.Core.DAO;
using BookOrg.Src.Logic.Core.DBEntities;
using System.IO;
using System.Text;

namespace BookOrg.Src.Logic.Importing
{
    public class CsvImporter : IImporter
    {
        public void Import<T>(string filePath, IDAOImportable<T> importable) where T : IDBEntity
        {
            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                string? line;
                bool firstLine = true;

                while ((line = reader.ReadLine()) != null)
                {
                    if (firstLine)
                    {
                        firstLine = false;
                        continue;
                    }

                    string[] values = line.Split(',');
                    if (values.Length != importable.ColumnCount)
                    {
                        throw new FormatException("Incorrect CSV format.");
                    }

                    T entity = importable.FromCsv(values);
                    importable.ImportEntity(entity);
                }
            }
        }
    }
}