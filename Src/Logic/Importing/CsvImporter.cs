using BookOrg.Src.Logic.Core.DAO;
using BookOrg.Src.Logic.Core.DBEntities;
using System.IO;
using System.Text;

namespace BookOrg.Src.Logic.Importing
{
    /// <include file='../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="CsvImporter"]/*'/>
    public class CsvImporter : IImporter
    {
        /// <summary>
        /// Reads a CSV file line by line, parses each line into an entity, and persists it using the provided DAO.
        /// </summary>
        /// <typeparam name="T">The type of the entity to import.</typeparam>
        /// <param name="filePath">The path to the CSV file.</param>
        /// <param name="importable">The DAO that handles the creation and insertion of the entity.</param>
        /// <exception cref="FormatException">Thrown if a CSV line does not contain the expected number of columns.</exception>
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