using SearchEngine.API.ConfigurationObjects;
using ILogger = Serilog.ILogger;

namespace SearchEngine.API;

public interface IContext
{
    Configuration Configuration { get; set; }
    
    ILogger Logger { get; set; }
    
    
}