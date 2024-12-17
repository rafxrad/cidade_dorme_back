using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CidadeDorme.Models;

namespace CidadeDorme.Services
{
    public class SalaService
    {
        private readonly Dictionary<string, Sala> _salas = new();

        public Sala CriarSala()
        {
            var codigo = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            var sala = new Sala { Codigo = codigo };
            _salas[codigo] = sala;
            return sala;
        }

        public Sala? ObterSala(string codigo) =>
            _salas.TryGetValue(codigo, out var sala) ? sala : null;

        public void AdicionarJogador(string codigo, Jogador jogador)
        {
            if (_salas.ContainsKey(codigo))
                _salas[codigo].Jogadores.Add(jogador);
        }
    }

}