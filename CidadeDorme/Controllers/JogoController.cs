using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CidadeDorme.Models;
using CidadeDorme.Services;
using Microsoft.AspNetCore.Mvc;

namespace CidadeDorme.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JogoController(SalaService salaService) : ControllerBase
    {
        private readonly SalaService _salaService = salaService;

        // Endpoint para criar uma nova sala
        [HttpPost("criar-sala")]
        public IActionResult CriarSala()
        {
            var sala = _salaService.CriarSala();
            return Ok(new { Codigo = sala.Codigo });
        }

        // Endpoint para entrar em uma sala
        [HttpPost("entrar-sala/{codigo}")]
        public IActionResult EntrarNaSala(string codigo, [FromBody] Jogador jogador)
        {
            var sala = _salaService.ObterSala(codigo);
            if (sala == null)
                return NotFound("Sala não encontrada!");

            if (sala.Jogadores.Count >= 6)
                return BadRequest("A sala já está cheia!");

            _salaService.AdicionarJogador(codigo, jogador);
            return Ok(new { Mensagem = $"{jogador.Nome} entrou na sala {codigo}!" });
        }

        // Endpoint para iniciar o jogo
        [HttpPost("iniciar-jogo/{codigo}")]
        public IActionResult IniciarJogo(string codigo)
        {
            var sala = _salaService.ObterSala(codigo);
            if (sala == null)
                return NotFound("Sala não encontrada!");

            if (sala.Jogadores.Count != 6)
                return BadRequest("O jogo só pode ser iniciado com 6 jogadores!");

            // Atribuir papéis aleatoriamente
            var jogadores = sala.Jogadores;
            var papeis = new[] { "Anjo", "Detetive", "Monstro", "Cidadão", "Cidadão", "Cidadão" };
            var random = new Random();
            var papeisEmbaralhados = papeis.OrderBy(_ => random.Next()).ToList();

            for (int i = 0; i < sala.Jogadores.Count; i++)
            {
                sala.Jogadores[i].Papel = papeisEmbaralhados[i];
            }

            sala.JogoIniciado = true;

            return Ok(new { Mensagem = "Jogo iniciado!", Jogadores = jogadores });
        }

        // Endpoint para obter o estado atual da sala
        [HttpGet("sala/{codigo}")]
        public IActionResult ObterSala(string codigo)
        {
            var sala = _salaService.ObterSala(codigo);
            if (sala == null)
                return NotFound("Sala não encontrada!");

            return Ok(sala);
        }
    }
}