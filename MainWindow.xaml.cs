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
        private static UDPReceiver uDPR;
        private static Thread connectionThread;
        private static SignalRHandler sRH;

        public MainWindow()
        {
            InitializeComponent();

            uDPR = new UDPReceiver();
            uDPR.Create();

            connectionThread = new Thread(new ThreadStart(Loop));

            sRH = new SignalRHandler();
            sRH.Connect_();
        }

        public void EngageLoop(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            connectionThread.Start();
        }

        private void Loop()
        {
            while (true)
            {
                uDPR.socket.BeginReceive(new AsyncCallback(OnUdpData), uDPR.socket);
                Thread.Sleep(250);
            }
        }

        private void OnUdpData(IAsyncResult result)
        {
            string message = uDPR.DecipherMessage(result);
            AddToMessageBox($"Data: {message} from Arduino ..probably.. :)");
            sRH.SendPlanet(message);
        }


        public void AddToMessageBox(string message)
        {
            Dispatcher.BeginInvoke(
                (Action)(() => 
                    MessageBox.Text = message + "\n" + MessageBox.Text
                    ));
        }
    }
}
