using DP_S_Marketplace.Models;

namespace DP_S_Marketplace.Contracts.Services;

public interface IApiService
{
    Task<ApiResponse<T>> GetAsync<T>(string uri, bool requiresToken = false);
    Task<ApiResponse<T>> PostAsync<T>(string uri, object data, bool requiresToken = false);
    Task<ApiResponse<T>> PostAsync<T>(string uri, HttpContent content, bool requiresToken = false);
    Task<ApiResponse<T>> PutAsync<T>(string uri, object data, bool requiresToken = false);
    Task<ApiResponse<T>> DeleteAsync<T>(string endpoint, long id, bool requiresToken = false);
    void SetToken(string token);
    Task<string> GetStringAsync(string uri, bool requiresToken = false);
    Task InitializeAsync();
}
