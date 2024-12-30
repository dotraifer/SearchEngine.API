using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Serilog.Events;

namespace SearchEngine.API.ConfigurationObjects;

public record Configuration
{
    [Required, Description("Elastic configuration")]
    public required ElasticConfiguration Elastic { get; init; }
    
    [AllowedValues("Debug", "Information", "Warning", "Error", "Fatal")]
    public LogEventLevel LogLevel { get; init; } = LogEventLevel.Debug;
}