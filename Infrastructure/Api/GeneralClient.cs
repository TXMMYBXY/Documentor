using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Documentor.Application.Api;
using Documentor.Application.Api.Models;

namespace Documentor.Infrastructure.Api;
public class GeneralClient : IGeneralClient
{
    private readonly HttpClient _httpClient;

    public GeneralClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
    }

    public async Task<TResponse?> PatchResponseAsync<TRequest, TResponse>(TRequest request, string uri)
    {
        var requestJson = JsonSerializer.Serialize(request);
        var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
        var response = await _httpClient.PatchAsync(uri, requestContent);
        
        await _IsSuccessStatusCode(response);

        var responseJson = await response.Content.ReadAsStringAsync();

        return _ConvertResponse<TResponse>(responseJson);
    }
    
    public async Task<TResponse?> PutResponseAsync<TRequest, TResponse>(TRequest request, string uri)
    {
        var requestJson = JsonSerializer.Serialize(request);
        var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(uri, requestContent);

        await _IsSuccessStatusCode(response);

        var responseJson = await response.Content.ReadAsStringAsync();

        return _ConvertResponse<TResponse>(responseJson);
    }

    public async Task<TResponse?> PostResponseAsync<TRequest, TResponse>(TRequest request, string uri)
    {
        var requestJson = JsonSerializer.Serialize(request);
        var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(uri, requestContent);
        
        await _IsSuccessStatusCode(response);

        var responseJson = await response.Content.ReadAsStringAsync();

        return _ConvertResponse<TResponse>(responseJson);
    }

    public async Task<TResponse?> MultipleDeletionResponseAsync<TRequest, TResponse>(TRequest request, string uri)
    {
        var requestJson = JsonSerializer.Serialize(request);
        var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
        var requestDelete = new HttpRequestMessage()
        {
            Method = HttpMethod.Delete,
            Content = requestContent,
            RequestUri = new Uri(uri)
        };
        requestDelete.Headers.Add("accept", "text/plain");
        var response = await _httpClient.SendAsync(requestDelete);
        
        await _IsSuccessStatusCode(response);

        var responseJson = await response.Content.ReadAsStringAsync();

        return _ConvertResponse<TResponse>(responseJson);
    }

    public async Task<TResponse?> DeleteResponseAsync<TResponse>(string uri)
    {
        var response = await _httpClient.DeleteAsync(uri);
        
        await _IsSuccessStatusCode(response);
        
        var responseJson = await response.Content.ReadAsStringAsync();
        
        return _ConvertResponse<TResponse>(responseJson);
    }

    public async Task<TResponse?> GetResponseAsync<TResponse>(string uri)
    {
        var response = await _httpClient.GetAsync(uri);

        await _IsSuccessStatusCode(response);
        
        var responseJson = await response.Content.ReadAsStringAsync();

        return _ConvertResponse<TResponse>(responseJson);
    }

    private static async Task _IsSuccessStatusCode(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var result = new ErrorResponse();
            try
            {
                result =
                    JsonSerializer.Deserialize<ErrorResponse>(errorContent);

            }
            catch (JsonException ex)
            {
                throw new HttpRequestException(
                    $"Request failed with status {response.StatusCode}",
                    null,
                    response.StatusCode
                );
            }
            
            throw new HttpRequestException(
                $"Request failed with status {response.StatusCode}, message: {result.Message}",
                null,
                response.StatusCode
            );
        }
    }

    private static T? _ConvertResponse<T>(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
            return default;

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<T>(response, options);
    }
}

