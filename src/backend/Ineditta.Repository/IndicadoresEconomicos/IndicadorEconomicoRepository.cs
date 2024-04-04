using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.IndicadoresEconomicos.Entities;
using Ineditta.Application.IndicadoresEconomicos.Repositories;
using Ineditta.Repository.Contexts;
using Ineditta.Repository.Models;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.IndicadoresEconomicos
{
    public class IndicadorEconomicoRepository : IIndicadorEconomicoRepository
    {
        private readonly InedittaDbContext _context;
    
        public IndicadorEconomicoRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask IncluirAsync(IndicadorEconomico indicadorEconomico)
        {
            _context.Indecons.Add(ToModel(indicadorEconomico));
            await _context.SaveChangesAsync();

            await Task.CompletedTask;
        }
        public async ValueTask<IndicadorEconomico> ObterAsync(int id)
        {
            var result = await _context.Indecons.AsNoTracking().SingleOrDefaultAsync(x => x.IdIndecon == id);
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
            return result == null ? default : new IndicadorEconomico(result.IdIndecon, result.Origem, result.Indicador, result.IdUsuario, result.Fonte, result.Data, result.DadoProjetado, result.CriadoEm);
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8603 // Possible null reference return.
        }
        public async ValueTask<bool> ExisteAsync(int id)
        {
            return await Task.FromResult(_context.Indecons.Any(x => x.IdIndecon == id));
        }
        public async ValueTask AtualizarAsync(IndicadorEconomico indicadorEconomico)
        {
            var model = ToModel(indicadorEconomico);
            model.IdIndecon = (int)indicadorEconomico.Id;

            _context.Indecons.Update(model);
            await _context.SaveChangesAsync();

            await Task.CompletedTask;
        }

        private static Func<IndicadorEconomico, Indecon> ToModel => data => new Indecon
        {
            IdIndecon = (int)data.Id,
            Origem = data.Origem,
            Indicador = data.Indicador,
            Fonte = data.Fonte,
            Data = data.Data,
            DadoProjetado = data.DadoProjetado,
            CriadoEm = data.CriadoEm,
        };
    }
}
