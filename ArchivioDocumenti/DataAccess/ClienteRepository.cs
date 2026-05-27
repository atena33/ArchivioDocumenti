using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ArchivioDocumenti.DataAccess
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly string _connectionString;

        public ClienteRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// Recupera la lista di tutti i clienti presenti e attivi.
        /// Mappa la SP: sp_GetClientiAttivi
        /// </summary>
        public async Task<IEnumerable<dynamic>> GetClientiAttiviAsync()
        {
            var clienti = new List<dynamic>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("dbo.sp_GetClientiAttivi", connection)) 
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            clienti.Add(new
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                RagioneSociale = reader.GetString(reader.GetOrdinal("RagioneSociale")),
                                Email = reader.GetString(reader.GetOrdinal("Email"))
                            });
                        }
                    }
                }
            }
            return clienti;
        }

        /// <summary>
        /// Inserisce un nuovo cliente e restituisce l'id generato.
        /// Mappa la SP: sp_InserisciCliente
        /// </summary>
        /// <param name="ragioneSociale"></param>
        /// <param name="email"></param>
        /// <returns>L'id del nuovo record creato nel database</returns>
        public async Task<int> InserisciClienteAsync(string ragioneSociale, string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("dbo.sp_InserisciCliente", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@RagioneSociale", ragioneSociale);
                    command.Parameters.AddWithValue("@Email", email);

                    await connection.OpenAsync();

                    var result = await command.ExecuteScalarAsync();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }
    }
}