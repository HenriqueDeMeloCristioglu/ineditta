using CSharpFunctionalExtensions;

namespace Ineditta.Application.Jornada.Entities
{
    public class Jornada : Entity
    {
        protected Jornada()
        {
        }
        
        private Jornada(string? jornadaSemanal, string? descricao, int isDeault)
        {
            JornadaSemanal = jornadaSemanal;
            Descricao = descricao;
            IsDeault = isDeault;
        }

        public string? JornadaSemanal { get; private set; }
        public string? Descricao { get; private set; }
        public int IsDeault { get; private set; }

        public static Result<Jornada> Criar(string? jornadaSemanal, string? descricao, int isDeault)
        {
            if (isDeault < 0) return Result.Failure<Jornada>("Informe o isDeault");

            var jornada = new Jornada(jornadaSemanal, descricao, isDeault);

            return Result.Success(jornada);
        }
    }
}
