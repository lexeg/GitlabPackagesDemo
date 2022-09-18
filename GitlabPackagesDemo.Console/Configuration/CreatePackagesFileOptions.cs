using CommandLine;

namespace GitlabPackagesDemo.Console.Configuration
{
    [Verb("createPackagesFile", isDefault: false, HelpText = "--path <PATH_TO_FILE> [--fullPath [true|false]]: creating a file with nuget packages")]
    public class CreatePackagesFileOptions
    {
        [Option(shortName: 'p', longName: "path", Required = true, HelpText = "path to file")]
        public string FilePath { get; set; }

        [Option(shortName: 'f', longName: "fullPath", Required = false, HelpText = "write fool path to project")]
        public bool WriteFullPath { get; set; }
    }
}