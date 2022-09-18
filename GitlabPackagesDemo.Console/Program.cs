using CommandLine;
using GitlabPackagesDemo.Common.Settings;
using GitlabPackagesDemo.Console.Configuration;
using GitlabPackagesDemo.Console.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

//TODO: Check unused nuget-packages in GitlabPackagesDemo.Common
//TODO: Проверь, что все классы лежат в нужных сборках. Если класс используется в разных проектах, то в Common. Если нет, то в Console или WPF
IHost host;
try
{
    var config = CreateConfiguration();
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(config)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger();
    host = ConfigureServices(config);

    var parserResult = Parser.Default.ParseArguments<CreatePackagesFileOptions>(args);
    await parserResult.WithParsedAsync(HandleParseSuccess);
    await parserResult.WithNotParsedAsync(HandleParseError);
}
catch (Exception ex)
{
    Log.Logger.Error(ex, "Unexpected exception occured");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

IConfigurationRoot CreateConfiguration()
{
    return new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
            optional: true)
        .Build();
}

static IHost ConfigureServices(IConfigurationRoot configuration) => Host.CreateDefaultBuilder()
    .ConfigureServices((_, services) =>
    {
        services
            .Configure<GitLabSettings>(configuration.GetSection(GitLabSettings.Key))
            .Configure<SearchSettings>(configuration.GetSection(SearchSettings.Key));

        services.AddScoped<IMainService, MainService>();
    })
    .UseSerilog()
    .Build();

Task HandleParseSuccess(CreatePackagesFileOptions options)
{
    Log.Logger.Information("Parameters obtained: {Source}, {Destination}", options.FilePath, options.WriteFullPath);
    var mainService = host.Services.GetService<IMainService>() ??
                      throw new NullReferenceException(nameof(IMainService));
    return mainService.CreatePackagesFile(options.FilePath, options.WriteFullPath);
}

static Task HandleParseError(IEnumerable<Error> errors)
{
    var items = errors.ToList();
    if (items.IsVersion() || items.IsHelp()) return Task.CompletedTask;
    Log.Logger.Error("{Errors}", string.Join(',', items));
    return Task.CompletedTask;
}