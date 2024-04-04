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
    public class IndicadorEconomicoRealRepository : IIndicadorEconomicoRealRepository
    {
        private readonly InedittaDbContext _context;

        public IndicadorEconomicoRealRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask IncluirAsync(IndicadorEconomicoReal indicadorEconomico)
        {
            _context.IndeconReals.Add(ToModel(indicadorEconomico));
            await _context.SaveChangesAsync();

            await Task.CompletedTask;
        }
        public async ValueTask<IndicadorEconomicoReal> ObterAsync(int id)
        {
            var result = await _context.IndeconReals.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
#pragma warning disable CS8603 // Possible null reference return.
            return result == null ? default : new IndicadorEconomicoReal(result.Id, result.Indicador, result.PeriodoData, result.DadoReal, result.CriadoEm);
#pragma warning restore CS8603 // Possible null reference return.
        }
        public async ValueTask<bool> ExisteAsync(int id)
        {
            return await Task.FromResult(_context.IndeconReals.Any(x => x.Id == id));
        }
        public async ValueTask AtualizarAsync(IndicadorEconomicoReal indicadorEconomico)
        {
            var model = ToModel(indicadorEconomico);
            model.Id = (int)indicadorEconomico.Id;

            _context.IndeconReals.Update(model);
            await _context.SaveChangesAsync();

            await Task.CompletedTask;
        }

        private static Func<IndicadorEconomicoReal, IndeconReal> ToModel => data => new IndeconReal
        {
            Id = (int)data.Id,
            Indicador = data.Indicador,
            PeriodoData = data.Data,
            DadoReal = data.DadoReal,
            CriadoEm = data.CriadoEm,
        };
    }
}
