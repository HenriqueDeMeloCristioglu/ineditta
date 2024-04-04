using CSharpFunctionalExtensions;
using Ineditta.Application.SharedKernel.Ocr.Dtos;
using Ineditta.BuildingBlocks.Core.Domain.Models;

namespace Ineditta.Application.SharedKernel.Ocr.Services
{
    public interface IOcrService
    {
        ValueTask<Result<ExtractTextResponseDto, Error>> ExtractTextAsync(byte[] pdfFile, CancellationToken cancellationToken = default);
    }
}
