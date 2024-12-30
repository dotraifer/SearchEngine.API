using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SearchEngine.API.ConfigurationObjects;

public record ElasticConfiguration
{
    [Required, Url, Description("Elastic Uri")]
    public required Uri Uri { get; init; }
    
    [Required, Description("Elastic index")]
    public required string IndexName { get; init; }
    
    [Description("Elastic user"), Required]
    public string? User { get; init; }
    
    [Description("Elastic password"), Required]
    public string? Password { get; init; }
}