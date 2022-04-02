using OpenMacroBoard.SDK;
using StreamDeckSharp;
using System;
using System.Drawing;
using System.Linq;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Websocket.Client;

namespace StreamDeckDocker
{
    public class Program
    {
        private WebsocketClient websocket;

        private static async void Main(string[] args)
        {
            RootCommand rootCommand = new RootCommand();
            rootCommand.Add(new Option<int>("-port"));
            rootCommand.Add(new Option<string>(new[] { "-pluginUUID", "-pluginuuid" }));
            rootCommand.Add(new Option<string>(new[] { "-registerEvent", "-registerevent" }));
            rootCommand.Add(new Option<string>("-info"));

            var program = new Program();
            rootCommand.Handler = CommandHandler.Create<int, string, string, string>(program.Run);
            await rootCommand.InvokeAsync(args);
            
            //DockerTest();

            //using (var deck = StreamDeck.OpenDevice())
            //{
            //    deck.SetBrightness(100);

            //    //var b = Brushes.White;
            //    //var f = new Font("Arial", 20);
            //    //var fb = new Font("Arial", 60, FontStyle.Bold);
            //    //var origin = new PointF(30, 33);
            //    ////Send the bitmap informaton to the device
            //    //for (int i = 0; i < deck.Keys.Count; i++)
            //    //{
            //    //    //deck.SetKeyBitmap(i, rowColors[i / 5]);
            //    //    var bmp = KeyBitmap.Create.FromGraphics(100, 100, (g) =>
            //    //    {
            //    //        g.DrawString($"{i,2}", f, b, origin);
            //    //    });
            //    //    deck.SetKeyBitmap(i, bmp);
            //    //}

            //    Console.ReadKey();
            //}
        }

        public void Run(int port, string pluginUUID = "uuid", string registerevent = "event", string info = "default info")
        {
            Console.WriteLine($"Port: {port}");
            Console.WriteLine($"PluginUUID: {pluginUUID}");
            Console.WriteLine($"RegisterEvent: {registerevent}");
            Console.WriteLine($"Info: {info}");

            websocket = new WebsocketClient(new Uri("ws://localhost:" + port));
            websocket.MessageReceived.Subscribe(MessageReceived);

            Console.ReadKey();
        }
        
        private void MessageReceived(ResponseMessage message)
        {
            Console.WriteLine(message.Text);
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
}
