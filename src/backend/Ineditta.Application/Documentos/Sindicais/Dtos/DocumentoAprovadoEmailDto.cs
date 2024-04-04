using Ineditta.Application.ClientesUnidades.Entities;

namespace Ineditta.Application.Documentos.Sindicais.Dtos
{
    public class DocumentoAprovadoEmailDto
    {
        public string? NomeDocumento { get; set; }
        public string? Abrangencia { get; set; }
        public string? AtividadesEconomicas { get; set; }
        public string? SindicatosPatronais { get; set; }
        public string? SindicatosLaborais { get; set; }
        public DateOnly? VigenciaInicial { get; set; }
        public DateOnly? VigenciaFinal { get; set; }
        public DateOnly? DataSla { get; set; }
        public string? Url { get; set; }
        public string? GestaoDeChamadosUrl { get; set; }
        public IEnumerable<ClienteUnidade>? Estabelecimentos { get; set; } = null!;
        public string? CodigosSindicatosCliente { get; set; }
        public string Assunto { get; set; } = null!;
        public Dictionary<int,string>? LocalizacoesUnidades { get; set; }
    }
}
