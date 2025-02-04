using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CidadeDorme.Models
{
    public class Jogador
    {
        public required string Nome { get; set; }
        public string? ConexaoId { get; set; }
        public string? Papel { get; set; } 
        public bool Vivo { get; set; } = true;
    }

}