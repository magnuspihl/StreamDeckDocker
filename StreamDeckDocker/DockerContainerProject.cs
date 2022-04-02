using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamDeckDocker
{
    public class DockerContainerProject
    {
        const string ProjectLabel = "com.docker.compose.project";
        const string WorkingDirLabel = "com.docker.compose.project.working_dir";

        public string Project { get; set; }
        public string WorkingDirectory { get; set; }
        public IReadOnlyList<ContainerListResponse> Containers { get; set; }

        private DockerContainerProject(string projectName, IEnumerable<ContainerListResponse> containers)
        {
            Project = projectName;
            Containers = containers.Where(c => c.Labels[ProjectLabel] == projectName).ToList();
            WorkingDirectory = containers.FirstOrDefault()?.Labels[WorkingDirLabel] ?? "";
        }

        public static IEnumerable<DockerContainerProject> GetDockerContainerProjects()
        {
            using (DockerClient client = new DockerClientConfiguration().CreateClient())
            {
                var containers = Task.Run(() => client.Containers.ListContainersAsync(new ContainersListParameters())).Result;
                Console.WriteLine($"Docker Containers: {containers.Count}");
                return containers.Select(c => c.Labels[ProjectLabel]).Distinct().Select(p => new DockerContainerProject(p, containers));
            }
        }
    }
}
