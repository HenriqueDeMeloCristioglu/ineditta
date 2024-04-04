namespace Ineditta.Application.Usuarios.Templates
{
    internal static class EmailBoasVindasTemplate
    {
        internal const string Html = @"<p>Prezado(a) Usuário(a) @Model.Username,</p>
<p>Encaminhamos abaixo o seu acesso para o Sistema Ineditta.</p>
<p><strong>Link de acesso:</strong> <a target='_blank' href='@Model.Url'>@Model.Url</a></p>
<p><strong>Usuário:</strong> @Model.AuthUsername ou @Model.Email</p>
<p><strong>Senha:</strong> @Model.Senha</p>
<p>Ao clicar no <strong>link</strong> acima, você acessa a página inicial onde valida por e-mail seu usuário, e só após efetua o seu primeiro acesso na página principal, o sistema exige que você altere a senha para uma de uso pessoal.</p>
<p>O padrão da senha exige no mínimo 6 caracteres, sendo 1 letra e 1 número.</p>
<p><strong>Observações:</strong> Para acessar o sistema recomendamos o navegador Google Chrome.</p>
<p>Em caso de dúvidas, estamos à disposição.</p>
<p>Atenciosamente,</p>
<p><strong>Ineditta Consultoria Sindical</strong></p>";
    }
}
