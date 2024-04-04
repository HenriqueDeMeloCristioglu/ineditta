using CSharpFunctionalExtensions;

namespace Ineditta.Application.Usuarios.Entities
{
    public class UsuarioModulo : Entity
    {
        protected UsuarioModulo()
        {

        }

        private UsuarioModulo(long id, bool criar, bool consultar, bool comentar, bool alterar, bool excluir, bool aprovar)
        {
            Id = id;
            Criar = criar;
            Consultar = consultar;
            Comentar = comentar;
            Alterar = alterar;
            Excluir = excluir;
            Aprovar = aprovar;
        }

        public bool Criar { get; private set; }
        public bool Consultar { get; private set; }
        public bool Comentar { get; private set; }
        public bool Alterar { get; private set; }
        public bool Excluir { get; private set; }
        public bool Aprovar { get; private set; }

        public static Result<UsuarioModulo> Gerar(long id, bool criar, bool consultar, bool comentar, bool alterar, bool excluir, bool aprovar)
        {
            var usuarioModulo = new UsuarioModulo(id, criar, consultar, comentar, alterar, excluir, aprovar);

            return Result.Success(usuarioModulo);
        }
    }
}