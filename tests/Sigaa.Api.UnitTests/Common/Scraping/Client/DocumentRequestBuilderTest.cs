using System.Net;
using Sigaa.Api.Common.Scraping.Client;
using Sigaa.Api.Common.Scraping.Document;

namespace Sigaa.Api.UnitTests.Common.Scraping.Client;

[TestSubject(typeof(DocumentRequestBuilder))]
public sealed class DocumentRequestBuilderTest
{
    private readonly HttpClient client;
    private readonly HttpMessageHandler messageHandler;
    private readonly IHtmlParser htmlParser;
    private readonly CancellationToken cancellationToken;

    public DocumentRequestBuilderTest()
    {
        messageHandler = A.Fake<HttpMessageHandler>();
        client = new HttpClient(messageHandler);
        htmlParser = A.Fake<IHtmlParser>();
        cancellationToken = CancellationToken.None;
    }

    private DocumentRequestBuilder CreateBuilder(HttpMethod? method = null, Uri? uri = null)
    {
        var request = new HttpRequestMessage(method ?? HttpMethod.Get, uri ?? new Uri("https://test.com/"));
        return new DocumentRequestBuilder(client, request, htmlParser, cancellationToken);
    }

    [Fact]
    public void WithPersistentSession_WhenCalled_ShouldReturnPersistentDocumentRequestBuilder()
    {
        // Arrange
        var builder = CreateBuilder();

        // Act
        var result = builder.WithPersistentSession();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PersistentDocumentRequestBuilder>();
    }

    [Fact]
    public void WithFormData_WhenCalled_ShouldSetMethodToPostAndSetContent()
    {
        // Arrange
        var builder = CreateBuilder();
        var formData = new Dictionary<string, string>
        {
            { "key", "value" }
        };

        // Act
        var result = builder.WithFormData(formData);

        result.Should().BeSameAs(builder);
    }

    [Fact]
    public async Task GetAwaiter_WhenRequestIsSuccessful_ShouldReturnParsedDocument()
    {
        // Arrange
        var uri = new Uri("https://test.com/");

        var builder = new DocumentRequestBuilder(uri, client, htmlParser, cancellationToken);
        const string htmlContent = "<html><body>Test</body></html>";

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(htmlContent),
            RequestMessage = new HttpRequestMessage(HttpMethod.Get, uri)
        };

        var expectedDocument = A.Fake<IDocument>();

        A.CallTo(messageHandler).Where(x => x.Method.Name == "SendAsync")
            .WithReturnType<Task<HttpResponseMessage>>()
            .Returns(response);

        A.CallTo(() => htmlParser.ParseAsync(htmlContent, cancellationToken)).Returns(expectedDocument);

        // Act
        var document = await builder;

        // Assert
        document.Should().BeSameAs(expectedDocument);
        document.Url.Should().Be(uri);
    }

    [Fact]
    public async Task AsTask_WhenRequestFails_ShouldThrowScrapingClientException()
    {
        // Arrange
        var uri = new Uri("https://test.com/");
        var builder = new DocumentRequestBuilder(uri, client, htmlParser, cancellationToken);

        A.CallTo(messageHandler).Where(x => x.Method.Name == "SendAsync")
            .WithReturnType<Task<HttpResponseMessage>>()
            .Throws(new HttpRequestException("Network error"));

        // Act
        var action = () => builder.AsTask();

        // Assert
        await action.Should().ThrowAsync<ScrapingClientException>()
            .WithMessage("A network error occurred while requesting 'https://test.com/'.");
    }
}