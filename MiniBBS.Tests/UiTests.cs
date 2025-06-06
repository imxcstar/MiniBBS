using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace MiniBBS.Tests;

public class UiTests
{
    private readonly HttpClient _client;
    private readonly IBrowsingContext _context;

    public UiTests()
    {
        var factory = new TestWebApplicationFactory();
        _client = factory.CreateClient();
        _context = BrowsingContext.New(Configuration.Default);
    }

    [Fact]
    public async Task HomePage_NavigateToPostDetails()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        var document = await _context.OpenAsync(req => req.Content(html));
        var link = document.QuerySelector("tbody tr td a") as IHtmlAnchorElement;
        Assert.NotNull(link);

        var detail = await _client.GetAsync(link!.Href);
        detail.EnsureSuccessStatusCode();
        var detailHtml = await detail.Content.ReadAsStringAsync();
        var detailDoc = await _context.OpenAsync(req => req.Content(detailHtml));
        Assert.Contains(link.Text, detailDoc.QuerySelectorAll("h1").Last().TextContent);
    }
}
