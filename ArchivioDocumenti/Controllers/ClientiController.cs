using ArchivioDocumenti.BusinessLogic;
using ArchivioDocumenti.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ArchivioDocumenti.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientiController : Controller
    {
        private readonly IClienteService _clienteService;

        public ClientiController(IClienteService clienteService)
        {
            _clienteService = clienteService ?? throw new ArgumentNullException(nameof(clienteService));
        }

        /// <summary>
        /// GET: api/clienti
        /// Recupera la lista di tutti i clienti.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTutti()
        {
            var clienti = await _clienteService.GetClientiAttiviAsync();
            return Ok(clienti);
        }

        /// <summary>
        /// POST: api/clienti
        /// Inserisce un nuovo cliente.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Crea([FromBody] Cliente nuovoCliente)
        {
            int nuovoId = await _clienteService.InserisciClienteAsync(nuovoCliente.RagioneSociale, nuovoCliente.Email);
            nuovoCliente.Id = nuovoId;
            nuovoCliente.Attivo = true;

            return Created($"/api/clienti/{nuovoId}", nuovoCliente);
        }
    }
}