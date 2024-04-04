using Ineditta.Application.Documentos.Sindicais.Dtos;

namespace Ineditta.API.ViewModels.DocumentosSindicais.ViewModels.Emails
{
    public class DocumentosAprovadosSisapDadosViewModel
    {
        public DocumentoAprovadoSisapEmail Documento { get; set; } = null!;
        public string? UrlArquivo { get; set; }
        public string UrlGestaoDeChamados { get; set; } = null!;
        public string Assunto { get; set; } = null!;
        public IEnumerable<string> Emails { get; set; } = null!;
    }
    public class DocumentoAprovadoSisapEmail
    {
        public IEnumerable<Abrangencia>? AbrangenciaLista { get; set; }
        public IEnumerable<Cnae>? AtividadesEconomicasLista { get; set; } = null!;
        public IEnumerable<SindicatoLaboral>? SindicatoLaboralLista { get; set; }
        public IEnumerable<SindicatoPatronal>? SindicatoPatronalLista { get; set; }
        public string? Abrangencia { get; set; }
        public string? GrupoEconomicoNome { get; set; } = null!;
        public string? NomeDocumento { get; set; } = null!;
        public string? AtividadesEconomicas { get; set; } = null!;
        public DateOnly? VigenciaInicial { get; set; } = null!;
        public DateOnly? VigenciaFinal { get; set; }
        public DateOnly? DataSla { get; set; }
        public string? SindicatoLaboral { get; set; }
        public string? SindicatoPatronal { get; set; }
        public string? Sigla { get; set; }
        public string? DataBase { get; set; }
        public IEnumerable<Estabelecimento>? Estabelecimentos { get; set; }
        public string? Origem { get; set; }
        public DateOnly? DataAprovacao { get; set; }
    }
}
