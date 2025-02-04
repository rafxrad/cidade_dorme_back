namespace CidadeDorme.DTOs
{
    public class SalaCreateDTO
    {
        public required string Nome { get; set; }
        public string? Senha { get; set; }
        public required int QuantidadeJogadores { get; set; }
    }
}