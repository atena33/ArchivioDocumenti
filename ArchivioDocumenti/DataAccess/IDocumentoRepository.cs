using ArchivioDocumenti.Models;

namespace ArchivioDocumenti.DataAccess
{
    public interface IDocumentoRepository
    {
        /// <summary>
        /// Recupera la lista di tutti i documenti presenti.
        /// Mappa la SP: sp_GetDocumenti
        /// </summary>
        Task<IEnumerable<Documento>> GetDocumentiAsync();

        /// <summary>
        /// Inserisce un nuovo documento e restituisce l'id generato.
        /// Mappa la SP: sp_InserisciDocumento
        /// </summary>
        /// <param name="documento"></param>
        /// <returns>L'id del nuovo record creato nel database</returns>
        Task<int> InserisciDocumentoAsync(Documento documento);

        /// <summary>
        /// Aggiorna i dati principali di un documento esistente.
        /// Mappa la SP: sp_AggiornaDocumento
        /// </summary>
        /// <param name="documento"></param>
        /// <returns>True se l'aggiornamento è andato a buon fine, False se il documento non esisteva</returns>
        Task<bool> AggiornaDocumentoAsync(Documento documento);

        /// <summary>
        /// Recupera i dettagli completi di un singolo documento tramite il suo id.
        /// Mappa la SP: sp_GetDocumentoDettaglio
        /// </summary>
        /// <param name="id">Id del documento da cercare</param>
        /// <returns>L'oggetto Documento popolato, oppure null se non viene trovato</returns>
        Task<Documento?> GetDocumentoByIdAsync(int id);

        /// <summary>
        /// Cerca i documenti nel sistema applicando filtri dinamici e opzionali.
        /// Mappa la SP: sp_RicercaDocumenti
        /// </summary>
        /// <param name="categoriaId"></param>
        /// <param name="clienteId"></param>
        /// <param name="stato"></param>
        Task<IEnumerable<Documento>> RicercaDocumentoAsync(string? categoriaNome, string? clienteRagioneSociale, string? stato);

        /// <summary>
        /// Aggiorna o associa il nome di un file allegato a un documento.
        /// Mappa la SP: sp_AggiornaAllegatoDocumento
        /// </summary>
        /// <param name="id">Id del documento a cui associare il file</param>
        /// <param name="nomeFile"></param>
        /// <returns>True se l'allegato è stato aggiornato con successo, False se il documento non esiste</returns>
        Task<bool> AggiornaAllegatoAsync(int id, string nomeFile);
    }
}
