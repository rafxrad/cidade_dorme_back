namespace CidadeDorme.Models
{
    public class Mensagem
    {
        public required string NomeJogador { get; set; }
        public DateTime? Timestamp { get; private set; } = DateTime.Now;
        public required string Conteudo { get; set; }
    }
}