using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace AlbertSavesThePlanets2UDPReceiverAndSignalRSender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection connection;
        public MainWindow()
        {
            InitializeComponent();

            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:32258/ChatHub")
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
            // UDP
            Create();
            Connect_();
        }

        public async void Loop(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                //BeginReceive();
                SendPlanet("BENIS HAHA");
                Thread.Sleep(500);
            }
        }


        //  UDP
        private static UdpClient socket;
        public static void Create()
        {
            if (socket == null)
                socket = new UdpClient(8080);
        }

        public void BeginReceive()
        {
            socket.BeginReceive(new AsyncCallback(OnUdpData), socket);
        }

        private void OnUdpData(IAsyncResult result)
        {
            UdpClient socket = result.AsyncState as UdpClient;
            IPEndPoint source = new IPEndPoint(0, 0);
            byte[] message = socket.EndReceive(result, ref source);
            string returnData = Encoding.ASCII.GetString(message);
            Console.WriteLine("Data: " + returnData.ToString() + " from " + source);
            SendPlanet(returnData);
        }
        //  UDP END


        private async void Connect_()
        {
            await ConnectSignalR();
        }

        private async Task ConnectSignalR()
        {
            connection.On<string>("ReceiveMessage", (message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{message}";
                });
            });

            await connection.StartAsync();
        }

        private async void SendPlanet(string planet)
        {
            try
            {
                await connection.InvokeAsync("SendMessage", planet);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
