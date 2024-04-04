using System.Diagnostics;
using System.Text;
using System.Text.Json;

using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;

using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.BuildingBlocks.Core.Web.API.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ineditta.Integration.Auth
{
    public class KeycloakHttpClient
     {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<KeycloakHttpClient> _logger;

        public KeycloakHttpClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<KeycloakHttpClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger;
        }

        public async ValueTask<Result<IDictionary<string, IEnumerable<string>>?, Error>> PostAndReceiveHeadersAsync<TRequest>(string uri, TRequest request)
        where TRequest : class
        {
            if (request is null)
            {
                return Result.Failure<IDictionary<string, IEnumerable<string>>?, Error>(Errors.Http.InvalidPayload());
            }

            var payload = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var authHeader = _httpContextAccessor!.HttpContext!.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader) || !authHeader.Any())
            {
                return Result.Failure<IDictionary<string, IEnumerable<string>>?, Error>(Errors.Http.InvalidHeader("Authorization"));
            }

            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}{uri}");

            httpRequestMessage.Headers.Add("Authorization", authHeader.FirstOrDefault());

            if (content is not null)
            {
                httpRequestMessage.Content = content;
            }

            using var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return Result.Success<IDictionary<string, IEnumerable<string>>?, Error>(default);
                }

                var responseHeaders = httpResponseMessage.Headers.ToDictionary(x => x.Key, x => x.Value);

                return Result.Success<IDictionary<string, IEnumerable<string>>?, Error>(responseHeaders);
            }

            var responseErrorContent = await httpResponseMessage.Content.ReadAsStringAsync();

#pragma warning disable CA1848 // Use the LoggerMessage delegates
            _logger.LogError("Failed to request {ResponseErrorContent}", responseErrorContent);
#pragma warning restore CA1848 // Use the LoggerMessage delegates

            return Result.Failure<IDictionary<string, IEnumerable<string>>?, Error>(Errors.Http.BadRequest(responseErrorContent));
        }

        public async ValueTask<Result<TResponse?, Error>> PostAsync<TRequest, TResponse>(string uri, TRequest request)
             where TRequest : class
             where TResponse : class
        {
            if (request is null)
            {
                return Result.Failure<TResponse?, Error>(Errors.Http.InvalidPayload());
            }

            var payload = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            return await SendAsync<TResponse>(uri, HttpMethod.Post, content);
        }

        public async ValueTask<Result<bool, Error>> PostAsync<TRequest>(string uri, TRequest request)
            where TRequest : class
        {
            if (request is null)
            {
                return Result.Failure<bool, Error>(Errors.Http.InvalidPayload());
            }

            var payload = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            return await SendAsync(uri, HttpMethod.Post, content);
        }

        public async ValueTask<Result<TResponse?, Error>> PutAsync<TRequest, TResponse>(string uri, TRequest request)
            where TRequest : class
            where TResponse : class
        {
            if (request is null)
            {
                return Result.Failure<TResponse?, Error>(Errors.Http.InvalidPayload());
            }

            var payload = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            return await SendAsync<TResponse>(uri, HttpMethod.Put, content);
        }

        public async ValueTask<Result<bool, Error>> PutAsync(string uri)
        {
            return await SendAsync(uri, HttpMethod.Put, null);
        }

        public async ValueTask<Result<bool, Error>> PutAsync<TRequest>(string uri, TRequest request)
            where TRequest : class
        {
            if (request is null)
            {
                return Result.Failure<bool, Error>(Errors.Http.InvalidPayload());
            }

            var payload = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            return await SendAsync(uri, HttpMethod.Put, content);
        }

        public async ValueTask<Result<TResponse?, Error>> GetAsync<TRequest, TResponse>(string uri, TRequest? request = null)
            where TRequest : class
            where TResponse : class
        {
            var queryString = request is not null ? QueryStringExtension.ToQueryString(request, true) : string.Empty;

            uri = string.IsNullOrEmpty(queryString) ? uri : $"{uri}?{queryString}";

            return await SendAsync<TResponse>(uri, HttpMethod.Get);
        }

        private async ValueTask<Result<TResponse?, Error>> SendAsync<TResponse>(string uri, HttpMethod httpMethod, StringContent? content = default)
        {
            var authHeader = _httpContextAccessor!.HttpContext!.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader) || !authHeader.Any())
            {
                return Result.Failure<TResponse?, Error>(Errors.Http.InvalidHeader("Authorization"));
            }

            using var httpRequestMessage = new HttpRequestMessage(httpMethod, $"{_httpClient.BaseAddress}{uri}");

            httpRequestMessage.Headers.Add("Authorization", authHeader.FirstOrDefault());

            if (content is not null)
            {
                httpRequestMessage.Content = content;
            }

            using var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return Result.Success<TResponse?, Error>(default);
                }

                var responseContent = await JsonSerializer.DeserializeAsync<TResponse>(await httpResponseMessage.Content.ReadAsStreamAsync(), new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                return Result.Success<TResponse?, Error>(responseContent);
            }

            var responseErrorContent = await httpResponseMessage.Content.ReadAsStringAsync();

#pragma warning disable CA1848 // Use the LoggerMessage delegates
            _logger.LogError("Failed to request {ResponseErrorContent}", responseErrorContent);
#pragma warning restore CA1848 // Use the LoggerMessage delegates

            return Result.Failure<TResponse?, Error>(Errors.Http.BadRequest(responseErrorContent));
        }

        private async ValueTask<Result<bool, Error>> SendAsync(string uri, HttpMethod httpMethod, StringContent? content = default)
        {
            var authHeader = _httpContextAccessor!.HttpContext!.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader) || !authHeader.Any())
            {
                return Result.Failure<bool, Error>(Errors.Http.InvalidHeader("Authorization"));
            }

            using var httpRequestMessage = new HttpRequestMessage(httpMethod, $"{_httpClient.BaseAddress}{uri}");

            httpRequestMessage.Headers.Add("Authorization", authHeader.FirstOrDefault());

            if (content is not null)
            {
                httpRequestMessage.Content = content;
            }

            using var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return Result.Success<bool, Error>(default);
                }

                await httpResponseMessage.Content.ReadAsStringAsync();

                return Result.Success<bool, Error>(true);
            }

            var responseErrorContent = await httpResponseMessage.Content.ReadAsStringAsync();

#pragma warning disable CA1848 // Use the LoggerMessage delegates
            _logger.LogError("Failed to request {ResponseErrorContent}", responseErrorContent);
#pragma warning restore CA1848 // Use the LoggerMessage delegates

            return Result.Failure<bool, Error>(Errors.Http.BadRequest(responseErrorContent));
        }
    }
}
