using CSharpFunctionalExtensions;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.BuildingBlocks.Core.FileStorage;
using Ineditta.BuildingBlocks.Core.Files.Pdfs;
using Microsoft.Extensions.Options;
using Ineditta.Application.AIs.DocumentosSindicais.Dtos;
using Ineditta.Application.AIs.DocumentosSindicais.Factories;
using Ineditta.Application.AIs.DocumentosSindicais.Services.Mte;
using Ineditta.Application.SharedKernel.Ocr.Services;
using Ineditta.Application.Documentos.Localizados.Services;
using Ineditta.Application.Documentos.Sindicais.Repositories;

namespace Ineditta.Application.AIs.DocumentosSindicais.Services.QuebraClausula
{
    public class QuebraClausulaService : IQuebraClausulaService
    {

        private readonly IDocumentoSindicalRepository _documentoSindicalRepository;
        private readonly IMteService _mteService;
        private readonly IOcrService _ocrService;
        private readonly IDocumentoLocalizadoService _documentoLocalizadoService;

        public QuebraClausulaService(IDocumentoSindicalRepository documentoSindicalRepository,
                                     IMteService mteService,
                                     IOcrService ocrService,
                                     IDocumentoLocalizadoService documentoLocalizadoService,
                                     IOptions<FileStorageConfiguration> fileStorageConfiguration)
        {
            _documentoSindicalRepository = documentoSindicalRepository;
            _mteService = mteService;
            _ocrService = ocrService;
            _documentoLocalizadoService = documentoLocalizadoService;
        }

        public async ValueTask<Result<QuebraClausulaDto, Error>> QuebrarContratoEmClausulas(long documentoSindicalId, CancellationToken cancellationToken = default)
        {
            var documentoSindical = await _documentoSindicalRepository.ObterPorIdAsync(documentoSindicalId);
            if (documentoSindical is null)
            {
                return Result.Failure<QuebraClausulaDto, Error>(Errors.General.NotFound(documentoSindicalId));
            }

            QuebraClausulaDto? clausulasQuebradas = default;

            //MTE
            if (documentoSindical.Versao == "Registrado")
            {
                if (string.IsNullOrEmpty(documentoSindical.NumeroSolicitacao))
                {
                    return Result.Failure<QuebraClausulaDto, Error>(Errors.General.Business("Documento do MTE deve ter o número de registro informado."));
                }

                var textoContrato = await _mteService.ObterHtmlContrato(documentoSindical.NumeroSolicitacao, cancellationToken);

                if (textoContrato.IsFailure)
                {
                    return Result.Failure<QuebraClausulaDto, Error>(Errors.General.Business("Site MTE indisponível durante a leitura do documento. Assim que normalizar o MTE, clique em Reprocessar Scrap."));
                }

                var clausulasQuebradasMte = QuebraClausulaMteFactory.RealizarQuebra(textoContrato.Value);

                if (clausulasQuebradasMte.IsFailure)
                {
                    return Result.Failure<QuebraClausulaDto, Error>(clausulasQuebradasMte.Error);
                }

                clausulasQuebradas = clausulasQuebradasMte.Value;
            }
            else
            {
                if (documentoSindical.DocumentoLocalizacaoId is null)
                {
                    return Result.Failure<QuebraClausulaDto, Error>(Errors.General.Business("Documento sindical não possuí documento localizado ID informado."));
                }

                var arquivo = await _documentoLocalizadoService.ObterBytesPorDocumentoId((long)documentoSindical.DocumentoLocalizacaoId, cancellationToken);

                if (arquivo.IsFailure)
                {
                    return Result.Failure<QuebraClausulaDto, Error>(arquivo.Error);
                }

                var pdfTexto = PdfManager.IsPdfText(arquivo.Value, cancellationToken);

                if (pdfTexto.IsFailure)
                {
                    return Result.Failure<QuebraClausulaDto, Error>(pdfTexto.Error);
                }

                //PDF com Texto
                if (pdfTexto.Value)
                {
                    var pdf = PdfManager.ExtractTextFromPdf(arquivo.Value, cancellationToken);

                    if (pdf.IsFailure)
                    {
                        return Result.Failure<QuebraClausulaDto, Error>(pdf.Error);
                    }

                    var clausulasQuebradasTexto = QuebraClausulaTextoFactory.RealizarQuebraTexto(pdf.Value.ConcatenatedText);

                    if (clausulasQuebradasTexto.IsFailure)
                    {
                        return Result.Failure<QuebraClausulaDto, Error>(clausulasQuebradasTexto.Error);
                    }

                    clausulasQuebradas = clausulasQuebradasTexto.Value;
                }
                else //PDF com Imagens - OCR
                {
                    var texto = await _ocrService.ExtractTextAsync(arquivo.Value, cancellationToken);

                    if (texto.IsFailure)
                    {
                        return Result.Failure<QuebraClausulaDto, Error>(texto.Error);
                    }

                    var clausulasQuebradasTexto = QuebraClausulaTextoFactory.RealizarQuebraTexto(texto.Value.Text);

                    if (clausulasQuebradasTexto.IsFailure)
                    {
                        return Result.Failure<QuebraClausulaDto, Error>(clausulasQuebradasTexto.Error);
                    }

                    clausulasQuebradas = clausulasQuebradasTexto.Value;
                }
            }

            return clausulasQuebradas is null
                ? Result.Failure<QuebraClausulaDto, Error>(Errors.General.Business("Não foi possível realizar a quebra das cláusulas."))
                : Result.Success<QuebraClausulaDto, Error>(clausulasQuebradas);
        }
    }
}
