using SearchEngine.API.ConfigurationObjects;
using ILogger = Serilog.ILogger;

namespace SearchEngine.API;

/// <summary>
/// Represents the context interface for the search engine API.
/// </summary>
public interface IContext
{
    Configuration Configuration { get; set; }

    ILogger Logger { get; set; }
}