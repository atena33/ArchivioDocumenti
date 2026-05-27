using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArchivioDocumenti.DataAccess;

namespace ArchivioDocumenti.BusinessLogic
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository ?? throw new ArgumentNullException(nameof(categoriaRepository));
        }

        public async Task<IEnumerable<dynamic>> GetCategorieAttiveAsync()
        {
            return await _categoriaRepository.GetCategorieAttiveAsync();
        }

        public async Task<int> InserisciCategoriaAsync(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentException("Il nome della categoria non può essere vuoto.");
            }

            return await _categoriaRepository.InserisciCategoriaAsync(nome.Trim());
        }

    }
}