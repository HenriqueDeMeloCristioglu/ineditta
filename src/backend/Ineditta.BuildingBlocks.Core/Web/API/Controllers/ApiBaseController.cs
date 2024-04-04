using System.Net;

using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.BuildingBlocks.Core.FileStorage;
using Ineditta.BuildingBlocks.Core.Idempotency.Web;
using Ineditta.BuildingBlocks.Core.Web.API.Actions;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Ineditta.BuildingBlocks.Core.Web.API.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

using Org.BouncyCastle.Asn1.Ocsp;

namespace Ineditta.BuildingBlocks.Core.Web.API.Controllers
{
    [ApiController]
    public abstract class ApiBaseController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly RequestStateValidator _requestStateValidator;

        protected ApiBaseController(IMediator mediator, RequestStateValidator requestStateValidator)
        {
            _mediator = mediator;
            _requestStateValidator = requestStateValidator;
        }

        protected static IActionResult OkFromEnvolpe(object? result)
        {
            return new EnvelopeResult(Envelope.Ok(result), result is null ? HttpStatusCode.NoContent : HttpStatusCode.OK);
        }

        protected static IActionResult NotFound(Error error)
        {
            return new EnvelopeResult(Envelope.Error(error), HttpStatusCode.NotFound);
        }

        protected static IActionResult Error(Error error)
        {
            return new EnvelopeResult(Envelope.Error(error), HttpStatusCode.BadRequest);
        }

        protected static IActionResult FromResult<T>(IResult<T, Error> result)
        {
            if (result.IsSuccess)
            {
                return OkFromEnvolpe(result.Value);
            }

            return result.Error == Errors.Http.NotFound() ? NotFound(result.Error) : Error(result.Error);
        }

        protected IActionResult FromResult(Result result)
        {
            return result.IsFailure ? Error(Errors.General.Business(result.Error)) : NoContent();
        }

        protected static IActionResult FromResult<T>(Result<T> result)
        {
            return result.IsFailure ? Error(Errors.General.Business(result.Error)) : OkFromEnvolpe(result.Value);
        }

        protected async ValueTask<IActionResult> Dispatch(IRequest<Result> request)
        {
            var result = await _mediator.Send(request);

            return _requestStateValidator.IsValid()
                ? FromResult(result)
                : new EnvelopeResult(Envelope.Error(_requestStateValidator.Errors.Select(error => EnvolopeError.Create(error, error.Code))), HttpStatusCode.BadRequest);
        }

        protected async ValueTask<IActionResult> Dispatch<T>(IRequest<Result<T>> request)
        {
            var result = await _mediator.Send(request);

            return _requestStateValidator.IsValid()
                ? FromResult(result)
                : new EnvelopeResult(Envelope.Error(_requestStateValidator.Errors.Select(error => EnvolopeError.Create(error, error.Code))), HttpStatusCode.BadRequest);
        }

        protected async ValueTask<IActionResult> Dispatch(IdempotentRequest request)
        {
            var result = await _mediator.Send(request);

            if (_requestStateValidator.IsValid() && result.IsSuccess)
            {
                return FromResult(result);
            }

            if (!_requestStateValidator.IsValid())
            {
                return new EnvelopeResult(Envelope.Error(_requestStateValidator.Errors.Select(error => EnvolopeError.Create(error, error.Code))), HttpStatusCode.BadRequest);
            }

            if (result.Error.Code == Errors.Http.Duplicated().Code)
            {
                return new EnvelopeResult(Envelope.Error(result.Error), HttpStatusCode.Conflict);
            }

            return new EnvelopeResult(Envelope.Error(result.Error), HttpStatusCode.BadRequest);
        }

        protected static IActionResult NotAcceptable()
        {
            return new EnvelopeResult(Envelope.Error(Errors.Http.NotAcceptable()), HttpStatusCode.NotAcceptable);
        }

        protected static IActionResult EmptyRequestBody()
        {
            return new EnvelopeResult(Envelope.Error(Errors.Http.EmptyRequestBody()), HttpStatusCode.BadRequest);
        }

        protected static IActionResult CreatedResult<TId>(TId id)
        {
            return new EnvelopeResult(Envelope.Ok(id), HttpStatusCode.Created);
        }

        protected static IActionResult BadRequestFromErrorMessage(string errorMessage)
        {
            return new EnvelopeResult(Envelope.Error(Errors.Http.BadRequest(errorMessage)), HttpStatusCode.BadRequest);
        }

        protected IActionResult DownloadExcelAsync(string fileName, byte[] bytes)
        {
            if (bytes.Length <= 0)
            {
                return NotFound(Errors.Http.NotFound());
            }

            return base.File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}.xlsx");
        }

        protected IActionResult DownloadFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(Errors.Http.NotFound());
            }

            return base.File(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read), GetContentType(filePath), Path.GetFileName(filePath));
        }
        protected IActionResult DownloadFile(string filePath, string filename)
        {
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(Errors.Http.NotFound());
            }

            return base.File(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read), GetContentType(filePath), filename);
        }
        protected FileStreamResult? DownloadFileStreamResult(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }

            return base.File(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read), GetContentType(filePath), Path.GetFileName(filePath));
        }

        protected async ValueTask<IActionResult> DownloadFileFromUri(Uri uri, Func<Uri, ValueTask<byte[]?>> action)
        {
            var file = await action(uri);

            return file is null ? NotFound(Errors.Http.NotFound()) : File(file, GetContentType(uri.LocalPath), Path.GetFileName(uri.LocalPath));
        }
        protected async ValueTask<IActionResult> DownloadFileFromUri(Uri uri, string filename, Func<Uri, ValueTask<byte[]?>> action)
        {
            var file = await action(uri);

            return file is null ? NotFound(Errors.Http.NotFound()) : File(file, GetContentType(uri.LocalPath), filename);
        }

        protected async ValueTask<FileContentResult?> DownloadFileContentResultFromUri(Uri uri, Func<Uri, ValueTask<byte[]?>> action)
        {
            var file = await action(uri);

            if (file is null)
            {
                return null;
            }

            return File(file, GetContentType(uri.LocalPath), Path.GetFileName(uri.LocalPath));
        }

        private static string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(path, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }

        protected IActionResult DownloadExcel(string fileNameWithoutExtension, byte[] bytes)
        {
            return File(bytes, MediaTypes.Excel, $"{fileNameWithoutExtension}.xlsx");
        }

        protected async ValueTask<IEnumerable<FileDto>> GetFiles(CancellationToken cancellationToken = default)
        {
            if ((Request.Form?.Files is null) || !Request.Form.Files.Any())
            {
                return Enumerable.Empty<FileDto>();
            }

            var files = new List<FileDto>();

            foreach (var formFile in Request.Form.Files)
            {
                using var ms = new MemoryStream();

                await formFile.CopyToAsync(ms, cancellationToken);

                var file = new FileDto(formFile.FileName, ms.ToArray(), string.Empty);

                files.Add(file);
            }

            return files;
        }
    }
}
