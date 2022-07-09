using System;
using System.IO;
using System.Windows;
using GitlabPackagesDemo.Common;
using GitlabPackagesDemo.Helpers;
using GitlabPackagesDemo.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace GitlabPackagesDemo.Views;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public App()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.Configure<GitLabSettings>(_configuration.GetSection(nameof(GitLabSettings)));
        services.Configure<SearchSettings>(_configuration.GetSection(nameof(SearchSettings)));

        services
            .AddScoped(typeof(FileSaver))
            .AddScoped(typeof(RepositoryService))
            .AddScoped(typeof(DialogFactory))
            .AddScoped(typeof(MainWindow));

        services.AddScoped<ILoggerFactory, NullLoggerFactory>();
        services.AddScoped(typeof(ILogger<>), typeof(Logger<>));
    }
}