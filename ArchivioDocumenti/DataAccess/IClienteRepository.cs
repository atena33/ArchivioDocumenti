using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArchivioDocumenti.DataAccess
{
    public interface IClienteRepository
    {
        /// <summary>
        /// Recupera la lista di tutti i clienti presenti e attivi.
        /// Mappa la SP: sp_GetClientiAttivi
        /// </summary>
        Task<IEnumerable<dynamic>> GetClientiAttiviAsync();

        /// <summary>
        /// Inserisce un nuovo cliente e restituisce l'id generato.
        /// Mappa la SP: sp_InserisciCliente
        /// </summary>
        /// <param name="ragioneSociale"></param>
        /// <param name="email"></param>
        /// <returns>L'id del nuovo record creato nel database</returns>
        Task<int> InserisciClienteAsync(string ragioneSociale, string email);
    }
}