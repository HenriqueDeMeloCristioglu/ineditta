using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.UpsertSisap
{
    public class UpsertSisapDocumentoSindicalRequest : IRequest<Result<int>>
    {
        public int Tipo { get; set; }
        public string? Origem { get; set; } = null!;
        public string Versao { get; set; } = null!;
        public string? NumeroSolicitacaoMR { get; set; }
        public DateOnly? DataRegistro { get; set; }
        public string? NumeroRegistro { get; set; }
        public DateOnly? Prorrogacao { get; set; }
        public string? Permissao { get; set; }
        public string? Observacao { get; set; }
        public int? IdDocSind { get; set; }
        public int IdDocumento { get; set; }
        public string DataBase { get; set; } = null!;
        public bool Restrito { get; set; }
        public IEnumerable<int>? ClienteEstabelecimento { get; set; }
        public IEnumerable<string>? Referencia { get; set; }
        public IEnumerable<int>? SindLaboral { get; set; }
        public IEnumerable<int>? SindPatronal { get; set; }
        public string? Cnaes { get; set; }
        public IEnumerable<int>? CnaesIds { get; set; }
        public string? Uf { get; set; }
        public required DateOnly ValidadeInicial { get; set; }
        public required DateOnly ValidadeFinal { get; set; }
        public DateOnly? DataAssinatura { get; set; }
        public string? NomeDocumento { get; set; } = null!;
        public string? Caminho { get; set; }
        public string? Nome { get; set; }
        public IEnumerable<int>? Abrangencia { get; set; }
        public int? UsuarioResponsavel { get; set; }
        public int? TipoUnidadeCliente { get; set; }
        public string? NomeArquivo { get; set; }
    }
}
