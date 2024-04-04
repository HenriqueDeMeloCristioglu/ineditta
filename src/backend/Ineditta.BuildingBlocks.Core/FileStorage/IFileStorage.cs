using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Models;

namespace Ineditta.BuildingBlocks.Core.FileStorage
{
    public interface IFileStorage
    {
        ValueTask<Result<FileInfoDto, string>> AddAsync(FileDto file, CancellationToken cancellationToken = default);
        ValueTask<Result<FileDto, Error>> GetAsync(string filePath, CancellationToken cancellationToken = default);
        ValueTask<Result> DeleteAsync(string filePath, CancellationToken cancellationToken = default);
    }
}
