using CSharpFunctionalExtensions;

namespace Ineditta.Application.ComentariosEtiquetas.Entities
{
    public class EtiquetaComentarioClausula : Entity
    {
        public string Valor { get; set; } = null!;

        private EtiquetaComentarioClausula(string valor)
        {
            Valor = valor;
        }

        protected EtiquetaComentarioClausula() { }

        public static Result<EtiquetaComentarioClausula> Criar(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return Result.Failure<EtiquetaComentarioClausula>("Você deve fornecer o valor da etiqueta");
            if (valor.Length > 70) return Result.Failure<EtiquetaComentarioClausula>("O valor da etiqueta não deve ser maior que 70 caracteres");

            var etiquetaComentarioClausula = new EtiquetaComentarioClausula(valor);

            return Result.Success(etiquetaComentarioClausula);
        }
    }
}
