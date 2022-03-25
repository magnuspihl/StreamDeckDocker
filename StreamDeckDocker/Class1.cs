using Docker.DotNet;
using Docker.DotNet.Models;
using OpenMacroBoard.SDK;
using StreamDeckSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace StreamDeckDocker
{
    public class Class1
    {
        static void Main(string[] args)
        {
            DockerTest();

            using (var deck = StreamDeck.OpenDevice())
            {
                deck.SetBrightness(100);

                //var b = Brushes.White;
                //var f = new Font("Arial", 20);
                //var fb = new Font("Arial", 60, FontStyle.Bold);
                //var origin = new PointF(30, 33);
                ////Send the bitmap informaton to the device
                //for (int i = 0; i < deck.Keys.Count; i++)
                //{
                //    //deck.SetKeyBitmap(i, rowColors[i / 5]);
                //    var bmp = KeyBitmap.Create.FromGraphics(100, 100, (g) =>
                //    {
                //        g.DrawString($"{i,2}", f, b, origin);
                //    });
                //    deck.SetKeyBitmap(i, bmp);
                //}

                Console.ReadKey();
            }
        }

        private static void DockerTest()
        {
            
            var projects = DockerContainerProject.GetDockerContainerProjects();
            foreach(var p in projects)
            {
                Console.WriteLine($"Project: {p.Project}, CWD: {p.WorkingDirectory}, Containers: {p.Containers.Count()}");
            }
        }
    }

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
