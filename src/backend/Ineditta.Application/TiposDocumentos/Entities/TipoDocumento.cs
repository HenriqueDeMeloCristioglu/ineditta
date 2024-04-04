using CSharpFunctionalExtensions;

namespace Ineditta.Application.TiposDocumentos.Entities
{
    public class TipoDocumento : Entity<int>
    {
        private TipoDocumento(string grupo, string nome, string? sigla, bool processado, TipoModuloCadastro moduloCadastro)
        {
            Grupo = grupo;
            Nome = nome;
            Sigla = sigla;
            Processado = processado;
            ModuloCadastro = moduloCadastro;
        }

        protected TipoDocumento() { }

        public string Grupo { get; private set; } = null!;
        public string Nome { get; private set; } = null!;
        public string? Sigla { get; private set; }
        public bool Processado { get; private set; }
        public TipoModuloCadastro ModuloCadastro { get; private set; }

        public static Result<TipoDocumento> Criar(string grupo, string nome, string? sigla, bool processado, TipoModuloCadastro moduloCadastro)
        {
            if (string.IsNullOrEmpty(grupo)) return Result.Failure<TipoDocumento>("Você precisa fornecer o grupo do tipo do documento");
            if (string.IsNullOrEmpty(nome)) return Result.Failure<TipoDocumento>("Você precisa fornecer o novo do tipo do documento");

            var tipoDocumento = new TipoDocumento(
                grupo,
                nome,
                sigla,
                processado,
                moduloCadastro
            );

            return Result.Success(tipoDocumento);
        }

        public Result Atualizar(string grupo, string nome, string? sigla, bool processado, TipoModuloCadastro moduloCadastro)
        {
            if (string.IsNullOrEmpty(grupo)) return Result.Failure<TipoDocumento>("Você precisa fornecer o grupo do tipo do documento");
            if (string.IsNullOrEmpty(nome)) return Result.Failure<TipoDocumento>("Você precisa fornecer o novo do tipo do documento");

            Grupo = grupo;
            Nome = nome;
            Sigla = sigla;
            Processado = processado;
            ModuloCadastro = moduloCadastro;

            return Result.Success();
        }
    }
}
