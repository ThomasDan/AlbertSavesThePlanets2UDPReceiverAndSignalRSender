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
        public static Thread signalRThread;
        public static object MessageBoxLock = new object();

        public MainWindow()
        {
            InitializeComponent();

            signalRThread = new Thread(new ThreadStart(Loop_True));


            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:32258/ChatHub")
                .Build();



            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            Connect_();

            // UDP
            Create();
        }

        public async void Loop(object sender, RoutedEventArgs e)
        {
            signalRThread.Start();
        }

        

        internal async void Loop_True()
        {
            /*Dictionary<int, string> dictionary = new Dictionary<int, string>()
            {
                { 5, "2021353134" },     // Mercury
                { 6, "16914689137" },     // venus
                { 3, "1034590137" },     // Earth
                { 0, "121164164110" },   // Mars
                { 8, "9717780195" },     // Sayt'urn
                { 2, "16916921689" },    // Juppiter
                { 7, "5716120143" },     // Uranus
                { 1, "897121989" },     // Neptune
                { 4, "16317989137" }     // pluto
            };*/

            while (true)
            {
                BeginReceive();
                Thread.Sleep(250);
                /*foreach (KeyValuePair<int, string> kvp in dictionary)
                {
                    SendPlanet(kvp.Key + kvp.Value);
                }*/
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
            socket = result.AsyncState as UdpClient; // UdpClient
            IPEndPoint source = new IPEndPoint(0, 0);
            byte[] message = socket.EndReceive(result, ref source);
            string returnData = Encoding.ASCII.GetString(message);
            Console.WriteLine("Data: " + returnData.ToString() + " from " + source);
            AddToMessageBox($"Data: {returnData.ToString()} from {source}");
            SendPlanet(returnData);
        }
        //  UDP END

        public void AddToMessageBox(string message)
        {
            /*
            Monitor.Enter(MessageBoxLock);
            try
            {
                messageBoxText += "\n" + message;
            }
            finally
            {
                Monitor.PulseAll(MessageBoxLock);
                Monitor.Exit(MessageBoxLock);
            }
            */
            Dispatcher.BeginInvoke((Action)(() => MessageBox.AppendText("\n" + message)));
        }

        private async void Connect_()
        {
            AddToMessageBox("Await connect Signal R");
            await ConnectSignalR();
        }

        private async Task ConnectSignalR()
        {
            connection.On<string>("ReceiveMessage", (message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{message}";
                    AddToMessageBox("Connection on, dispatcher invoke, new message");
                });
            });

            try
            {
                await connection.StartAsync();
                AddToMessageBox("Connection started Async");
            }
            catch (Exception ex)
            {
                Connect_();
                AddToMessageBox($"Connection exception {ex.Message}");
            }
        }

        private async void SendPlanet(string planet)
        {
            try
            {
                await connection.InvokeAsync("SendMessage", planet);
                AddToMessageBox($"Send planet, connection invoke async, send message planet");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                AddToMessageBox($"Send planet exception, {ex.Message}");
            }
        }
    }
}
