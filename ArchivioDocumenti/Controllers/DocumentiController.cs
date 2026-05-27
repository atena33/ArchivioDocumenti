using ArchivioDocumenti.BusinessLogic;
using ArchivioDocumenti.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArchivioDocumenti.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentiController : Controller
    {
        
        
        private readonly IDocumentoService _documentoService;

        public DocumentiController(IDocumentoService documentoService)
        {
            _documentoService = documentoService ?? throw new ArgumentNullException(nameof(documentoService));
        }

        /// <summary>
        /// GET: api/documenti
        /// Recupera la lista di tutti i documenti.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTutti()
        {
            var documenti = await _documentoService.GetDocumentiDisponibiliAsync();
            return Ok(documenti); 
        }

        /// <summary>
        /// GET: api/documenti/{id}
        /// Recupera il dettaglio di un singolo documento tramite id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPerId(int id)
        {
            var documento = await _documentoService.GetDettaglioAsync(id);

            if (documento == null)
            {
                return NotFound(new { Messaggio = $"Documento con ID {id} non trovato." }); 
            }

            return Ok(documento); 
        }

        /// <summary>
        /// POST: api/documenti
        /// Inserisce un nuovo documento.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Crea([FromBody] Documento nuovoDocumento)
        {
            try
            {
                int nuovoId = await _documentoService.CreaNuovoDocumentoAsync(nuovoDocumento);
                nuovoDocumento.Id = nuovoId;
                return CreatedAtAction(nameof(GetPerId), new { id = nuovoId }, nuovoDocumento);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Errore = ex.Message });
            }
        }

        /// <summary>
        /// PUT: api/documenti/{id}
        /// Aggiorna i dati principali di un documento esistente.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Aggiorna(int id, [FromBody] Documento documentoModificato)
        {
            if (documentoModificato == null || id != documentoModificato.Id)
            {
                return BadRequest("Dati non corrispondenti.");
            }

            try
            {
                bool esito = await _documentoService.ModificaDocumentoAsync(documentoModificato);
                if (!esito) return NotFound(new { Messaggio = "Documento non trovato." });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Errore = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/documenti/ricerca
        /// Cerca documenti applicando filtri opzionali nella query string.
        /// </summary>
        [HttpGet("ricerca")]
        public async Task<IActionResult> Ricerca([FromQuery] string? categoriaNome, [FromQuery] string? clienteRagioneSociale, [FromQuery] string? stato)
        {
            var risultati = await _documentoService.CercaConFiltriAsync(categoriaNome, clienteRagioneSociale, stato);
            return Ok(risultati);
        }

        /// <summary>
        /// PATCH: api/documenti/{id}/allegato
        /// Gestisce l'upload di un file allegato.
        /// </summary>
        [HttpPatch("{id:int}/allegato")]
        public async Task<IActionResult> CaricaAllegato(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Messaggio = "Il file allegato è obbligatorio." });
            }

            long dimensioneMassimaB = 5 * 1024 * 1024;
            if (file.Length > dimensioneMassimaB)
            {
                return BadRequest(new { Messaggio = "Il file supera la dimensione massima consentita di 5 Megabyte." });
            }

            string estensione = Path.GetExtension(file.FileName).ToLower();
            var estensioniConsentite = new List<string> { ".pdf", ".docx", ".doc", ".jpg", ".png" };

            if (!estensioniConsentite.Contains(estensione))
            {
                return BadRequest(new { Messaggio = $"Estensione {estensione} non consentita. Scegli tra: PDF, DOCX, DOC, JPG, PNG." });
            }

            string cartellaDestinazione = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(cartellaDestinazione)) Directory.CreateDirectory(cartellaDestinazione);

            string nomeFileDaSalvare = $"doc_{id}_{Guid.NewGuid()}{estensione}";
            string percorsoCompleto = Path.Combine(cartellaDestinazione, nomeFileDaSalvare);

            using (var stream = new FileStream(percorsoCompleto, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            bool esito = await _documentoService.SalvaAllegatoDocumentoAsync(id, nomeFileDaSalvare);

            if (!esito)
            {
                if (System.IO.File.Exists(percorsoCompleto)) System.IO.File.Delete(percorsoCompleto);
                return NotFound(new { Messaggio = $"Impossibile associare l'allegato. Documento {id} non trovato." });
            }

            return Ok(new { Messaggio = "Allegato superato i controlli e salvato.", NomeFile = nomeFileDaSalvare });
        }
    }


}
