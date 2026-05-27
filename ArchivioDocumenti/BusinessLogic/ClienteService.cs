using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArchivioDocumenti.DataAccess;

namespace ArchivioDocumenti.BusinessLogic
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository ?? throw new ArgumentNullException(nameof(clienteRepository));
        }

        public async Task<IEnumerable<dynamic>> GetClientiAttiviAsync()
        {
            return await _clienteRepository.GetClientiAttiviAsync();
        }

        public async Task<int> InserisciClienteAsync(string ragioneSociale, string email)
        {
            if (string.IsNullOrWhiteSpace(ragioneSociale))
            {
                throw new ArgumentException("La ragione sociale non può essere vuota.");
            }
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                throw new ArgumentException("Fornire un indirizzo email valido.");
            }

            return await _clienteRepository.InserisciClienteAsync(ragioneSociale.Trim(), email.Trim());
        }
    }
}