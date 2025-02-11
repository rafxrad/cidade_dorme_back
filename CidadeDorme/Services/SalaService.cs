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

        public Sala CriarSala(string nome, int quantidadeJogadores, string? senha = null)
        {
            // Verifica se já existe uma sala com o mesmo nome
            if (_salas.Values.Any(s => s.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception("Já existe uma sala com esse nome!");
            }
            
            var codigo = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

            var sala = new Sala
            {
                Nome = nome,
                Codigo = codigo,
                Senha = senha,
                QuantidadeJogadores = quantidadeJogadores
            };


            _salas[codigo] = sala;
            return sala;
        }

        public void DestruirSala(string codigoSala)
        {
            if (!_salas.ContainsKey(codigoSala))
            {
                throw new Exception("Sala não encontrada!");
            }

            // Remover a sala do dicionário
            _salas.Remove(codigoSala);
        }

        public Sala? ObterSala(string codigo) =>
            _salas.TryGetValue(codigo, out var sala) ? sala : null;

        public void AdicionarJogador(string codigoSala, Jogador jogador, string? senha)
        {
            if (jogador.Nome.ToLower() == "sistema")
            {
                throw new Exception("Nome indisponível para uso");
            }

            var sala = ObterSala(codigoSala) ?? throw new Exception("Sala não encontrada!");

            // Verificar se a sala exige senha e se a senha foi fornecida corretamente
            if (!string.IsNullOrEmpty(sala.Senha) && sala.Senha != senha)
            {
                throw new Exception("Senha incorreta para acessar a sala.");
            }

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
            if (sala.Estado.Fase != "Noite") throw new Exception("A fase atual não permite essa ação!");

            var jogador = sala.Jogadores.FirstOrDefault(j => j.Nome == nomeJogador)
                ?? throw new Exception("Jogador não encontrado!");

            if (!jogador.Vivo)
                throw new Exception("O Monstro não pode atacar um jogador morto!");

            var monstro = sala.Jogadores.FirstOrDefault(j => j.Papel == "Monstro" && j.Vivo);
            if (monstro == null)
                throw new Exception("Nenhum monstro vivo encontrado na sala!");

            if (monstro.Nome == nomeJogador)
                throw new Exception("O Monstro não pode atacar a si mesmo!");

            sala.Estado.Vitima = nomeJogador;
        }

        public void AnjoProteger(string codigoSala, string nomeJogador)
        {
            var sala = ObterSala(codigoSala) ?? throw new Exception("Sala não encontrada!");
            if (sala.Estado.Fase != "Noite") throw new Exception("A fase atual não permite essa ação!");

            if (string.IsNullOrEmpty(sala.Estado.Vitima))
                throw new Exception("O Anjo só pode proteger após o Monstro atacar!");

            var jogador = sala.Jogadores.FirstOrDefault(j => j.Nome == nomeJogador)
                ?? throw new Exception("Jogador não encontrado!");

            if (!jogador.Vivo)
                throw new Exception("O Anjo não pode proteger um jogador morto!");

            sala.Estado.Protegido = nomeJogador;
        }

        public bool DetetiveInvestigar(string codigoSala, string nomeJogador)
        {
            var sala = ObterSala(codigoSala) ?? throw new Exception("Sala não encontrada!");
            if (sala.Estado.Fase != "Noite") throw new Exception("A fase atual não permite essa ação!");

            var jogador = sala.Jogadores.FirstOrDefault(j => j.Nome == nomeJogador)
                ?? throw new Exception("Jogador não encontrado!");

            if (!jogador.Vivo)
                throw new Exception("O Detetive não pode investigar um jogador morto!");

            sala.Estado.Investigado = nomeJogador;
            return jogador.Papel == "Monstro";
        }

        public void Votar(string codigoSala, string nomeJogador, string nomeVotado)
        {
            var sala = ObterSala(codigoSala) ?? throw new Exception("Sala não encontrada!");

            if (sala.Estado.Fase != "Dia")
                throw new Exception("A fase atual não permite votos!");

            var jogador = sala.Jogadores.FirstOrDefault(j => j.Nome == nomeJogador)
                ?? throw new Exception("Jogador não encontrado!");

            var jogadorVotado = sala.Jogadores.FirstOrDefault(j => j.Nome == nomeVotado)
                ?? throw new Exception("Jogador votado não encontrado!");

            // Verificar se o jogador está vivo
            if (!jogador.Vivo)
                throw new Exception("Jogadores mortos não podem votar!");

            // Verificar se o jogador votado está vivo
            if (!jogadorVotado.Vivo)
                throw new Exception("Não é possível votar em um jogador que está morto!");

            // Verificar se o jogador já votou
            if (sala.Estado.Votos.ContainsKey(nomeJogador))
                throw new Exception("Jogador já votou e não pode votar novamente!");

            // Adicionar o voto (quem votou -> em quem votou)
            sala.Estado.Votos[nomeJogador] = nomeVotado;
        }

        public void EnviarMensagem(string codigoSala, string nomeJogador, string conteudo)
        {
            var sala = ObterSala(codigoSala) ?? throw new Exception("Sala não encontrada!");

            if (sala.Estado.Fase != "Dia")
                throw new Exception("A fase atual não permite votos!");

            var jogador = sala.Jogadores.FirstOrDefault(j => j.Nome == nomeJogador)
                ?? throw new Exception("Jogador não encontrado!");

            // Verificar se o jogador está vivo
            if (!jogador.Vivo)
                throw new Exception("Morto não fala!");

            // Criar a mensagem com timestamp
            var mensagem = new Mensagem
            {
                NomeJogador = nomeJogador,
                Conteudo = conteudo,
            };

            // Adicionar a mensagem na lista
            sala.Estado.Mensagens.Add(mensagem);
        }

        public void EnviarMensagemSistema(string codigoSala, string conteudo)
        {
            var sala = ObterSala(codigoSala) ?? throw new Exception("Sala não encontrada!");

            // Criar a mensagem com timestamp
            var mensagem = new Mensagem
            {
                NomeJogador = "Sistema",
                Conteudo = conteudo,
            };

            // Adicionar a mensagem na lista
            sala.Estado.Mensagens.Add(mensagem);
        }


        public string ProcessarTurno(string codigoSala)
        {
            var sala = ObterSala(codigoSala) ?? throw new Exception("Sala não encontrada!");
            if (sala.Estado.Fase != "Noite") throw new Exception("A fase atual não permite finalizar a noite!");

            // Verifica se algum jogador marcado pelo monstro não foi salvo
            var vitima = sala.Estado.Vitima;
            var protegido = sala.Estado.Protegido;

            if (vitima != null && vitima != protegido)
            {
                var jogador = sala.Jogadores.FirstOrDefault(j => j.Nome == vitima);
                if (jogador != null) jogador.Vivo = false;
            }

            // Muda a fase para "Dia"
            MudarFase(codigoSala, "Dia");

            var result = vitima != protegido ? $"{vitima} foi morto!" : "Ninguém morreu esta noite";

            // Verifica condições de vitória
            var vivos = sala.Jogadores.Where(j => j.Vivo).ToList();
            var monstro = vivos.FirstOrDefault(j => j.Papel == "Monstro");
            if (monstro != null && vivos.Count <= 3)
            {
                return $"{result}. O monstro venceu!";
            }

            if (vivos.All(j => j.Papel != "Monstro"))
            {
                return $"{result}. A cidade venceu!";
            }

            return $"{result}. O jogo continua!";
        }

        public string ApurarVotos(string codigoSala)
        {
            var sala = ObterSala(codigoSala) ?? throw new Exception("Sala não encontrada!");
            if (sala.Estado.Fase != "Dia")
                throw new Exception("A apuração só pode ser feita na fase de Dia!");

            var totalVivos = sala.Jogadores.Count(j => j.Vivo);
            var totalVotos = sala.Estado.Votos.Count;

            // Verificar se pelo menos metade dos jogadores vivos votou
            if (totalVotos < Math.Ceiling(totalVivos / 2.0))
                return "Votação inconclusiva. Nem metade dos jogadores votou.";

            // Contar votos e verificar empate
            var votosAgrupados = sala.Estado.Votos
                .GroupBy(v => v.Value) // Agrupar pelo nome do jogador votado
                .OrderByDescending(g => g.Count()) // Ordenar pelos votos recebidos
                .ToList();

            if (votosAgrupados.Count > 1 && votosAgrupados[0].Count() == votosAgrupados[1].Count())
                return "A votação resultou em empate. Ninguém foi eliminado.";

            var votos = votosAgrupados.FirstOrDefault();
            if (votos == null || string.IsNullOrEmpty(votos.Key))
                throw new Exception("Nenhum voto foi registrado.");

            var jogadorEliminado = sala.Jogadores.FirstOrDefault(j => j.Nome == votos.Key)
                ?? throw new Exception("Jogador votado não encontrado na sala.");

            // Eliminar o jogador mais votado
            jogadorEliminado.Vivo = false;

            // Checar se o jogador eliminado é o monstro
            if (jogadorEliminado.Papel == "Monstro")
            {
                sala.JogoIniciado = false; // Finalizar o jogo
                sala.Estado.Fase = "Finalizado";
                return $"A cidade venceu! O monstro ({jogadorEliminado.Nome}) foi eliminado!";
            }

            // Verificar condição de vitória do monstro
            var jogadoresVivos = sala.Jogadores.Where(j => j.Vivo).ToList();
            var monstroVivo = jogadoresVivos.FirstOrDefault(j => j.Papel == "Monstro");
            if (monstroVivo != null && jogadoresVivos.Count == 3)
            {
                sala.JogoIniciado = false; // Finalizar o jogo
                sala.Estado.Fase = "Finalizado";
                return "O monstro venceu! Apenas ele e mais dois jogadores restam.";
            }

            // Mudar para a próxima fase (Noite)
            MudarFase(codigoSala, "Noite");

            return $"O jogador {jogadorEliminado.Nome} foi eliminado.";
        }

        private void MudarFase(string codigoSala, string novaFase)
        {
            var sala = ObterSala(codigoSala) ?? throw new Exception("Sala não encontrada!");
            sala.Estado.Fase = novaFase;

            // Resetar atributos específicos para a nova fase
            if (novaFase == "Noite")
            {
                sala.Estado.Vitima = null;
                sala.Estado.Protegido = null;
                sala.Estado.Investigado = null;
            }
            else if (novaFase == "Dia")
            {
                sala.Estado.Votos.Clear();
            }
        }

        public List<Sala> ObterTodasAsSalas()
        {
            return _salas.Values.ToList();
        }


    }

}