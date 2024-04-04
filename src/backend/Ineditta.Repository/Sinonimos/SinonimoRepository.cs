using System.Globalization;
using System.Text;

using Ineditta.Application.Sinonimos.Entities;
using Ineditta.Application.Sinonimos.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

using MySqlConnector;

namespace Ineditta.Repository.Sinonimos
{
    public class SinonimoRepository : ISinonimoRepository
    {
        private readonly InedittaDbContext _context;

        public SinonimoRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(Sinonimo sinonimo)
        {
            _context.Sinonimos.Update(sinonimo);

            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(Sinonimo sinonimo)
        {
            _context.Sinonimos.Add(sinonimo);

            await Task.CompletedTask;
        }

        public async ValueTask<Sinonimo?> ObterPorIdAsync(long id)
        {
            return await _context.Sinonimos.Where(e => e.Id == id).SingleOrDefaultAsync();
        }

        public async ValueTask<Sinonimo?> ObterPorNomeExatoAsync(string nome)
        {
            var parameters = new List<MySqlParameter>();

            var queryBuilder = new StringBuilder(@"
                SELECT * FROM sinonimos
                WHERE upper(trim(nome_sinonimo)) LIKE upper(trim(@nome))
            "
            );

            parameters.Add(new MySqlParameter("@nome", nome));

            var sinonimo = await _context.Sinonimos.FromSqlRaw(queryBuilder.ToString(), parameters.ToArray()).FirstOrDefaultAsync();

            return sinonimo is not null ? sinonimo : null;
        }

        public async ValueTask<Sinonimo?> ObterPorNomeAproximadoAsync(string nome)
        {
            nome = nome.Trim().ToUpper(CultureInfo.InvariantCulture);

            var parametersSoundsLike = new List<MySqlParameter>();

            var queryBuilderSoundsLike = new StringBuilder(@"
                SELECT * FROM sinonimos
                WHERE nome_sinonimo SOUNDS LIKE @nome
                ORDER BY SOUNDEX(nome_sinonimo) DESC, ABS(length(nome_sinonimo) - length(@nome)) ASC
            "
            );

            parametersSoundsLike.Add(new MySqlParameter("@nome", nome));

            var sinonimoSoundsLike = await _context.Sinonimos.FromSqlRaw(queryBuilderSoundsLike.ToString(), parametersSoundsLike.ToArray()).FirstOrDefaultAsync();

            if (sinonimoSoundsLike is not null)
            {
                return sinonimoSoundsLike;
            }

            var parametersMatchAgainst = new List<MySqlParameter>();

            var queryBuilderMatchAgainst = new StringBuilder(@"
                SELECT * FROM sinonimos
                WHERE MATCH(nome_sinonimo) AGAINST(@nome IN BOOLEAN MODE)
                ORDER BY MATCH(nome_sinonimo) AGAINST(@nome IN BOOLEAN MODE) DESC, ABS(length(nome_sinonimo) - length(@nome)) ASC
            "
            );

            parametersMatchAgainst.Add(new MySqlParameter("@nome", nome));

            var sinonimoMatchAgainst = await _context.Sinonimos.FromSqlRaw(queryBuilderMatchAgainst.ToString(), parametersMatchAgainst.ToArray()).FirstOrDefaultAsync();

            return sinonimoMatchAgainst is not null ? sinonimoMatchAgainst : null;
        }

        public async ValueTask<Sinonimo?> ObterPorEstruturaClausulaIdENomeAsync(int estruturaClausulaId, string nome)
        {
            nome = nome.Trim().ToUpper(CultureInfo.InvariantCulture);

            var parameters = new List<MySqlParameter>();

            var queryBuilder = new StringBuilder(@"
                SELECT * FROM sinonimos
                WHERE estrutura_clausula_id_estruturaclausula = @estruturaClausulaId AND UPPER(TRIM(nome_sinonimo)) like @nome
            ");

            parameters.Add(new MySqlParameter("@estruturaClausulaId", estruturaClausulaId));
            parameters.Add(new MySqlParameter("@nome", nome));

            var sinonimo = await _context.Sinonimos.FromSqlRaw(queryBuilder.ToString(), parameters.ToArray()).FirstOrDefaultAsync();

            return sinonimo is not null ? sinonimo : null;
        }

        public async ValueTask<IEnumerable<Sinonimo>?> ObterPorIdEstruturaClausulaAsync(long id, long estruturaClausulaId)
        {
            return await _context.Sinonimos.Where(s => s.Id == id && s.EstruturaClausulaId == estruturaClausulaId).ToListAsync();
        }
    }
}
