using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CidadeDorme.Models
{
    public class EstadoJogo
    {
        public string Fase { get; set; } = "Esperando"; // "Noite", "Dia", "Votação"
        public string? Vitima { get; set; }
        public string? Protegido { get; set; }
        public string? Investigado { get; set; }
        public List<string> Votos { get; set; } = [];
    }

}