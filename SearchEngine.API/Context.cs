using SearchEngine.API.ConfigurationObjects;
using Serilog;
using ILogger = Serilog.ILogger;

namespace SearchEngine.API;


/// <summary>
/// Represents the context for the search engine API.
/// </summary>
public class Context : IContext
{
    public Context(string yamlFilePath)
    {
        Configuration = GetConfiguration(yamlFilePath);
        Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/webcrawler-.logs",
                rollingInterval: RollingInterval.Day)
            .MinimumLevel.Is(Configuration.LogLevel)
            .CreateLogger();
    }

    public Configuration Configuration { set; get; }
    
    public ILogger Logger { get; set; }
    
    /// <summary>
    /// Reads the YAML configuration file and parses it into a <see cref="Configuration"/> object.
    /// </summary>
    /// <param name="yamlFilePath">The file path to the YAML configuration file.</param>
    /// <returns>A <see cref="Configuration"/> object parsed from the YAML file.</returns>
    private static Configuration GetConfiguration(string yamlFilePath)
    {
        var yaml = File.ReadAllText(yamlFilePath);
        return YamlParser.Parse<Configuration>(yaml);
    }
}
