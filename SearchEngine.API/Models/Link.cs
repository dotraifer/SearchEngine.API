namespace SearchEngine.API.Models;

public record Link
{
    public string Url { get; set; }
    public string Text { get; set; } // Anchor text
}