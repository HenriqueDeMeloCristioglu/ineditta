using System.Text;

using Ineditta.Application.TemplatesEmails.Entities;
using Ineditta.Application.TemplatesEmails.Repositories;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

using MySqlConnector;

namespace Ineditta.Repository.TemplatesEmails
{
    public class TemplateEmailRepository : ITemplateEmailRepository
    {
        private readonly InedittaDbContext _context;

        public TemplateEmailRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask<TemplateEmail?> ObterPorUsuarioEmailAsync(string email, TipoTemplateEmail tipoTemplate)
        {
            var parameters = new List<MySqlParameter>();

            var query = new StringBuilder(@"
                SELECT tet.* FROM template_email_tb tet 
                INNER JOIN usuario_adm ua ON ua.email_usuario = @emailUsuario
                WHERE CASE WHEN ua.nivel = 'Grupo Econômico' THEN tet.nivel = @nivelGrupo AND tet.referencia_id = ua.id_grupoecon 
                           WHEN ua.nivel = 'Matriz' THEN tet.nivel = @nivelMatriz AND tet.referencia_id IN (
           	                    SELECT cut.cliente_matriz_id_empresa FROM cliente_unidades cut
           	                    WHERE JSON_CONTAINS(ua.ids_fmge, JSON_ARRAY(cut.id_unidade))
           	                    GROUP BY cut.cliente_matriz_id_empresa
                           )
                           WHEN ua.nivel = 'Unidade' THEN tet.nivel = @nivelUnidade AND JSON_CONTAINS(ua.ids_fmge, JSON_ARRAY(tet.referencia_id))
                           ELSE FALSE
                      END
                      AND tet.tipo_template = @tipoTemplate
            ");

            parameters.Add(new MySqlParameter("@emailUsuario", email));
            parameters.Add(new MySqlParameter("@nivelGrupo", (int)Nivel.GrupoEconomico));
            parameters.Add(new MySqlParameter("@nivelMatriz", (int)Nivel.Matriz));
            parameters.Add(new MySqlParameter("@nivelUnidade", (int)Nivel.Unidade));
            parameters.Add(new MySqlParameter("@tipoTemplate", (int)tipoTemplate));

            return await _context.TemplatesEmails.FromSqlRaw(query.ToString(), parameters.ToArray())
                            .Select(t => t)
                            .SingleOrDefaultAsync();
        }
    }
}
