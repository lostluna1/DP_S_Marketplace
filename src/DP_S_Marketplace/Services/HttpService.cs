using System.Net.Http.Headers;
using System.Text;
using DP_S_Marketplace.Contracts.Services;
using DP_S_Marketplace.Core.Helpers;
using DP_S_Marketplace.Models;

namespace DP_S_Marketplace.Services;

public class HttpService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiBaseUrl;
    private string? _token;
    public HttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiBaseUrl = $"http://103.36.223.176:5244";
        _= Settoken();
    }
    public async Task Settoken()
    {
        var loginData = new
        {
            username = "Der",
            password = "123456789"
        };

        var a = await PostAsync<LoginInfo>("/api/auth/login", loginData);
        if (a.Data != null)
        {
            _token = a.Data.Token;
        }

    }
    public class LoginInfo
    {
        public string? Token
        {
            get; set;
        }
    }
    public void SetToken(string token)
    {
        _token = token;
    }
    private void AddAuthorizationHeader()
    {
        if (!string.IsNullOrEmpty(_token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( _token);
        }
    }
    public async Task<string> GetStringAsync(string uri, bool requiresToken = false)
    {
        if (requiresToken)
        {
            AddAuthorizationHeader();
        }
        var response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string uri, bool requiresToken = false)
    {
        if (requiresToken)
        {
            AddAuthorizationHeader();
        }
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}{uri}");
            var jsonResponse = await response.Content.ReadAsStringAsync();
            return await Json.ToObjectAsync<ApiResponse<T>>(jsonResponse);
        }
        catch (Exception e)
        {

            throw new Exception(e.Message);
        }
    }


    public async Task<ApiResponse<T>> PostAsync<T>(string uri, object data, bool requiresToken = false)
    {
        if (requiresToken)
        {
            AddAuthorizationHeader();
        }
        var jsonRequestBody = await Json.StringifyAsync(data);
        StringContent content = new(jsonRequestBody, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_apiBaseUrl}{uri}", content);
        var jsonResponse = await response.Content.ReadAsStringAsync();
        return await Json.ToObjectAsync<ApiResponse<T>>(jsonResponse);
    }
    public async Task<ApiResponse<T>> PostAsync<T>(string uri, HttpContent content, bool requiresToken = false)
    {
        if (requiresToken)
        {
            AddAuthorizationHeader();
        }

        var response = await _httpClient.PostAsync($"{_apiBaseUrl}{uri}", content);
        var jsonResponse = await response.Content.ReadAsStringAsync();
        return await Json.ToObjectAsync<ApiResponse<T>>(jsonResponse);
    }


    public async Task<ApiResponse<T>> PutAsync<T>(string uri, object data, bool requiresToken = false)
    {
        if (requiresToken)
        {
            AddAuthorizationHeader();
        }
        var jsonRequestBody = await Json.StringifyAsync(data);
        StringContent content = new(jsonRequestBody, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{_apiBaseUrl}{uri}", content);
        var jsonResponse = await response.Content.ReadAsStringAsync();
        return await Json.ToObjectAsync<ApiResponse<T>>(jsonResponse);
    }

    public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint, long id, bool requiresToken = false)
    {
        if (requiresToken)
        {
            AddAuthorizationHeader();
        }
        var uri = $"{_apiBaseUrl}{endpoint}?id={id}";
        var response = await _httpClient.DeleteAsync(uri);
        var jsonResponse = await response.Content.ReadAsStringAsync();
        return await Json.ToObjectAsync<ApiResponse<T>>(jsonResponse);
    }


}
