using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArchivioDocumenti.DataAccess
{
    public interface ICategoriaRepository
    {
        /// <summary>
        /// Recupera la lista di tutte le categorie presenti e attive.
        /// Mappa la SP: sp_GetCategorieAttive
        /// </summary>
        Task<IEnumerable<dynamic>> GetCategorieAttiveAsync();

        /// <summary>
        /// Inserisce una nuova categoria e restituisce l'id generato.
        /// Mappa la SP: sp_InserisciCategoria
        /// </summary>
        /// <param name="nome"></param>
        /// <returns>L'id del nuovo record creato nel database</returns>
        Task<int> InserisciCategoriaAsync(string nome);
    }
}