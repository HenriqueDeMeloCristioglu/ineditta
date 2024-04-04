using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.Integration.Email.Dtos;

namespace Ineditta.Integration.Email.Protocols
{
    public interface IEmailSender
    {
        ValueTask<Result<SendEmailResponseDto, Error>> SendAsync(SendEmailRequestDto request, CancellationToken cancellationToken = default);
    }
}
