using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CidadeDorme.Models
{
    public class Jogador
    {
        public required string Nome { get; set; }
        public string? ConexaoId { get; set; } // Para identificar via SignalR
        public string? Papel { get; set; } // Anjo, Detetive, Monstro, Cidadão
        public bool Vivo { get; set; } = true;
    }

}