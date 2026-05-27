using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArchivioDocumenti.BusinessLogic
{
    public interface IClienteService
    {
        Task<IEnumerable<dynamic>> GetClientiAttiviAsync();
        Task<int> InserisciClienteAsync(string ragioneSociale, string email);
    }
}