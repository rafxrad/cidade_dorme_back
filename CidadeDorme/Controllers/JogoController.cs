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
            try
            {
                if (salaDto.QuantidadeJogadores < 6)
                {
                    return BadRequest("O número mínimo de jogadores em uma sala deve ser 6");
                }
                var sala = _salaService.CriarSala(salaDto.Nome, salaDto.QuantidadeJogadores, salaDto.Senha);

                _salaService.EnviarMensagemSistema(sala.Codigo!, "Sala criada, com sucesso");

                return Ok(new
                {
                    NomeSala = sala.Nome,
                    Codigo = sala.Codigo!,
                    QuantidadeJogadores = sala.QuantidadeJogadores!
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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

                _salaService.EnviarMensagemSistema(sala.Codigo!, $"{jogador.Nome} entrou na sala");

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
                return BadRequest("O jogo só pode ser iniciado com 6 jogadores ou mais!");

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

            _salaService.EnviarMensagemSistema(sala.Codigo!, "A noite cai sobre a cidade... olhos se fecham, mas o perigo espreita. O jogo começou!");


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
        public IActionResult MonstroAtacar(string codigo, [FromBody] NomeJogadorDTO dto)
        {
            try
            {
                _salaService.MonstroAtacar(codigo, dto.NomeJogador);

                _salaService.EnviarMensagemSistema(codigo, "O vento trouxe um sussurro sombrio... o destino de alguém foi selado");

                return Ok("Ataque registrado!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("anjo-salvar/{codigo}")]
        public IActionResult AnjoSalvar(string codigo, [FromBody] NomeJogadorDTO dto)
        {
            try
            {
                _salaService.AnjoProteger(codigo, dto.NomeJogador);

                _salaService.EnviarMensagemSistema(codigo, "Alguém sentiu um calor inexplicável... talvez um destino tenha sido mudado");

                return Ok("Salvamento registrado!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("detetive-acusar/{codigo}")]
        public IActionResult DetetiveAcusar(string codigo, [FromBody] NomeJogadorDTO dto)
        {
            try
            {
                var resultado = _salaService.DetetiveInvestigar(codigo, dto.NomeJogador);

                _salaService.EnviarMensagemSistema(codigo, "O Detetive passou a noite observando... mas o que ele descobriu?");

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

        [HttpPost("enviar-mensagem/{codigo}")]
        public IActionResult EnviarMensagem(string codigo, [FromBody] MensagemDTO mensagemDTO)
        {
            try
            {
                _salaService.EnviarMensagem(codigo, mensagemDTO.NomeJogador, mensagemDTO.Conteudo);
                return Ok("Mensagem registrada registrado!");
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

                _salaService.EnviarMensagemSistema(codigo, "Os primeiros raios de sol iluminam a vila... um novo dia começa, mas a que custo?");
                _salaService.EnviarMensagemSistema(codigo, $"{resultado}");

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

                _salaService.EnviarMensagemSistema(codigoSala, "A multidão murmura, dedos apontam... e um nome é dito em voz alta.");

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