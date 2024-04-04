using Amazon.Textract.Model;
using Amazon.Textract;
using CSharpFunctionalExtensions;
using Ineditta.Application.SharedKernel.Ocr.Dtos;
using Ineditta.Application.SharedKernel.Ocr.Services;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Microsoft.Extensions.Options;
using Ineditta.Integration.OCR.Configurations;
using Microsoft.FeatureManagement;
using Ineditta.Integration.Ocr.Providers.Aws.Factories;
using Ineditta.Application.SharedKernel.FeaturesFlags;
using Amazon;
using System.Text;
using Amazon.Runtime;
using System.Net;
using PDFtoImage;
using SkiaSharp;

namespace Ineditta.Integration.Ocr.Providers.Aws.Services
{
    public class OcrAwsService : IOcrService
    {
        private readonly OcrConfiguration _configuration;
        private readonly IFeatureManager _featureManager;
        private const int RetryCount = 10;

        public OcrAwsService(IOptions<OcrConfiguration> configuration, IFeatureManager featureManager)
        {
            _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
            _featureManager = featureManager;
        }

        public async ValueTask<Result<ExtractTextResponseDto, Error>> ExtractTextAsync(byte[] pdfFile, CancellationToken cancellationToken = default)
        {
            return await RetryPolicyFactory.ExecuteWithRetryFailureAsync(async () =>
            {
                if (!await _featureManager.IsEnabledAsync(nameof(FeatureFlag.UtilizaOcr)))
                {
                    return Result.Success<ExtractTextResponseDto, Error>(new ExtractTextResponseDto($"Feature flag de comunicação com o serviço de OCR está desabilitado."));
                }

                var imageBytes = ConvertPdfToImages(pdfFile);

                var responseStringBuilder = new StringBuilder();

                foreach (var imageByte in imageBytes)
                {
                    var textractResponse = await SendImageToTextractAsync(imageByte);

                    if (textractResponse.IsFailure)
                    {
                        return Result.Failure<ExtractTextResponseDto, Error>(textractResponse.Error);
                    }

                    responseStringBuilder.Append(textractResponse.Value);
                    responseStringBuilder.AppendLine(string.Empty);
                }

                return Result.Success<ExtractTextResponseDto, Error>(new ExtractTextResponseDto(responseStringBuilder.ToString()));

            }, RetryCount);
        }

        private static List<byte[]> ConvertPdfToImages(byte[] pdfFile)
        {
            var imageBytes = new List<byte[]>();

            var renderOptions = new RenderOptions
            {
                Dpi = 300
            };

            #pragma warning disable CA1416 // Validate platform compatibility
            var images = Conversion.ToImages(Convert.ToBase64String(pdfFile), options: renderOptions);
            #pragma warning restore CA1416 // Validate platform compatibility

            foreach (var image in images)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    SKImage.FromBitmap(image).Encode(SKEncodedImageFormat.Jpeg, 300).SaveTo(memoryStream);

                    imageBytes.Add(memoryStream.ToArray());

                }
            }

            return imageBytes;
        }

        private async ValueTask<Result<string, Error>> SendImageToTextractAsync(byte[] imageByte)
        {
            var request = new AnalyzeDocumentRequest
            {
                Document = new Document
                {
                    Bytes = new MemoryStream(imageByte),
                },
                FeatureTypes = new List<string> { "TABLES", "FORMS" } // Especifique os tipos de recursos que você deseja analisar
            };

            try
            {
                using (var textractClient = new AmazonTextractClient(_configuration.Aws.AccessKey, _configuration.Aws.SecretKey, RegionEndpoint.USEast1))
                {
                    var analyzeDocumenResponse = await textractClient.AnalyzeDocumentAsync(request);

                    var stringBuilder = new StringBuilder();

                    // Função recursiva para processar os blocos e seus child blocks
                    void ProcessBlocks(IEnumerable<Block> blocks)
                    {
                        foreach (var block in blocks)
                        {
                            if (block.BlockType == "LINE")
                            {
                                stringBuilder.AppendLine(block.Text);
                            }
                            else if (block.BlockType == "TABLE" && block.Relationships != null)
                            {
                                var cells = block.Relationships
                                    .Where(r => r.Type == "CHILD")
                                    .SelectMany(r => r.Ids)
                                    .Select(id => analyzeDocumenResponse.Blocks.Find(b => b.Id == id))
                                    .Where(c => c != null && c.Geometry != null);

                                // Formatando as linhas da tabela
                                foreach (var row in cells.GroupBy(c => c?.Geometry?.BoundingBox?.Top))
                                {
                                    var formattedRow = new StringBuilder();
                                    var sortedCells = row.OrderBy(c => c?.Geometry?.BoundingBox?.Left);

                                    if (sortedCells.Any())
                                    {
                                        foreach (var cell in sortedCells)
                                        {
                                            // Se o texto não estiver vazio, então adiciona ao formattedRow
                                            if (!string.IsNullOrEmpty(cell?.Text))
                                            {
                                                formattedRow.Append(cell?.Text);
                                                formattedRow.Append("||");
                                            }
                                            else if (cell?.Relationships?.Any() ?? false)
                                            {
                                                // Processa recursivamente os child blocks
                                                #pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                                                ProcessBlocks(cell.Relationships
                                                    .Where(r => r.Type == "CHILD")
                                                    .SelectMany(r => r.Ids)
                                                    .Select(id => analyzeDocumenResponse.Blocks.Find(b => b.Id == id))
                                                    .Where(c => c != null && c.Geometry != null));
                                                #pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                                            }
                                        }
                                    }

                                    // Adiciona a linha formatada ao stringBuilder, removendo o último separador "||"
                                    stringBuilder.AppendLine(formattedRow.ToString().TrimEnd('|'));
                                }
                            }
                        }
                    }

                    // Chama a função recursiva para processar os blocos
                    ProcessBlocks(analyzeDocumenResponse.Blocks);

                    return stringBuilder.ToString();
                }
            }
            catch (AmazonServiceException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                return Result.Failure<string, Error>(Errors.Http.TooManyRequests());
            }
            catch (AmazonServiceException ex) when ((int)ex.StatusCode >= 500 && (int)ex.StatusCode <= 599)
            {
                return Result.Failure<string, Error>(Errors.Http.ErrorCode());
            }
            catch (AmazonServiceException ex)
            {
                return Result.Failure<string, Error>(Errors.General.InternalServerError($"An error ocurred on AWS: {ex.StatusCode} - {ex.ErrorCode} - {ex.Message}"));
            }
            catch (Exception ex)
            {
                return Result.Failure<string, Error>(Errors.General.InternalServerError($"Não foi possível processar o documento: {ex.Message}"));
            }
        }
    }
}
