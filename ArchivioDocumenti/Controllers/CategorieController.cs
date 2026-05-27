using ArchivioDocumenti.BusinessLogic;
using ArchivioDocumenti.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ArchivioDocumenti.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategorieController : Controller
    {
        private readonly ICategoriaService _categoriaService;

        public CategorieController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService ?? throw new ArgumentNullException(nameof(categoriaService));
        }

        /// <summary>
        /// GET: api/categorie
        /// Recupera la lista di tutte le categorie.
        /// </summary>

        [HttpGet]
        public async Task<IActionResult> GetTutti()
        {
            var categorie = await _categoriaService.GetCategorieAttiveAsync();
            return Ok(categorie);
        }

        /// <summary>
        /// POST: api/categorie
        /// Inserisce una nuova categoria.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Crea([FromBody] Categoria nuovaCategoria)
        {
            int nuovoId = await _categoriaService.InserisciCategoriaAsync(nuovaCategoria.Nome);
            nuovaCategoria.Id = nuovoId;
            nuovaCategoria.Attiva = true;

            return Created($"/api/categorie/{nuovoId}", nuovaCategoria);
        }
    }
}