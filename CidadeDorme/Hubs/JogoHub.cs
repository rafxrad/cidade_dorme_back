using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CidadeDorme.DTOs;
using CidadeDorme.Models;
using CidadeDorme.Services;
using Microsoft.AspNetCore.SignalR;

namespace CidadeDorme.Hubs
{
    public class JogoHub(SalaService salaService) : Hub
    {
        private readonly SalaService _salaService = salaService;

        public async Task EntrarNaSala(string codigo, ConectarSalaDTO conectarDTO)
        {
            var sala = _salaService.ObterSala(codigo);
            if (sala == null || sala.Jogadores.Count >= 6)
                throw new HubException("Sala inválida ou cheia!");

            var jogador = new Jogador { Nome = conectarDTO.NomeJogador, ConexaoId = Context.ConnectionId };
            _salaService.AdicionarJogador(codigo, jogador, conectarDTO.Senha);

            await Clients.Group(codigo).SendAsync("JogadorEntrou", conectarDTO.NomeJogador);
            await Groups.AddToGroupAsync(Context.ConnectionId, codigo);
        }

        public async Task IniciarJogo(string codigo)
        {
            var sala = _salaService.ObterSala(codigo);
            if (sala == null || sala.Jogadores.Count != 6)
                throw new HubException("Não é possível iniciar o jogo!");

            // Atribuir papéis aleatoriamente
            var jogadores = sala.Jogadores;
            var papeis = new[] { "Anjo", "Detetive", "Monstro", "Cidadão", "Cidadão", "Cidadão" };
            var random = new Random();

            foreach (var jogador in jogadores)
                jogador.Papel = papeis[random.Next(papeis.Length)];

            sala.JogoIniciado = true;

            await Clients.Group(codigo).SendAsync("JogoIniciado", jogadores.Select(j => j.Nome));
        }

        public async Task EnviarMensagem(string codigoSala, string nomeJogador, string mensagem)
        {
            // Envia a mensagem para todos os jogadores na sala
            await Clients.Group(codigoSala).SendAsync("ReceberMensagem", nomeJogador, mensagem);
        }
    }
}