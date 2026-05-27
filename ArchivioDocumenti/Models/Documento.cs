namespace ArchivioDocumenti.Models
{
    public class Documento
    {
        public int Id { get; set; }

        public string Titolo { get; set; } = string.Empty;

        public string? Descrizione { get; set; }

        public Categoria CategoriaDocumento { get; set; } = new Categoria();
        public Cliente Cliente { get; set; } = new Cliente();

        public string NomeFile { get; set; } = string.Empty;

        public StatoDocumento Stato { get; set; }

        public DateTime DataCreazione { get; set; } = DateTime.Now;
        public DateTime DataUltimaModifica { get; set; } = DateTime.Now;

        
    }
}

public enum StatoDocumento
{
    Bozza,
    Pubblicato,
    Archiviato,
    Annullato
}