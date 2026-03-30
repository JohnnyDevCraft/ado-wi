using System.Net;
using ReverseMarkdown;

namespace WorkItems.Services;

public static class HtmlMarkdownConverter
{
    private static readonly Converter Converter = new(new Config
    {
        UnknownTags = Config.UnknownTagsOption.Drop
    });

    public static string ConvertDescriptionField(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var trimmed = value.Trim();
        if (!LooksLikeHtml(trimmed))
        {
            return WebUtility.HtmlDecode(trimmed);
        }

        var markdown = Converter.Convert(trimmed);
        return NormalizeMarkdown(markdown);
    }

    public static bool LooksLikeHtml(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return value.Contains('<') && value.Contains('>') &&
               (value.Contains("</", StringComparison.Ordinal) ||
                value.Contains("<p", StringComparison.OrdinalIgnoreCase) ||
                value.Contains("<div", StringComparison.OrdinalIgnoreCase) ||
                value.Contains("<br", StringComparison.OrdinalIgnoreCase) ||
                value.Contains("<ul", StringComparison.OrdinalIgnoreCase) ||
                value.Contains("<ol", StringComparison.OrdinalIgnoreCase) ||
                value.Contains("<table", StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeMarkdown(string markdown)
    {
        return markdown
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal)
            .Trim();
    }
}
