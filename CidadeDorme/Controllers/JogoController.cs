using CidadeDorme.DTOs;
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
        public IActionResult CriarSala([FromBody] SalaCreateDTO salaDto)
        {
            if (salaDto.QuantidadeJogadores < 6)
            {
                return BadRequest("O número mínimo de jogadores em uma sala deve ser 6");
            }
            var sala = _salaService.CriarSala(salaDto.Nome, salaDto.QuantidadeJogadores, salaDto.Senha);
            return Ok(new
            {
                NomeSala = sala.Nome,
                Codigo = sala.Codigo!,
                QuantidadeJogadores = sala.QuantidadeJogadores!
            });
        }

        [HttpDelete("destruir-sala/{codigoSala}")]
        public IActionResult DestruirSala(string codigoSala)
        {
            try
            {
                _salaService.DestruirSala(codigoSala);
                return NoContent();                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("entrar-sala/{codigo}")]
        public IActionResult EntrarNaSala(string codigo, [FromBody] ConectarSalaDTO conectarDTO) 
        {
            try
            {
                var sala = _salaService.ObterSala(codigo);
                if (sala == null)
                    return NotFound("Sala não encontrada!");

                if (sala.Jogadores.Count >= sala.QuantidadeJogadores)
                    return BadRequest("A sala já está cheia!");

                // Mapeia o DTO para o objeto Jogador
                var jogador = new Jogador
                {
                    Nome = conectarDTO.NomeJogador
                };

                _salaService.AdicionarJogador(codigo, jogador, conectarDTO.Senha);

                return Ok(new
                {
                    Mensagem = $"{jogador.Nome} entrou na sala {codigo}!",
                    ConexaoId = jogador.ConexaoId! 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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

            if (sala.Estado.Fase != "Esperando")
                return BadRequest("O jogo já foi iniciado ou não está na fase de espera!");

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
            sala.Estado.Fase = "Noite";

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

        [HttpPost("monstro-atacar/{codigo}")]
        public IActionResult MonstroAtacar(string codigo, [FromBody] string nomeJogador)
        {
            try
            {
                _salaService.MonstroAtacar(codigo, nomeJogador);
                return Ok("Ataque registrado!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("anjo-salvar/{codigo}")]
        public IActionResult AnjoSalvar(string codigo, [FromBody] string nomeJogador)
        {
            try
            {
                _salaService.AnjoProteger(codigo, nomeJogador);
                return Ok("Salvamento registrado!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("detetive-acusar/{codigo}")]
        public IActionResult DetetiveAcusar(string codigo, [FromBody] string nomeJogador)
        {
            try
            {
                var resultado = _salaService.DetetiveInvestigar(codigo, nomeJogador);
                return Ok(new { AcusadoEhMonstro = resultado });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("cidade-votar/{codigo}")]
        public IActionResult CidadeVotar(string codigo, [FromBody] VotarDTO votoDTO)
        {
            try
            {
                _salaService.Votar(codigo, votoDTO.NomeJogador, votoDTO.NomeVotado);
                return Ok("Voto registrado!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("processar-turno/{codigo}")]
        public IActionResult ProcessarTurno(string codigo)
        {
            try
            {
                var resultado = _salaService.ProcessarTurno(codigo);
                return Ok(new { Resultado = resultado });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("apurar-votos/{codigoSala}")]
        public IActionResult ApurarVotos(string codigoSala)
        {
            try
            {
                // Chama o serviço para apurar votos e eliminar o jogador
                var jogadorEliminado = _salaService.ApurarVotos(codigoSala);

                return Ok(new
                {
                    Mensagem = "Apuração realizada com sucesso!",
                    JogadorEliminado = jogadorEliminado
                });
            }
            catch (Exception ex)
            {
                // Retorna erro em caso de problemas
                return BadRequest(new { Erro = ex.Message });
            }
        }

        [HttpGet("todas-as-salas")]
        public IActionResult ObterTodasAsSalas()
        {
            try
            {
                var salas = _salaService.ObterTodasAsSalas(); // Método que retorna todas as salas
                return Ok(salas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }


    }
}