using Microsoft.ServiceBus.Messaging;
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

namespace ScareFloor.LeaderBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string connectionString = "HostName=ScareFloor.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=tCrFeIzJOYwCIFHXtTpnnBek9cDNYyr/2z0Rt52eh/w=";
        string iotHubD2cEndpoint = "messages/events";
        int leaderboardValue = 0;

        public MainWindow()
        {
            InitializeComponent();
            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);

            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            foreach (string partition in d2cPartitions)
            {
                var receiver = eventHubClient.GetDefaultConsumerGroup().
                    CreateReceiver(partition, DateTime.Now);
                ReceiveMessagesFromDeviceAsync(receiver);
            }

            async Task ReceiveMessagesFromDeviceAsync(EventHubReceiver receiver)
            {
                while (true)
                {
                    EventData eventData = await receiver.ReceiveAsync();
                    if (eventData == null) continue;

                    string data = Encoding.UTF8.GetString(eventData.GetBytes());
                    if (data == "Filled!")
                    {
                        leaderboardValue++;
                        LeaderboardValue.Text = Convert.ToString(leaderboardValue);
                       
                    }
                    Console.WriteLine("Message received: '{0}'", data);
                }
            }
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            leaderboardValue = 0;
            LeaderboardValue.Text = Convert.ToString(leaderboardValue);
        }
    }
}
