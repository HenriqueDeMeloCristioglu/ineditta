using CSharpFunctionalExtensions;

using Ineditta.API.ViewModels.InformacoesAdicionaisClientes.ViewModels;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.API.Factories.InformacoesAdicionais.Cliente
{
    public class InformacoesAdicionaisClienteFactory
    {
        private readonly InedittaDbContext _context;

        public InformacoesAdicionaisClienteFactory(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask<InformacaoAdicionalClienteViewModel?> CriarAsync(int id, Usuario usuario)
        {
            var result = await(from ifac in _context.InformacaoAdicionalCliente
                               join us in _context.UsuarioAdms on EF.Property<int>(ifac, "UsuarioAlteracaoId") equals us.Id into _us
                               from us in _us.DefaultIfEmpty()
                               where (ifac.DocumentoSindicalId == id) && (usuario.GrupoEconomicoId != null && (ifac.GrupoEconomicoId == usuario.GrupoEconomicoId!.Value))
                               select new InformacaoAdicionalClienteViewModel
                               {
                                   DocumentoSindicalId = ifac.DocumentoSindicalId,
                                   GrupoEconomicoId = ifac.GrupoEconomicoId,
                                   Aprovado = ifac.Aprovado,
                                   InformacoesAdicionais = ifac.InformacoesAdicionais,
                                   ObservacoesAdicionais = ifac.ObservacoesAdicionais,
                                   DataUltimaAlteracao = EF.Property<DateTime?>(ifac, "DataAlteracao") == null ? DateOnly.FromDateTime(EF.Property<DateTime>(ifac, "DataInclusao")) : DateOnly.FromDateTime(EF.Property<DateTime>(ifac, "DataAlteracao")),
                                   NomeUsuario = us != null ? us.Nome : string.Empty,
                                   Orientacao = ifac.Orientacao,
                                   OutrasInformacoes = ifac.OutrasInformacoes
                               }).FirstOrDefaultAsync();

            if (result == null)
            {
                return null;
            }

            return result;
        }
    }
}
