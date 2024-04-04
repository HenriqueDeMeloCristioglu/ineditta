using CSharpFunctionalExtensions;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ineditta.BuildingBlocks.Core.FileStorage.Adapters.Physical
{
    public class PhysicalFileStorage : IFileStorage
    {
        private readonly FileStorageConfiguration _configuration;
        private readonly ILogger<IFileStorage> _logger;

        public PhysicalFileStorage(IOptions<FileStorageConfiguration> configuration, ILogger<IFileStorage> logger)
        {
            _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
        }

        public async ValueTask<Result<FileInfoDto, string>> AddAsync(FileDto file, CancellationToken cancellationToken = default)
        {
            try
            {
                var basePath = Path.Combine(_configuration.Path, file.Folder);

                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }

                var filePath = Path.Combine(basePath, file.Name);

                await File.WriteAllBytesAsync(filePath, file.Content, cancellationToken);

                return Result.Success<FileInfoDto, string>(new FileInfoDto(filePath, file.Name));
            }
            catch (DirectoryNotFoundException dnfex)
            {
                _logger.LogError(dnfex, "Failed to add file - Directory not found");
                return Result.Failure<FileInfoDto, string>("Não foi possível realizar o upload do arquivo");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add file");
                return Result.Failure<FileInfoDto, string>("Não foi possível realizar o upload do arquivo");
            }
        }

        public async ValueTask<Result> DeleteAsync(string filePath, CancellationToken cancellationToken = default)
        {
            try
            {
                File.Delete(filePath);
                await Task.CompletedTask;
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file");
                return Result.Failure("Erro ao tentar deletar o arquivo.");
            }
        }

        public async ValueTask<Result<FileDto, Error>> GetAsync(string filePath, CancellationToken cancellationToken = default)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    byte[] fileContent = await File.ReadAllBytesAsync(filePath, cancellationToken);
                    FileInfo fileInfo = new FileInfo(filePath);

                    string folder = fileInfo.Directory?.Name ?? string.Empty;
                    string fileName = fileInfo.Name;

                    FileDto fileDto = new FileDto(fileName, fileContent, folder);

                    return Result.Success<FileDto, Error>(fileDto);
                }
                else
                {
                    return Result.Failure<FileDto, Error>(Errors.General.Business("Arquivo não encontrado"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get file");
                return Result.Failure<FileDto, Error>(Errors.General.Business("Erro ao obter o arquivo"));
            }
        } 
    }
}
