using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Process = System.Diagnostics.Process;

namespace AzureDevOpsBackup // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {

            var organizationName = "";
            var pat = ""; // Personal Access Token to read projects and repos
            var azureDevOpsUri = "https://dev.azure.com/" + organizationName;

            var connection = new VssConnection(
                new Uri(azureDevOpsUri),
                new VssBasicCredential(
                    string.Empty,
                    pat
                )
            );

            var projectHttpClient = connection.GetClient<ProjectHttpClient>();
            var gitHttpClient = connection.GetClient<GitHttpClient>();
            var projects = await projectHttpClient.GetProjects();

            foreach (var project in projects)
            {
                Console.WriteLine("==={0}===", project.Name);
                Console.WriteLine("Repositories:");
                var repositories = await gitHttpClient.GetRepositoriesAsync(project.Name);
                foreach (var repository in repositories)
                {
                    Console.WriteLine("\t{0}", repository.WebUrl);
                    var cloneUrl = repository.WebUrl.Replace("dev.azure.com", $"{pat}@dev.azure.com");
                    await Process.Start("git", $"clone {cloneUrl}").WaitForExitAsync();
                }
            }
        }
    }
}