namespace SearchEngine.API.Models;

public record Image
{
    public string Url { get; set; }
    public string AltText { get; set; }
}