using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CidadeDorme.Models;

namespace CidadeDorme.Services
{
    public class SalaService
    {
        private readonly Dictionary<string, Sala> _salas = [];

        public Sala CriarSala()
        {
            var codigo = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            var sala = new Sala { Codigo = codigo };
            _salas[codigo] = sala;
            return sala;
        }

        public Sala? ObterSala(string codigo) =>
            _salas.TryGetValue(codigo, out var sala) ? sala : null;

        public void AdicionarJogador(string codigoSala, Jogador jogador)
        {
            var sala = ObterSala(codigoSala) ?? throw new Exception("Sala não encontrada!");

            // Verifica se já existe um jogador com o mesmo nome
            if (sala.Jogadores.Any(j => j.Nome.Equals(jogador.Nome, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception("Já existe um jogador com este nome na sala!");
            }

            // Gera um ID de conexão único para o jogador
            jogador.ConexaoId = Guid.NewGuid().ToString();
            sala.Jogadores.Add(jogador);
        }


        public void MonstroAtacar(string codigoSala, string nomeJogador)
        {
            var sala = ObterSala(codigoSala) ?? throw new Exception("Sala não encontrada!");
            var jogador = sala.Jogadores.FirstOrDefault(j => j.Nome == nomeJogador && j.Vivo) ?? throw new Exception("Jogador não encontrado ou já está morto!");
            jogador.MarcadoPeloMonstro = true;
        }

        public void AnjoSalvar(string codigoSala, string nomeJogador)
        {
            var sala = ObterSala(codigoSala) ?? throw new Exception("Sala não encontrada!");
            var jogador = sala.Jogadores.FirstOrDefault(j => j.Nome == nomeJogador && j.Vivo) ?? throw new Exception("Jogador não encontrado ou já está morto!");
            jogador.MarcadoPeloMonstro = false; // Remove a marcação do monstro
        }

        public bool DetetiveAcusar(string codigoSala, string nomeJogador)
        {
            var sala = ObterSala(codigoSala) ?? throw new Exception("Sala não encontrada!");
            var jogador = sala.Jogadores.FirstOrDefault(j => j.Nome == nomeJogador && j.Vivo) ?? throw new Exception("Jogador não encontrado ou já está morto!");
            return jogador.Papel == "Monstro"; // Retorna verdadeiro se o acusado for o monstro
        }

        public void CidadeVotar(string codigoSala, string nomeJogador)
        {
            var sala = ObterSala(codigoSala) ?? throw new Exception("Sala não encontrada!");
            var jogador = sala.Jogadores.FirstOrDefault(j => j.Nome == nomeJogador && j.Vivo) ?? throw new Exception("Jogador não encontrado ou já está morto!");
            jogador.Vivo = false; // O jogador votado está eliminado
        }

        public string ProcessarTurno(string codigoSala)
        {
            var sala = ObterSala(codigoSala) ?? throw new Exception("Sala não encontrada!");

            // Verifica se algum jogador marcado pelo monstro não foi salvo
            foreach (var jogador in sala.Jogadores)
            {
                if (jogador.MarcadoPeloMonstro && jogador.Vivo)
                {
                    jogador.Vivo = false; // Morreu
                }

                jogador.MarcadoPeloMonstro = false; // Reseta a marcação
            }

            // Verifica condições de vitória
            var vivos = sala.Jogadores.Where(j => j.Vivo).ToList();
            var monstro = vivos.FirstOrDefault(j => j.Papel == "Monstro");
            if (monstro != null && vivos.Count <= 3)
            {
                return "O monstro venceu!";
            }

            if (vivos.All(j => j.Papel != "Monstro"))
            {
                return "A cidade venceu!";
            }

            return "O jogo continua!";
        }
    }

}