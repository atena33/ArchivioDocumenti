using ArchivioDocumenti.Models;

namespace ArchivioDocumenti.BusinessLogic
{
    public interface IDocumentoService
    {
        Task<IEnumerable<Documento>> GetDocumentiDisponibiliAsync();
        Task<Documento?> GetDettaglioAsync(int id);
        Task<int> CreaNuovoDocumentoAsync(Documento documento);
        Task<bool> ModificaDocumentoAsync(Documento documento);
        Task<IEnumerable<Documento>> CercaConFiltriAsync(string? categoriaNome, string? clienteRagioneSociale, string? stato);
        Task<bool> SalvaAllegatoDocumentoAsync(int id, string nomeFile);
    }
}
