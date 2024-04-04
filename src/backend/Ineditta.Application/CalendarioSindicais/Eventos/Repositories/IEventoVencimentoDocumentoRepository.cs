using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.CalendarioSindicais.Eventos.Dtos;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Repositories
{
    public interface IEventoVencimentoDocumentoRepository
    {
        ValueTask<IEnumerable<string>> ObterListaEmailNotificacao(long documentoId);
        ValueTask<IEnumerable<VencimentoDocumentoViewModel>> ObterNotificacoesVencimentoDocumento();
    }
}
