using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ArchivioDocumenti.DataAccess
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly string _connectionString;

        public CategoriaRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// Recupera la lista di tutte le categorie presenti e attive.
        /// Mappa la SP: sp_GetCategorieAttive
        /// </summary>
        public async Task<IEnumerable<dynamic>> GetCategorieAttiveAsync()
        {
            var categorie = new List<dynamic>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("dbo.sp_GetCategorieAttive", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            categorie.Add(new
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Nome = reader.GetString(reader.GetOrdinal("Nome"))
                            });
                        }
                    }
                }
            }
            return categorie;
        }

        /// <summary>
        /// Inserisce una nuova categoria e restituisce l'id generato.
        /// Mappa la SP: sp_InserisciCategoria
        /// </summary>
        /// <param name="nome"></param>
        /// <returns>L'id del nuovo record creato nel database</returns>
        public async Task<int> InserisciCategoriaAsync(string nome)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("dbo.sp_InserisciCategoria", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Nome", nome);

                    await connection.OpenAsync();

                    var result = await command.ExecuteScalarAsync();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }
    }
}