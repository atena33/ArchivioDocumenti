using ArchivioDocumenti.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ArchivioDocumenti.DataAccess
{
    public class DocumentoRepository : IDocumentoRepository
    {
        private readonly string _connectionString;

        public DocumentoRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// Aggiorna o associa il nome di un file allegato a un documento.
        /// Mappa la SP: sp_AggiornaAllegatoDocumento
        /// </summary>
        /// <param name="id">Id del documento a cui associare il file</param>
        /// <param name="nomeFile"></param>
        /// <returns>True se l'allegato è stato aggiornato con successo, False se il documento non esiste</returns>
        public async Task<bool> AggiornaAllegatoAsync(int id, string nomeFile)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_AggiornaAllegatoDocumento", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@NomeFile", SqlDbType.NVarChar, 255).Value = nomeFile;

                    await conn.OpenAsync();

                    object result = await cmd.ExecuteScalarAsync();

                    if (result != null && result != DBNull.Value)
                    {
                        int esito = Convert.ToInt32(result);
                        return esito == 1;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Aggiorna i dati principali di un documento esistente.
        /// Mappa la SP: sp_AggiornaDocumento
        /// </summary>
        /// <param name="documento"></param>
        /// <returns>True se l'aggiornamento è andato a buon fine, False se il documento non esisteva</returns>
        public async Task<bool> AggiornaDocumentoAsync(Documento documento)
        {
            if (documento == null)
            {
                throw new ArgumentNullException(nameof(documento));
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_AggiornaDocumento", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = documento.Id;
                    cmd.Parameters.Add("@Titolo", SqlDbType.NVarChar, 255).Value = documento.Titolo;
                    cmd.Parameters.Add("@Descrizione", SqlDbType.NVarChar, -1).Value =
                        string.IsNullOrEmpty(documento.Descrizione) ? DBNull.Value : (object)documento.Descrizione;
                    cmd.Parameters.Add("@CategoriaId", SqlDbType.Int).Value = documento.CategoriaDocumento.Id;
                    cmd.Parameters.Add("@ClienteId", SqlDbType.Int).Value = documento.Cliente.Id;
                    cmd.Parameters.Add("@NomeFile", SqlDbType.NVarChar, 255).Value = documento.NomeFile;
                    cmd.Parameters.Add("@Stato", SqlDbType.NVarChar, 50).Value = documento.Stato.ToString();

                    await conn.OpenAsync();

                    object result = await cmd.ExecuteScalarAsync();

                    if (result != null && result != DBNull.Value)
                    {
                        int righeModificate = Convert.ToInt32(result);

                        return righeModificate > 0;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Recupera la lista di tutti i documenti presenti.
        /// Mappa la SP: sp_GetDocumenti
        /// </summary>
        public async Task<IEnumerable<Documento>> GetDocumentiAsync()
        {
            string connectionString = _connectionString;
            var listaDocumenti = new List<Documento>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetDocumenti", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string statoDb = reader.GetString(reader.GetOrdinal("Stato"));
                            StatoDocumento statoConvertito = Enum.Parse<StatoDocumento>(statoDb, true);
                            var documento = new Documento
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Titolo = reader.GetString(reader.GetOrdinal("Titolo")),
                                Descrizione = reader.IsDBNull(reader.GetOrdinal("Descrizione"))
                                                ? null
                                                : reader.GetString(reader.GetOrdinal("Descrizione")),
                                CategoriaDocumento = new Categoria { Id = reader.GetInt32(reader.GetOrdinal("CategoriaId")), Nome = reader.GetString(reader.GetOrdinal("CategoriaNome")) },
                                Cliente = new Cliente { Id = reader.GetInt32(reader.GetOrdinal("ClienteId")), RagioneSociale = reader.GetString(reader.GetOrdinal("ClienteRagioneSociale")) },
                                NomeFile = reader.IsDBNull(reader.GetOrdinal("NomeFile"))
                                                ? string.Empty
                                                : reader.GetString(reader.GetOrdinal("NomeFile")),
                                Stato = statoConvertito,
                                DataCreazione = reader.GetDateTime(reader.GetOrdinal("DataCreazione")),
                                DataUltimaModifica = reader.GetDateTime(reader.GetOrdinal("DataUltimaModifica"))
                            };

                            listaDocumenti.Add(documento);
                        }
                    }
                }
            }

            return listaDocumenti;
        }

        /// <summary>
        /// Recupera i dettagli completi di un singolo documento tramite il suo id.
        /// Mappa la SP: sp_GetDocumentoDettaglio
        /// </summary>
        /// <param name="id">Id del documento da cercare</param>
        /// <returns>L'oggetto Documento popolato, oppure null se non viene trovato</returns>
        public async Task<Documento?> GetDocumentoByIdAsync(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetDocumentoDettaglio", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string statoDb = reader.GetString(reader.GetOrdinal("Stato"));
                            StatoDocumento statoConvertito = Enum.Parse<StatoDocumento>(statoDb, true);
                            return new Documento
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Titolo = reader.GetString(reader.GetOrdinal("Titolo")),

                                Descrizione = reader.IsDBNull(reader.GetOrdinal("Descrizione"))
                                                ? null
                                                : reader.GetString(reader.GetOrdinal("Descrizione")),

                                CategoriaDocumento = new Categoria { Id = reader.GetInt32(reader.GetOrdinal("CategoriaId")), Nome = reader.GetString(reader.GetOrdinal("CategoriaNome")) },
                                Cliente = new Cliente { Id = reader.GetInt32(reader.GetOrdinal("ClienteId")), RagioneSociale = reader.GetString(reader.GetOrdinal("ClienteRagioneSociale")) },
                                NomeFile = reader.IsDBNull(reader.GetOrdinal("NomeFile"))
                                                ? string.Empty
                                                : reader.GetString(reader.GetOrdinal("NomeFile")),

                                Stato = statoConvertito,

                                DataCreazione = reader.GetDateTime(reader.GetOrdinal("DataCreazione")),
                                DataUltimaModifica = reader.GetDateTime(reader.GetOrdinal("DataUltimaModifica"))
                            };
                        }
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Inserisce un nuovo documento e restituisce l'id generato.
        /// Mappa la SP: sp_InserisciDocumento
        /// </summary>
        /// <param name="documento"></param>
        /// <returns>L'id del nuovo record creato nel database</returns>
        public async Task<int> InserisciDocumentoAsync(Documento documento)
        {
            if (documento == null)
            {
                throw new ArgumentNullException(nameof(documento));
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InserisciDocumento", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Titolo", SqlDbType.NVarChar, 255).Value = documento.Titolo;
                    cmd.Parameters.Add("@Descrizione", SqlDbType.NVarChar, -1).Value =
                        string.IsNullOrEmpty(documento.Descrizione) ? DBNull.Value : (object)documento.Descrizione;
                    cmd.Parameters.Add("@CategoriaId", SqlDbType.Int).Value =
                        documento.CategoriaDocumento != null ? documento.CategoriaDocumento.Id : (object)DBNull.Value;
                    cmd.Parameters.Add("@ClienteId", SqlDbType.Int).Value =
                        documento.Cliente != null ? documento.Cliente.Id : (object)DBNull.Value;
                    cmd.Parameters.Add("@NomeFile", SqlDbType.NVarChar, 255).Value =
                        string.IsNullOrEmpty(documento.NomeFile) ? DBNull.Value : (object)documento.NomeFile;
                    cmd.Parameters.Add("@Stato", SqlDbType.NVarChar, 50).Value = documento.Stato.ToString() ?? "Bozza";

                    await conn.OpenAsync();

                    object result = await cmd.ExecuteScalarAsync();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }

                    throw new InvalidOperationException("Il database non ha restituito l'id del nuovo documento inserito.");
                }
            }
        }

        /// <summary>
        /// Cerca i documenti nel sistema applicando filtri dinamici e opzionali.
        /// Mappa la SP: sp_RicercaDocumenti
        /// </summary>
        /// <param name="categoriaId"></param>
        /// <param name="clienteId"></param>
        /// <param name="stato"></param>
        public async Task<IEnumerable<Documento>> RicercaDocumentoAsync(string? categoriaNome, string? clienteRagioneSociale, string? stato)
        {
            var listaRisultati = new List<Documento>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_RicercaDocumenti", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parametri allineati esattamente alla nuova Stored Procedure testuale
                    cmd.Parameters.Add("@CategoriaNome", SqlDbType.NVarChar, 255).Value =
                        string.IsNullOrEmpty(categoriaNome) ? DBNull.Value : (object)categoriaNome;

                    cmd.Parameters.Add("@ClienteRagioneSociale", SqlDbType.NVarChar, 255).Value =
                        string.IsNullOrEmpty(clienteRagioneSociale) ? DBNull.Value : (object)clienteRagioneSociale;

                    cmd.Parameters.Add("@Stato", SqlDbType.NVarChar, 50).Value =
                        string.IsNullOrEmpty(stato) ? DBNull.Value : (object)stato;

                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string statoDb = reader.GetString(reader.GetOrdinal("Stato"));
                            var documento = new Documento
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Titolo = reader.GetString(reader.GetOrdinal("Titolo")),
                                Descrizione = reader.IsDBNull(reader.GetOrdinal("Descrizione")) ? null : reader.GetString(reader.GetOrdinal("Descrizione")),
                                CategoriaDocumento = new Categoria
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CategoriaId")),
                                    Nome = reader.GetString(reader.GetOrdinal("CategoriaNome"))
                                },
                                Cliente = new Cliente
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ClienteId")),
                                    RagioneSociale = reader.GetString(reader.GetOrdinal("ClienteRagioneSociale"))
                                },
                                NomeFile = reader.IsDBNull(reader.GetOrdinal("NomeFile")) ? string.Empty : reader.GetString(reader.GetOrdinal("NomeFile")),
                                Stato = Enum.Parse<StatoDocumento>(statoDb, true),
                                DataCreazione = reader.GetDateTime(reader.GetOrdinal("DataCreazione")),
                                DataUltimaModifica = reader.GetDateTime(reader.GetOrdinal("DataUltimaModifica"))
                            };
                            listaRisultati.Add(documento);
                        }
                    }
                }
            }
            return listaRisultati;
        }
    }
}