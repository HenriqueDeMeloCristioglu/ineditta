using CSharpFunctionalExtensions;

using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.BuildingBlocks.Core.Domain.Contracts;

namespace Ineditta.Application.Documentos.Sindicais.Entities
{
    public sealed class DocumentoSindical : Entity<int>, IAuditable
    {
        public const string PastaDocumento = "documentos_sindicais";

        private List<SindicatoPatronal>? _sindicatoPatronais;
        private List<SindicatoLaboral>? _sindicatoLaborais;
        private List<Cnae>? _cnaes;
        private List<Estabelecimento>? _estabelecimentos;
        private List<Abrangencia>? _abrangencias;
        private List<string>? _referencias;

        private DocumentoSindical(string? versao, DateOnly dataValidadeInicial, DateOnly? dataValidadeFinal, DateOnly? dataProrrogacao, int tipoDocumentoId,
            string? origem, string? numeroSolicitacao, DateOnly? dataRegistroMTE, string? numeroRegistroMTE, DateOnly? dataAssinatura,
            string? permissao, string? observacao, int? documentoLocalizacaoId, string? database, bool restrito,
            IEnumerable<Estabelecimento>? estabelecimentos, IEnumerable<string>? referencias, IEnumerable<SindicatoLaboral>? sindicatoLaborais, IEnumerable<SindicatoPatronal>? sindicatoPatronais, IEnumerable<Cnae>? cnaes, IEnumerable<Abrangencia>? abrangencias,
            TipoModulo modulo, string? uf, string? caminhoArquivo, DateOnly? dataAprovacao, int? tipoNegocioId, string? nomeArquivo, string? numeroLei, string? fonteLei)
        //"SISAP", request.Uf, documentoLocalizado.Caminho, documentoLocalizado.DataAprovacao
        {
            Versao = versao;
            DataValidadeInicial = dataValidadeInicial;
            DataValidadeFinal = dataValidadeFinal;
            DataProrrogacao = dataProrrogacao;
            NumeroSolicitacao = numeroSolicitacao;
            DataAssinatura = dataAssinatura;
            NumeroRegistroMTE = numeroRegistroMTE;
            DataRegistroMTE = dataRegistroMTE;
            Origem = origem;
            Observacao = observacao;
            DataAprovacao = dataAprovacao;
            Permissao = permissao;
            TipoDocumentoId = tipoDocumentoId;
            TipoNegocioId = tipoNegocioId;
            Uf = uf;
            Database = database;
            Restrito = restrito;
            DocumentoLocalizacaoId = documentoLocalizacaoId;
            CaminhoArquivo = caminhoArquivo;
            Modulo = modulo;
            _sindicatoPatronais = sindicatoPatronais?.ToList();
            _sindicatoLaborais = sindicatoLaborais?.ToList();
            _cnaes = cnaes?.ToList();
            _estabelecimentos = estabelecimentos?.ToList();
            _abrangencias = abrangencias?.ToList();
            _referencias = referencias?.ToList();
            NomeArquivo = nomeArquivo;
            NumeroLei = numeroLei;
            FonteWeb = fonteLei;
        }


#pragma warning disable CS0628 // New protected member declared in sealed type
        protected DocumentoSindical()
#pragma warning restore CS0628 // New protected member declared in sealed type
        {
        }

        public string? Versao { get; private set; }

        public DateOnly DataValidadeInicial { get; private set; }

        public DateOnly? DataValidadeFinal { get; private set; }

        public DateOnly? DataProrrogacao { get; private set; }

        public string? NumeroSolicitacao { get; private set; }

        public DateOnly? DataAssinatura { get; private set; }

        public string? NumeroRegistroMTE { get; private set; }
        public DateOnly? DataRegistroMTE { get; private set; }
        public string? Origem { get; private set; }
        public string? Observacao { get; private set; }

        public DateOnly? DataAprovacao { get; private set; }

        public string? Permissao { get; private set; }

        public int? TipoNegocioId { get; private set; }

        public int TipoDocumentoId { get; private set; }

        public string? Uf { get; private set; }

        public string? Database { get; private set; }

        public bool Restrito { get; private set; }

        public int? DocumentoLocalizacaoId { get; private set; }

        public string? SlaEntrega { get; private set; }
        public DateOnly? DataSla { get; private set; }

        public DateOnly? DataScrap { get; private set; }

        public string? ScrapAprovado { get; private set; }

        public int? UsuarioResponsavelId { get; private set; }

        public string? Descricao { get; private set; }

        public string? NumeroLei { get; private set; }

        public string? CaminhoArquivo { get; private set; }

        public string? Anuencia { get; private set; }

        public string? Status { get; private set; }

        public string? FonteWeb { get; private set; }

        public DateTime DataUpload { get; private set; }

        public TipoModulo Modulo { get; private set; }

        public int? UsuarioAprovadorId { get; private set; }

        public string Titulo { get; private set; } = null!;

        public string? FormularioComunicado { get; private set; }

        public DateOnly? DataLiberacao { get; private set; }

        public string? NomeArquivo { get; private set; }
        public DateOnly? DataAtualizacao { get; private set; }


        public IEnumerable<SindicatoPatronal>? SindicatosPatronais => _sindicatoPatronais?.AsReadOnly();
        public IEnumerable<SindicatoLaboral>? SindicatosLaborais => _sindicatoLaborais?.AsReadOnly();
        public IEnumerable<Estabelecimento>? Estabelecimentos => _estabelecimentos?.AsReadOnly();
        public IEnumerable<Cnae>? Cnaes => _cnaes?.AsReadOnly();
        public IEnumerable<Abrangencia>? Abrangencias => _abrangencias?.AsReadOnly();
        public IEnumerable<string>? Referencias => _referencias?.AsReadOnly();
        public static Result<DocumentoSindical> Criar(string versao, DateOnly validadeInicial, DateOnly validadeFinal, DateOnly? prorrogacao, int tipo,
                                                      string? origem, string? numeroSolicitacao, DateOnly? dataRegistroMTE, string? numeroRegistroMTE, DateOnly? dataAssinatura,
                                                      string? permissao, string? observacao, int? documentoLocalizacaoId, bool restrito,
                                                      IEnumerable<Estabelecimento>? estabelecimentos, IEnumerable<string>? referencias, IEnumerable<SindicatoLaboral>? sindicatoLaborais, IEnumerable<SindicatoPatronal>? sindicatoPatronais, IEnumerable<Cnae>? cnaes, IEnumerable<Abrangencia>? abrangencias,
                                                      TipoModulo modulo, string? uf, string? caminhoArquivo, DateOnly? dataAprovacao, int? tipoNegocioId, string? nomeArquivo, string dataBase, string? numeroLei, string? fonteLei)
        {
            var dataBaseFormatada = dataBase.ToUpperInvariant();

            var documentoSindical = new DocumentoSindical(versao, validadeInicial, validadeFinal, prorrogacao, tipo,
                                                          origem, numeroSolicitacao, dataRegistroMTE, numeroRegistroMTE, dataAssinatura,
                                                          permissao, observacao, documentoLocalizacaoId, dataBaseFormatada, restrito,
                                                          estabelecimentos, referencias, sindicatoLaborais, sindicatoPatronais, cnaes, abrangencias,
                                                          modulo, uf, caminhoArquivo, dataAprovacao, tipoNegocioId, nomeArquivo, numeroLei, fonteLei);

            return Result.Success(documentoSindical);
        }

        public Result Atualizar(string versao, DateOnly validadeInicial, DateOnly validadeFinal, DateOnly? prorrogacao, int tipo,
                                                      string? origem, string? numeroSolicitacao, DateOnly? dataRegistroMTE, string? numeroRegistroMTE, DateOnly? dataAssinatura,
                                                      string? permissao, string? observacao, int? documentoLocalizacaoId, bool restrito,
                                                      IEnumerable<Estabelecimento>? estabelecimentos, IEnumerable<string>? referencias, IEnumerable<SindicatoLaboral>? sindicatoLaborais, IEnumerable<SindicatoPatronal>? sindicatoPatronais, IEnumerable<Cnae>? cnaes, IEnumerable<Abrangencia>? abrangencias,
                                                      TipoModulo modulo, string? uf, string? caminhoArquivo, int? tipoNegocioId, string dataBase, DateOnly? dataSla = null)
        {
            Versao = versao;
            DataValidadeInicial = validadeInicial;
            DataValidadeFinal = validadeFinal;
            DataProrrogacao = prorrogacao;
            NumeroSolicitacao = numeroSolicitacao;
            DataAssinatura = dataAssinatura;
            NumeroRegistroMTE = numeroRegistroMTE;
            DataRegistroMTE = dataRegistroMTE;
            Origem = origem;
            Observacao = observacao;
            Permissao = permissao;
            TipoDocumentoId = tipo;
            Uf = uf;
            DataAprovacao = null;
            Database = dataBase;
            Restrito = restrito;
            DocumentoLocalizacaoId = documentoLocalizacaoId;
            CaminhoArquivo = caminhoArquivo;
            Modulo = modulo;
            TipoNegocioId = tipoNegocioId;
            DataSla = dataSla;
            DataAtualizacao = DateOnly.FromDateTime(DateTime.Today);
            _sindicatoPatronais = sindicatoPatronais?.ToList();
            _sindicatoLaborais = sindicatoLaborais?.ToList();
            _cnaes = cnaes?.ToList();
            _estabelecimentos = estabelecimentos?.ToList();
            _abrangencias = abrangencias?.ToList();
            _referencias = referencias?.ToList();

            return Result.Success();
        }

        internal Result Aprovar(DateOnly dataSla, int usuarioAprovador)
        {
            if (DataAprovacao is not null) return Result.Failure("Documento já foi aprovado.");
            if (usuarioAprovador <= 0) return Result.Failure("O id do usuário aprovador deve ser maior que 0");

            DataSla = dataSla;
            DataAprovacao = DateOnly.FromDateTime(DateTime.Today);
            DataAtualizacao = DateOnly.FromDateTime(DateTime.Today);
            UsuarioAprovadorId = usuarioAprovador;
            return Result.Success();
        }

        internal Result AtualizarDataSla(DateOnly dataSla)
        {
            if (dataSla < DateOnly.FromDateTime(DateTime.Today)) return Result.Failure("Você não pode criar uma data sla para o passado");

            DataSla = dataSla;
            DataAtualizacao = DateOnly.FromDateTime(DateTime.Today);

            return Result.Success();
        }

        internal Result Liberar()
        {
            DataLiberacao = DateOnly.FromDateTime(DateTime.Today);

            return Result.Success();
        }

        internal Result AtualizarEstabelecimentos(List<Estabelecimento>? estabelecimentos)
        {
            _estabelecimentos = estabelecimentos;
            return Result.Success();
        }
    }
}
