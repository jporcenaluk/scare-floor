using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace ScareFloor.ReadHubMessages
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "HostName=ScareFloor.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=tCrFeIzJOYwCIFHXtTpnnBek9cDNYyr/2z0Rt52eh/w=";
            string iotHubD2cEndpoint = "messages/events";

            var eventHubClient = EventHubClient.
                CreateFromConnectionString(connectionString, iotHubD2cEndpoint);

            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            foreach (string partition in d2cPartitions)
            {
                var receiver = eventHubClient.GetDefaultConsumerGroup().
                    CreateReceiver(partition, DateTime.Now);
                ReceiveMessagesFromDeviceAsync(receiver);
            }
            Console.ReadLine();
        }

        async static Task ReceiveMessagesFromDeviceAsync(EventHubReceiver receiver)
        {
            while (true)
            {
                EventData eventData = await receiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine("Message received: '{0}'", data);
            }
        }
    }
}
