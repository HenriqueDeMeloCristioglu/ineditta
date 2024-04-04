using Ineditta.BuildingBlocks.Core.Domain.Models;

namespace Ineditta.Integration.Auth.Erros
{
    internal static class AuthErrors
    {
        public static Error DuplicatedEmail() => Error.Create("email.duplicated", "Email já cadastrado");

        public static Error DuplicatedUsername() => Error.Create("username.duplicated", "Username já cadastrado");

        public static Error UserNotFound() => Error.Create("username.duplicated", "Usuário não foi encontrado");

        public static Error EmailUpdateCredential() => Error.Create("email.update.credential", "Não foi possível enviar o e-mail de atualização das credenciais");
    }
}
