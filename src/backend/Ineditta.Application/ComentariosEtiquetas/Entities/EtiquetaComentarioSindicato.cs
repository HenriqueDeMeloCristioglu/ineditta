using CSharpFunctionalExtensions;

namespace Ineditta.Application.ComentariosEtiquetas.Entities
{
    public class EtiquetaComentarioSindicato : Entity
    {
        public string Valor { get; set; } = null!;

        private EtiquetaComentarioSindicato(string valor)
        {
            Valor = valor;
        }

        protected EtiquetaComentarioSindicato() { }

        public static Result<EtiquetaComentarioSindicato> Criar(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return Result.Failure<EtiquetaComentarioSindicato>("Você deve fornecer o valor da etiqueta");
            if (valor.Length > 70) return Result.Failure<EtiquetaComentarioSindicato>("O valor da etiqueta não deve ser maior que 70 caracteres");

            var etiquetaComentarioSindicato = new EtiquetaComentarioSindicato(valor);

            return Result.Success(etiquetaComentarioSindicato);
        }
    }
}
