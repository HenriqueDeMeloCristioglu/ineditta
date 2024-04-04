using CSharpFunctionalExtensions;

using Ineditta.Application.Usuarios.Entities;

namespace Ineditta.Application.CalendarioSindicais.Usuarios.Entities
{
    public class CalendarioSindicalUsuario : Entity
    {
        private CalendarioSindicalUsuario(string titulo, DateTime dataHora, TipoRecorrencia? recorrencia, DateTime? validadeRecorrencia, TimeSpan? notificarAntes, string local, string? comentarios, bool visivel, Usuario usuario)
        {
            Titulo = titulo;
            DataHora = dataHora;
            Recorrencia = recorrencia;
            ValidadeRecorrencia = validadeRecorrencia;
            NotificarAntes = notificarAntes;
            Local = local;
            Comentarios = comentarios;
            Visivel = visivel;
            IdUsuario = usuario.Id;
        }

        protected CalendarioSindicalUsuario() { }

        public string Titulo { get; private set; } = null!;
        public DateTime DataHora { get; private set; }
        public TipoRecorrencia? Recorrencia { get; private set; }
        public DateTime? ValidadeRecorrencia { get; set; }
        public TimeSpan? NotificarAntes { get; private set; }
        public string Local { get; private set; } = null!;
        public string? Comentarios { get; private set; } = null!;
        public bool Visivel { get; private set; }
        public long IdUsuario { get; private set; }

        public static Result<CalendarioSindicalUsuario> Criar(string titulo, DateTime dataHora, TipoRecorrencia? recorrencia, DateTime? validadeRecorrencia, TimeSpan? notificarAntes, string local, string? comentarios, bool visivel, Usuario usuario)
        {
            if (usuario is null) return Result.Failure<CalendarioSindicalUsuario>("É necessario que haja um usuário atrelado a este evento.");
            if (titulo == null) return Result.Failure<CalendarioSindicalUsuario>("Você precisa fornecer um título para o evento.");
            if (recorrencia is not null && (int)recorrencia.Value > 1 && validadeRecorrencia is null) return Result.Failure<CalendarioSindicalUsuario>("Ao determinar que um evento é recorrente você deve escolher uma data de validade para a recorrência.");
            if (recorrencia is not null && (int)recorrencia.Value > 1 && validadeRecorrencia < dataHora) return Result.Failure<CalendarioSindicalUsuario>("A validade deve ser maior que a Data e Hora do evento.");
            if (dataHora < DateTime.Now) return Result.Failure<CalendarioSindicalUsuario>("Você não pode criar um evento para o passado.");

            CalendarioSindicalUsuario evento = new(titulo, dataHora, recorrencia, validadeRecorrencia, notificarAntes, local, comentarios, visivel, usuario);
            return Result.Success(evento);
        }

        public Result Atualizar(string titulo, DateTime dataHora, TipoRecorrencia? recorrencia, DateTime? validadeRecorrencia, TimeSpan? notificarAntes, string local, string? comentarios, bool visivel, Usuario usuario)
        {
            if (usuario is null) return Result.Failure<CalendarioSindicalUsuario>("É necessario que haja um usuário atrelado a este evento.");
            if (titulo == null) return Result.Failure<CalendarioSindicalUsuario>("Você precisa fornecer um título para o evento.");
            if (recorrencia is not null && (int)recorrencia.Value > 1 && validadeRecorrencia is null) return Result.Failure<CalendarioSindicalUsuario>("Ao determinar que um evento é recorrente você deve escolher uma data de validade para a recorrência.");
            if (recorrencia is not null && (int)recorrencia.Value > 1 && validadeRecorrencia < dataHora) return Result.Failure<CalendarioSindicalUsuario>("A validade deve ser maior que a Data e Hora do evento.");
            if (dataHora < DateTime.Now) return Result.Failure<CalendarioSindicalUsuario>("Você não pode criar um evento para o passado.");

            Titulo = titulo;
            DataHora = dataHora;
            Recorrencia = recorrencia;
            ValidadeRecorrencia = validadeRecorrencia;
            NotificarAntes = notificarAntes;
            Local = local;
            Comentarios = comentarios;
            Visivel = visivel;
            IdUsuario = usuario.Id;

            return Result.Success();
        }
    }
}
