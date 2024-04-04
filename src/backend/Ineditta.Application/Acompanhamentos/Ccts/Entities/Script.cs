using CSharpFunctionalExtensions;

namespace Ineditta.Application.AcompanhamentosCcts.Entities
{
    public class Script
    {
        public const int IndiceDataAssembleiaPatronal = 9;
        public const int IndiceHoraAssembleiaPatronal = 11;
        public const int IndiceDataReuniaoEntrePartes = 16;
        public const int IndiceHoraReuniaoEntrePartes = 17;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Script()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        public string Fase { get; private set; }
        public IEnumerable<string>? Respostas { get; private set; }
        public DateTime Horario { get; private set; }
        public string NomeUsuario { get; private set; }

        private Script(string fase, IEnumerable<string>? respostas, DateTime horario, string nomeUsuario)
        {
            Fase = fase;
            Respostas = respostas;
            Horario = horario;
            NomeUsuario = nomeUsuario;
        }

        public static Result<Script> Gerar(string fase, IEnumerable<string>? respostas, DateTime horario, string nomeUsuario)
        {
            if (fase is null) return Result.Failure<Script>("Campo fase em branco");
            if (horario == DateTime.MinValue) return Result.Failure<Script>("Campo horario em branco");
            if (nomeUsuario is null) return Result.Failure<Script>("Campo nome de usuário em branco");

            var script = new Script(fase, respostas, horario, nomeUsuario);

            return Result.Success(script);
        }
    }
}
