using Markdig;
using Microsoft.AspNetCore.Components;

using Microsoft.FluentUI.AspNetCore.Components;
namespace PalmHill.BlazorChat.Client.Components.Markdown;


public partial class MarkdownSection : FluentComponentBase
{
    private string? _content;
    private bool _contentChanged;
    private static readonly MarkdownPipeline _markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

    [Parameter]
    public string? Content { get; set; }

    [Parameter]
    public EventCallback<string> ContentChanged { get; set; }

    [Parameter]
    public EventCallback OnContentConverted { get; set; }

    public MarkupString HtmlContent { get; private set; }

    protected override void OnParametersSet()
    {
        if (Content != _content)
        {
            _content = Content;
            UpdateHtmlContent();
            _contentChanged = true;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_contentChanged)
        {
            _contentChanged = false;
            await ContentChanged.InvokeAsync(_content);
            await OnContentConverted.InvokeAsync();
        }
    }

    public void SetContent(string content)
    {
        if (content != _content)
        {
            _content = content;
            UpdateHtmlContent();
            _contentChanged = true;
            StateHasChanged();
        }
    }

    private void UpdateHtmlContent()
    {
        HtmlContent = ConvertToMarkupString(_content);
    }

    private static MarkupString ConvertToMarkupString(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new MarkupString();
        }

        try
        {
            string html = Markdig.Markdown.ToHtml(value, _markdownPipeline);
            return new MarkupString(html);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error converting Markdown to HTML: {ex.Message}");
            return new MarkupString($"<p>Error rendering Markdown: {ex.Message}</p>");
        }
    }

}