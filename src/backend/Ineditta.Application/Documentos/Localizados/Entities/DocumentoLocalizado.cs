using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Org.BouncyCastle.Asn1.Ocsp;

namespace Ineditta.Application.DocumentosLocalizados.Entities
{
    public sealed class DocumentoLocalizado : Entity
    {
        public string? NomeDocumento { get; private set; }
        public string? Origem { get; private set; }
        public string CaminhoArquivo { get; private set; } = null!;
        public Situacao Situacao { get; private set; }
        public DateOnly? DataAprovacao { get; private set; }
        public DateTime DataRegistro { get; set; }
        public bool Referenciado { get; private set; }
        public long? IdLegado { get; private set; }
        public string? Uf { get; private set; }

        private DocumentoLocalizado(string? nomeDocumento, string? origem, string caminhoArquivo, Situacao situacao, DateOnly? dataAprovacao, bool referenciado, long? idLegado, string? uf)
        {
            NomeDocumento = nomeDocumento;
            Origem = origem ?? "Não informada";
            CaminhoArquivo = caminhoArquivo;
            Situacao = situacao;
            DataAprovacao = dataAprovacao;
            Referenciado = referenciado;
            IdLegado = idLegado;
            Uf = uf;
        }

        private DocumentoLocalizado() { }

        public static Result<DocumentoLocalizado> Criar(string? nomeDocumento, string? origem, string caminhoArquivo, Situacao situacao, DateOnly? dataAprovacao, long? idLegado, string? uf)
        {
            if (string.IsNullOrEmpty(caminhoArquivo)) return Result.Failure<DocumentoLocalizado>("Você deve fornecer um caminho para o arquivo");
            if (idLegado is not null && idLegado <= 0) return Result.Failure<DocumentoLocalizado>("O idLegado não pode ser menor ou igual a 0");
            if (situacao == Situacao.Aprovado && dataAprovacao is null) return Result.Failure<DocumentoLocalizado>("Se o documento é aprovado, então deve ter uma data de aprovação");
            if (dataAprovacao is not null && situacao != Situacao.Aprovado) return Result.Failure<DocumentoLocalizado>("Se o documento possui data de aprovação, então sua situação deve ser 'aprovado'");
            if (string.IsNullOrEmpty(nomeDocumento)) return Result.Failure<DocumentoLocalizado>("Nome do arquivo está nulo ou vazio.");

            var documentoLocalizado = new DocumentoLocalizado
            (
                nomeDocumento,
                origem,
                caminhoArquivo,
                situacao,
                dataAprovacao,
                false,
                idLegado,
                uf
            );

            return Result.Success<DocumentoLocalizado>(documentoLocalizado);
        }

        public Result Atualizar(string? nomeDocumento, string? origem, string caminhoArquivo, Situacao situacao, DateOnly? dataAprovacao, bool referenciado, long? idLegado, string? uf)
        {
            if (string.IsNullOrEmpty(caminhoArquivo)) return Result.Failure<DocumentoLocalizado>("Você deve fornecer um caminho para o arquivo");
            if (idLegado is not null && idLegado <= 0) return Result.Failure<DocumentoLocalizado>("O idLegado não pode ser menor ou igual a 0");
            if (situacao == Situacao.Aprovado && dataAprovacao is null) return Result.Failure<DocumentoLocalizado>("Se o documento é aprovado, então deve ter uma data de aprovação");
            if (dataAprovacao is not null && situacao != Situacao.Aprovado) return Result.Failure<DocumentoLocalizado>("Se o documento possui data de aprovação, então sua situação deve ser 'aprovado'");
            if (string.IsNullOrEmpty(nomeDocumento)) return Result.Failure<DocumentoLocalizado>("Nome do arquivo está nulo ou vazio.");

            NomeDocumento = nomeDocumento;
            Origem = origem;
            CaminhoArquivo = caminhoArquivo;
            Situacao = situacao;
            DataAprovacao = dataAprovacao;
            Referenciado = referenciado;
            IdLegado = idLegado;
            Uf = uf;

            return Result.Success();
        }

        public Result Aprovar()
        {
            Situacao = Situacao.Aprovado;
            DataAprovacao = DateOnly.FromDateTime(DateTime.Today);
            return Result.Success();
        }
    }
}
