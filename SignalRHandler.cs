using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;

namespace AlbertSavesThePlanets2UDPReceiverAndSignalRSender
{
    internal class SignalRHandler
    {
        private static HubConnection connection;

        internal SignalRHandler()
        {
            InitializeSignalR();
        }

        private void InitializeSignalR()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:32258/ChatHub")
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
        }


        internal async Task Connect_()
        {
            //AddToMessageBox("Awaiting connection to Signal R");
            await ConnectSignalR();
        }

        internal async Task ConnectSignalR()
        {
            /* // We don't do anything with Received messages from the website, but the website does send one on occasion, just to maintain the connection.
            connection.On<string>("ReceiveMessage", (message) =>
            {
                
                Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{message}";
                    //AddToMessageBox("Connection on, dispatcher invoke, new message");

                });
                
            });
            //*/

            try
            {
                await connection.StartAsync();
                //AddToMessageBox("Connection started Async");
            }
            catch (Exception ex)
            {
                Connect_();
                //AddToMessageBox($"Connection exception {ex.Message}");
            }
        }
        public async Task SendPlanet(string planet)
        {
            try
            {
                await connection.InvokeAsync("SendMessage", planet);
                //AddToMessageBox($"Send planet, connection invoke async, send message planet");
            }
            catch (Exception ex)
            {
                //AddToMessageBox($"Send planet exception, {ex.Message}");
            }
        }
    }
}
