using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.Integration.Email.Configurations;
using Ineditta.Integration.Email.Dtos;

using Microsoft.Extensions.Options;

using Ineditta.Integration.Email.Protocols;
using Amazon;
using Amazon.SimpleEmail.Model;
using Amazon.SimpleEmail;
using Microsoft.Extensions.Logging;

namespace Ineditta.Integration.Email.Providers.Aws.Services
{
    public class AwsEmailSender : IEmailSender
    {
        private readonly EmailConfiguration _awsVariables;
        private readonly ILogger<AwsEmailSender> _logger;
        private readonly RegistarEmailCaixaDeSaidaService _registarEmailCaixaDeSaidaService;

        public AwsEmailSender(IOptions<EmailConfiguration> awsVariables, ILogger<AwsEmailSender> logger, RegistarEmailCaixaDeSaidaService awsRegistarEmailCaixaDeSaidaService)
        {
            _awsVariables = awsVariables?.Value ?? throw new ArgumentNullException(nameof(awsVariables));
            _logger = logger;
            _registarEmailCaixaDeSaidaService = awsRegistarEmailCaixaDeSaidaService;
        }

        public async ValueTask<Result<SendEmailResponseDto, Error>> SendAsync(SendEmailRequestDto request, CancellationToken cancellationToken = default)
        {
            var emailReques = new SendEmailRequest
            {
                Source = $"Sistema Ineditta <{_awsVariables.Aws.Source}>",
                Message = new Message
                {
                    Body = new Body
                    {
                        Html = new Content
                        {
                            Charset = "UTF-8",
                            Data = request.Body
                        }
                    },
                    Subject = new Content
                    {
                        Charset = "UTF-8",
                        Data = request.Subject
                    }
                },
                Destination = new Destination
                {
                    ToAddresses = request.To?.ToList(),
                    CcAddresses = request.Cc?.ToList()
                }
            };

            var client = new AmazonSimpleEmailServiceClient(_awsVariables.Aws.AccessKey, _awsVariables.Aws.SecretKey, RegionEndpoint.SAEast1);
            string messageId;
            try
            {
                var response = await client.SendEmailAsync(emailReques, cancellationToken);

                messageId = response.MessageId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar e-mail");

                return Result.Failure<SendEmailResponseDto, Error>(Error.Create("aws.exception.generic", "Não foi possível enviar o email"));
            }

            await _registarEmailCaixaDeSaidaService.Handle(request, messageId, cancellationToken);

            var emailResponse = new SendEmailResponseDto
            {
                MessageId = messageId
            };

            return Result.Success<SendEmailResponseDto, Error>(emailResponse);
        }
    }
}


