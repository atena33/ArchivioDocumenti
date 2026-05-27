using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArchivioDocumenti.BusinessLogic
{
    public interface ICategoriaService
    {
        Task<IEnumerable<dynamic>> GetCategorieAttiveAsync();
        Task<int> InserisciCategoriaAsync(string nome);
    }
}