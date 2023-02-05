using System.Net.Http.Headers;
using BackendDevTest.BusinessLogic;
using BackendDevTest.Helper;
using BackendDevTest.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

class Program
{
    static async Task Main(string[] args)
    {
        // Register services
        var services = new ServiceCollection();
        ConfigureServices(services);

        // Call the action to process index blocks
        await services
        .AddSingleton<ProcessToIndexBlockService, ProcessToIndexBlockService>()
            .BuildServiceProvider()
            .GetService<ProcessToIndexBlockService>()
            .ProcessToIndexBlock();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Get parent path
        string str_directory = Environment.CurrentDirectory.ToString();
        string parent = System.IO.Directory.GetParent(str_directory).Parent.Parent.FullName;
      
        // Create logger with serilog
        var serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Information()
           .WriteTo.File($"{parent}/Logs/log{CommonHelper.FormatDateTimeToShortString()}.txt")
           .CreateLogger();

        // Register services and logging
        services
            .AddLogging(configure =>
            {
                configure.ClearProviders();
                configure.AddFilter("Microsoft", LogLevel.Warning);
                configure.AddFilter("System", LogLevel.Warning);
                configure.AddSerilog(logger: serilogLogger, dispose: true);
                configure.AddConsole();
            })
            .AddHttpClient<IEtherscanService, EtherscanService>(http =>
            {
                http.BaseAddress = new System.Uri(CommonHelper.EtherscanBaseAddress);
                http.DefaultRequestHeaders.Accept.Clear();
                http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });
    }
}