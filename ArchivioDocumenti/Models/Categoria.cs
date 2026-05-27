namespace ArchivioDocumenti.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public bool Attiva { get; set; } = true;
    }
}
