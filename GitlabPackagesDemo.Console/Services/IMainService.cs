namespace GitlabPackagesDemo.Console.Services;

public interface IMainService
{
    Task CreatePackagesFile(string filePath, bool writeFullPath);
}