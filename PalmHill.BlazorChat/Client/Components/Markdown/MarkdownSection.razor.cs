using Markdig;
using Microsoft.AspNetCore.Components;

using Microsoft.FluentUI.AspNetCore.Components;
namespace PalmHill.BlazorChat.Client.Components.Markdown;


public partial class MarkdownSection : FluentComponentBase
{
    private string? _content;
    private bool _raiseContentConverted;

    /// <summary>
    /// Gets or sets the Markdown content 
    /// </summary>
    [Parameter]
    public string? Content { get; set; }

    /// <summary>
    /// Event callback for when the Markdown content is converted to HTML.
    /// </summary>
    [Parameter]
    public EventCallback OnContentConverted { get; set; }

    public string? InternalContent
    {
        get => _content;
        set
        {
            _content = value;
            HtmlContent = ConvertToMarkupString(_content);


            if (OnContentConverted.HasDelegate)
            {
                OnContentConverted.InvokeAsync();
            }
            _raiseContentConverted = true;
            StateHasChanged();
        }
    }

    public MarkupString HtmlContent { get; private set; }


    protected override void OnInitialized()
    {
        if (Content is null)
            throw new ArgumentException("You need to provide either Content or FromAsset parameter");

        InternalContent = Content;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_raiseContentConverted)
        {
            _raiseContentConverted = false;
            if (OnContentConverted.HasDelegate)
            {
                await OnContentConverted.InvokeAsync();
            }

        }
    }

    public void RefreshContent()
    {
        InternalContent = Content;
    }


    private static MarkupString ConvertToMarkupString(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            // Convert markdown string to HTML
            string? html = Markdig.Markdown.ToHtml(value, new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());

            // Return sanitized HTML as a MarkupString that Blazor can render
            return new MarkupString(html);
        }

        return new MarkupString();
    }
}