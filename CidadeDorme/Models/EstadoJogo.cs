using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CidadeDorme.Models
{
    public class EstadoJogo
{
    public string Fase { get; set; } = "Esperando"; 
    public string? Vitima { get; set; }
    public string? Protegido { get; set; }
    public string? Investigado { get; set; }
    
    // Dicionário onde a chave é o jogador que votou e o valor é o jogador que recebeu o voto
    public Dictionary<string, string> Votos { get; set; } = new();
}

}