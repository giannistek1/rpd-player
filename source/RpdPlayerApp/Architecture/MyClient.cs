using System.Net;

namespace RpdPlayerApp.Architecture;

internal class MyClient
{
    private readonly HttpClient _httpClient;
    public bool HeadOnly { get; set; }

    public MyClient(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
    }

    public async Task<HttpResponseMessage> SendAsync(
        Uri uri,
        CancellationToken cancellationToken = default)
    {
        var method = HeadOnly ? HttpMethod.Head : HttpMethod.Get;

        using var request = new HttpRequestMessage(method, uri);
        return await _httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);
    }
}
