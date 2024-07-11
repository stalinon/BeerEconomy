using System.Net.Mime;
using System.Text;
using BeerEconomy.Common.Helpers;
using Newtonsoft.Json;

namespace BeerEconomy.Common.ApiClients.Impl;

/// <summary>
///     Базовый класс для всех API клиентов
/// </summary>
internal abstract class ApiClientBase
{
    #region Fields

    private readonly string _baseAddress;
    private readonly JsonSerializerSettings _serializerSettings;

    #endregion

    #region .ctor

    /// <summary>
    /// .ctor
    /// </summary>
    protected ApiClientBase(
        string baseAddress,
        JsonSerializerSettings? serializerSettings = null)
    {
        _baseAddress = baseAddress;
        _serializerSettings = serializerSettings ?? new JsonSerializerSettings();
    }

    #endregion

    #region Protected methods

    /// <inheritdoc cref="HttpClient.GetAsync(string?)"/>
    protected virtual async Task<T> GetAsync<T>(
        string url,
        QueryBase? query = null,
        CancellationToken cancellationToken = default)
    where T : class
    {
        var response = await GetAsync(url, query, cancellationToken: cancellationToken);
        return await TryGetValue<T>(response, cancellationToken);
    }

    /// <inheritdoc cref="HttpClient.GetAsync(string?)"/>
    protected virtual async Task<T> TryGetAsync<T>(
        string url,
        QueryBase? query = null,
        CancellationToken cancellationToken = default)
    where T : class
    {
        var response = await GetAsync(url, query, cancellationToken: cancellationToken);
        return await TryGetValue<T>(response, cancellationToken);
    }

    /// <inheritdoc cref="HttpClient.GetAsync(string?)"/>
    protected virtual async Task<T> GetAsync<T>(
        string url,
        QueryBase? query = null,
        Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await GetAsync(url, query, headers, cancellationToken);
        return await TryGetValue<T>(response, cancellationToken);
    }

    /// <inheritdoc cref="HttpClient.PostAsync(string?, HttpContent?)"/>
    protected virtual async Task<TResult> PostAsync<TResult>(
        string url,
        HttpContent content,
        CancellationToken cancellationToken = default)
        where TResult : class
    {
        var client = new HttpClient();
        var response = await client.PostAsync(GetFullUrl(url), content, cancellationToken);
        return await TryGetValue<TResult>(response, cancellationToken);
    }

    /// <inheritdoc cref="HttpClient.PostAsync(string?, HttpContent?)"/>
    protected virtual async Task<TResult> PostAsync<T, TResult>(
        string url,
        T? request = null,
        CancellationToken cancellationToken = default)
        where T : class
        where TResult : class
    {
        var client = new HttpClient();
        var response = await client.PostAsync(GetFullUrl(url), GetBody(request), cancellationToken);
        return await TryGetValue<TResult>(response, cancellationToken);
    }

    /// <inheritdoc cref="HttpClient.PutAsync(string?, HttpContent?)"/>
    protected virtual async Task<TResult> PutAsync<T, TResult>(
        string url,
        T? request = null,
        CancellationToken cancellationToken = default)
        where T : class
        where TResult : class
    {
        var client = new HttpClient();
        var response = await client.PutAsync(GetFullUrl(url), GetBody(request), cancellationToken);
        return await TryGetValue<TResult>(response, cancellationToken);
    }

    /// <inheritdoc cref="HttpClient.PutAsync(string?, HttpContent?)"/>
    protected virtual async Task<HttpResponseMessage> PutAsync<T>(
        string url,
        T? request = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        var client = new HttpClient();
        var response = await client.PutAsync(GetFullUrl(url), GetBody(request), cancellationToken);
        return response;
    }

    /// <inheritdoc cref="HttpClient.DeleteAsync(string?)"/>
    protected virtual async Task<HttpResponseMessage> DeleteAsync(
        string url,
        QueryBase? query = null,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(GetFullUrlWithQuery(url, query))
        };

        var client = new HttpClient();
        var response = await client.SendAsync(httpRequest, cancellationToken);
        return response;
    }

    #endregion

    #region Private methods

    /// <inheritdoc cref="HttpClient.GetAsync(string?)"/>
    private async Task<HttpResponseMessage> GetAsync(
        string url,
        QueryBase? query = null,
        Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(GetFullUrlWithQuery(url, query)),
        };

        if (headers != null)
        {
            foreach (var (name, header) in headers)
            {
                httpRequest.Headers.Add(name, header);
            }
        }
        
        var client = new HttpClient();
        var response = await client.SendAsync(httpRequest, cancellationToken);
        return response;
    }

    /// <summary>
    ///     Получить полный URL
    /// </summary>
    private string GetFullUrl(string url) => $"{_baseAddress}/{url}";

    /// <summary>
    ///     Получить полный URL вместе с сформированным запросом
    /// </summary>
    private string GetFullUrlWithQuery(string url, QueryBase? query)
    {
        url += $"?{query}";

        return GetFullUrl(url);
    }

    /// <summary>
    ///     Получить тело запроса
    /// </summary>
    private HttpContent? GetBody<T>(T? request) where T : class
    {
        HttpContent? httpContent = null;
        if (request != null)
        {
            switch (request)
            {
                case IEnumerable<KeyValuePair<string, string>> form:
                    httpContent = new FormUrlEncodedContent(form);
                    break;
                default:
                    var jsonRequest = JsonConvert.SerializeObject(request, _serializerSettings);
                    httpContent = new StringContent(jsonRequest, Encoding.UTF8, MediaTypeNames.Application.Json);
                    break;
            }
        }

        return httpContent;
    }
    
    private async Task<T> TryGetValue<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
        where T : class
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }
        
        return JsonConvert.DeserializeObject<T>(content, _serializerSettings);
    }

    #endregion
}