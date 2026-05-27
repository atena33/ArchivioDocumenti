using ArchivioDocumenti.DataAccess;
using ArchivioDocumenti.Models;
using System.Text.RegularExpressions;

namespace ArchivioDocumenti.BusinessLogic
{
    public class DocumentoService : IDocumentoService
    {
        private readonly IDocumentoRepository _repository;

        public DocumentoService(IDocumentoRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IEnumerable<Documento>> GetDocumentiDisponibiliAsync()
        {
            return await _repository.GetDocumentiAsync();
        }

        public async Task<int> CreaNuovoDocumentoAsync(Documento documento)
        {
            ValidazioneDocumento(documento);

            return await _repository.InserisciDocumentoAsync(documento);
        }

        public async Task<Documento?> GetDettaglioAsync(int id)
        {
            if (id <= 0) return null;
            return await _repository.GetDocumentoByIdAsync(id);
        }

        public async Task<bool> ModificaDocumentoAsync(Documento documento)
        {
            ValidazioneDocumento(documento);
            return await _repository.AggiornaDocumentoAsync(documento);
        }

        public async Task<IEnumerable<Documento>> CercaConFiltriAsync(string? categoriaNome, string? clienteRagioneSociale, string? stato)
        {
            return await _repository.RicercaDocumentoAsync(categoriaNome, clienteRagioneSociale, stato);
        }

        public async Task<bool> SalvaAllegatoDocumentoAsync(int id, string nomeFile)
        {
            if (string.IsNullOrWhiteSpace(nomeFile)) return false;
            return await _repository.AggiornaAllegatoAsync(id, nomeFile);
        }

        private void ValidazioneDocumento(Documento documento)
        {
            if (documento == null)
                throw new ArgumentNullException(nameof(documento), "I dati del documento sono nulli.");

            if (string.IsNullOrWhiteSpace(documento.Titolo))
                throw new ArgumentException("Il titolo del documento è obbligatorio.");

            if (documento.CategoriaDocumento.Id <= 0)
                throw new ArgumentException("La categoria del documento è obbligatoria.");

            if (documento.CategoriaDocumento.Id <= 0)
                throw new ArgumentException("Il Cliente/Azienda è obbligatorio per questo documento.");

            if (!Enum.IsDefined(typeof(StatoDocumento), documento.Stato))
                throw new ArgumentException("Lo stato specificato non rientra tra quelli previsti dal sistema.");

           
        }
    }

    
}

