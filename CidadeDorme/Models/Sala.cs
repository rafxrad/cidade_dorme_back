using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CidadeDorme.Models
{
    public class Sala
    {
        public required string Codigo { get; set; }
        public List<Jogador> Jogadores { get; set; } = [];
        public bool JogoIniciado { get; set; } = false;
        public EstadoJogo Estado { get; set; } = new EstadoJogo();
    }

}