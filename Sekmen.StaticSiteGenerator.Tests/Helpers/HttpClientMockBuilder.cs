namespace Sekmen.StaticSiteGenerator.Tests.Helpers;

public class HttpClientMockBuilder
{
    private readonly Dictionary<string, HttpResponseMessage> _responses = [];
    private readonly Dictionary<string, (long ContentLength, bool ShouldSucceed)> _headResponses = [];
    
    public HttpClientMockBuilder WithGetResponse(string url, string content, string mediaType = "text/html")
    {
        _responses[url] = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(content, Encoding.UTF8, mediaType)
        };
        return this;
    }
    
    public HttpClientMockBuilder WithGetResponse(string url, byte[] content, string mediaType = "application/octet-stream")
    {
        _responses[url] = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(content)
            {
                Headers = { ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(mediaType) }
            }
        };
        return this;
    }
    
    public HttpClientMockBuilder WithNotFoundResponse(string url)
    {
        _responses[url] = new HttpResponseMessage(HttpStatusCode.NotFound);
        return this;
    }
    
    public HttpClientMockBuilder WithHeadResponse(string url, long contentLength, bool shouldSucceed = true)
    {
        _headResponses[url] = (contentLength, shouldSucceed);
        return this;
    }
    
    public HttpClient Build()
    {
        Mock<HttpMessageHandler> mock = new Mock<HttpMessageHandler>();
        
        mock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Get &&
                    _responses.ContainsKey(r.RequestUri!.ToString())),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken ct) =>
                _responses[request.RequestUri!.ToString()])
            .Verifiable();
        
        mock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Head &&
                    _headResponses.ContainsKey(r.RequestUri!.ToString())),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken ct) =>
            {
                string url = request.RequestUri!.ToString();
                (long contentLength, bool shouldSucceed) = _headResponses[url];
                
                if (!shouldSucceed)
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content.Headers.ContentLength = contentLength;
                return response;
            })
            .Verifiable();
        
        return new HttpClient(mock.Object);
    }
}
