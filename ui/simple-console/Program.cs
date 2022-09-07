using Cake.Figlet;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System.Text;

namespace mqtt.demo
{
    internal class Program
    {
        private static readonly MqttConfiguration config = new("127.0.0.1", 1883, "protolabs/plymouth/sensors/outside-temp");
        private static int width = 0;
        private static int height = 0;
        private static MqttStatus mqttStatus = new ("Starting up...", false, null);
        // Establish a Task Cancellation Policy
        private static CancellationTokenSource source = new();
        private static CancellationToken token = source.Token;
        private static bool isConnecting = false;

        static async Task Main(string[] args)
        {
            InspectArgs(args);
            mqttStatus = new MqttStatus("New MQTT Status", true, await ConnectToMqttBroker(config, WriteAt));
            PresentMenuOptions();
            
            // Waiting for KeyPress Loop...
            do
            {
                while (!Console.KeyAvailable)
                {
                    Footer();
                    Thread.Sleep(350);
                }
            } while (!ShouldIQuit());
        }

        private static async Task<IMqttClient> ConnectToMqttBroker(MqttConfiguration cfg, Action<string, int, int, bool> writeAtCallback)
        {
            // Prepare interactions with MQTT Broker via MQTT Factory
            var mqttFactory = new MqttFactory();
            var mqttClient = mqttFactory.CreateMqttClient();
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(cfg.IpAddress, cfg.Port)
                .Build();

            // Handles what happens when a new message is received
            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                var payloadAsJson = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                var definition = new { value = 0, units = "", unitsDesc = "", timestampUtc = "", timestampLocal = "" };
                var deserializedJson = JsonConvert.DeserializeAnonymousType(payloadAsJson, definition);
                writeAtCallback($"Topic: {e.ApplicationMessage.Topic}", 11, 1, false);
                writeAtCallback($"Received application message. Temp:{deserializedJson.value} {deserializedJson.units} @ {DateTime.UtcNow}", 10, 1, false);
                return Task.CompletedTask;
            };

            // Handles what happens when the client detects it got disconnected from MQTT Broker & attempts to reconnect
            mqttClient.DisconnectedAsync += e =>
            {
                return Task.Run(
                    async () =>
                    {
                        if (e.ClientWasConnected && !source.IsCancellationRequested)
                        {
                            writeAtCallback("Cancelling token...", 10, 1, true);
                            source.Cancel();
                            isConnecting = true;
                        }
                        while (isConnecting)
                        {
                            try
                            {
                                if (!mqttClient.IsConnected)
                                {
                                    writeAtCallback("Disconnected client detected, attempting to reconnect.", 11, 1, false);
                                    source = new CancellationTokenSource(); token = source.Token;
                                    await mqttClient.ConnectAsync(mqttClientOptions, token);

                                    // Subscribe to topics when session is clean etc.
                                    writeAtCallback($"MQTT connected. Topic [{mqttClient.Options.WillTopic}]", 9, 1, true);
                                    writeAtCallback(" ", 11, 1, false);
                                }
                            }
                            catch (Exception)
                            {
                                    // Handle the exception properly (logging etc.).                                
                            }
                            finally
                            {
                                await Task.Delay(TimeSpan.FromSeconds(2));
                            }
                        }
                    });
            };

            mqttClient.ConnectingAsync += e =>
            {
                return Task.Run(
                    async () =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(2));
                        if (mqttClient.IsConnected)
                        {
                            isConnecting = false;
                        }
                        else
                        {
                            isConnecting = true;
                        }
                    });
            };

            // Handles what happens when MQTT Client is connected
            mqttClient.ConnectedAsync += e =>
            {
                return Task.Run(async () =>
                {
                    var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                        .WithTopicFilter(f => { f.WithTopic(cfg.Topic); })
                        .Build();

                    await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

                    // writeAtCallback($"MQTT client subscribed to topic [{cfg.Topic}].", 9, 1);
                });
            };

            // Attempts to connect for the first time
            await mqttClient.ConnectAsync(mqttClientOptions, token);
            return mqttClient;
        }

        private static bool ShouldIQuit()
        {
            if (mqttStatus == null) { throw new NullReferenceException($"MQTT Status is not being reported - {nameof(mqttStatus)}"); }
            bool shouldIQuit;
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Q:
                    shouldIQuit = true;
                    break;
                default:
                    shouldIQuit = false;
                    break;
            }
            return shouldIQuit;
        }

        private static void Footer()
        {
            var dateInfo = $"UTC Date/Time: {DateTime.UtcNow}";
            var haw = $"H: {height} W: {width}";
            var blankspace = width - dateInfo.Length - haw.Length - mqttStatus.ToString().Length;
            var buffer = new string(' ', blankspace/2);
            var buffer2 = blankspace % 2 == 0 ? "" : " ";
            Console.SetCursorPosition(0, height-1);
            Console.ForegroundColor = ConsoleColor.Gray; Console.BackgroundColor = ConsoleColor.Blue;
            Console.Write($"{dateInfo}{buffer}{mqttStatus}{buffer}{buffer2}{haw}");
            Console.ResetColor();
        }

        private static void InspectArgs(string[] args)
        {
            if (args.Any())
            {
                Console.WriteLine(FigletAliases.Figlet(null, "Inspecting Arguments:"));
                foreach (var arg in args)
                {
                    Console.WriteLine($"Next Arg: {arg}");
                }

            }        
        }

        private static void PresentMenuOptions()
        {
            width = Console.WindowWidth;
            height = Console.WindowHeight;
            Console.WriteLine(FigletAliases.Figlet(null, "MQTT    Test"));
            Label("Menu", width);
            Console.WriteLine();
            Console.WriteLine("Q - To quit program");
            Footer();
        }

        private static void Label(string text = "missing label", int? width = null)
        {
            string? label;
            if (!width.HasValue)
            {
                label = $" {text ?? "missing label"} ";
            }
            else
            {
                var filler = new String('═', ((width ?? 30) - (text ?? "missing label").Length - 4));
                label = $"══ {text} {filler}";
            }
            Console.Write(label);
        }

        private static void WriteAt(string labelText, int nTop, int nLeft, bool refresh = false)
        {
            if (refresh)
            {
                Console.Clear();
                PresentMenuOptions();
            }
            var (cLeft, cTop) = Console.GetCursorPosition();
            Console.SetCursorPosition(nLeft, nTop);
            Console.Write(new String(' ', width));
            Console.Write(labelText.CenterString(width));
            Console.SetCursorPosition(cLeft, cTop);
        }

        private static void Separator(char? c = '-', int? length = null)
        {
            Console.WriteLine(new String(c ?? '?', length ?? width));
        }
    }
}