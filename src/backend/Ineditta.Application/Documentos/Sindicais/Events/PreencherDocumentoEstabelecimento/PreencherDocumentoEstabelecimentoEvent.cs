using Ineditta.BuildingBlocks.Core.Bus;

namespace Ineditta.Application.Documentos.Sindicais.Events.PreencherDocumentoEstabelecimento
{
    public class PreencherDocumentoEstabelecimentoEvent : Message
    {
        public int DocumentoId { get; set; }
        public IEnumerable<int>? CnaesIds { get; set; }
        public IEnumerable<int>? SindicatosLaboraisIds { get; set; }
        public IEnumerable<int>? SindicatosPatronaisIds { get; set; }
        public IEnumerable<int>? LocalizacoesIds { get; set; }
        public string EmailUsuario { get; set; }
        public IEnumerable<long>? UsuariosParaNotificarIds { get; set; }

        public PreencherDocumentoEstabelecimentoEvent(int documentoId, IEnumerable<int>? cnaesIds, IEnumerable<int>? localizacoesIds, IEnumerable<int>? sindicatosLaboraisIds, IEnumerable<int>? sindicatosPatronaisIds, string emailUsuario, IEnumerable<long>? usuariosParaNotificarIds)
        {
            DocumentoId = documentoId;
            CnaesIds = cnaesIds;
            SindicatosLaboraisIds = sindicatosLaboraisIds;
            SindicatosPatronaisIds = sindicatosPatronaisIds;
            LocalizacoesIds = localizacoesIds;
            EmailUsuario = emailUsuario;
            UsuariosParaNotificarIds = usuariosParaNotificarIds;
        }
    }
}
