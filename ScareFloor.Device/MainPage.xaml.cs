﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Audio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Text;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ScareFloor.Device
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DeviceClient deviceClient;
        private LightControl lightControl;
        public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>();

        public MainPage()
        {

            InitializeComponent();
            InitDeviceClient();
            ReceiveMessage();
            InitLightControl();
          
            MessageList.ItemsSource = Messages;
        }


        private void InitLightControl()
        {
            var gpio = GpioController.GetDefault();
            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                GpioStatus.Text = "There is no GPIO controller on this device.";
                return;
            }

            lightControl = new LightControl(gpio);
            GpioStatus.Text = "GPIO pin initialized correctly.";
        }

        private void InitDeviceClient()
        {
            string iotHubUri = "ScareFloor.azure-devices.net"; // ! put in value !
            string deviceId = "scarefloor"; // ! put in value !
            string deviceKey = "nJQ8o1NRGh6N39OD6dwpICXDi/n+KMZ0dXQpfPB0VqA="; // ! put in value !

            deviceClient = DeviceClient.Create(iotHubUri,
                    AuthenticationMethodFactory.
                        CreateAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey),
                    TransportType.Http1);
        }

        private void LightToggle(object sender, RoutedEventArgs e)
        {
            var status = lightControl.GetLightMeterStatus();
            if (status == LightControl.LightMeterStatus.Filled || status == LightControl.LightMeterStatus.PartiallyFilled)
            {
                lightControl.TurnAllLightsOff();
            }
            else
            {
                lightControl.TurnAllLightsOn();
            }
        }

        private async void SendMessage(object sender, RoutedEventArgs e)
        {
            var str = "Hello, Cloud!";
            var message = new Message(Encoding.ASCII.GetBytes(str));

            await deviceClient.SendEventAsync(message);
            MessageStatus.Text = "Sent";
            Messages.Add($"{str} (sent)");
        }

        private async void ReceiveMessage()
        {
            MessageReceived.Text = "Receiving cloud to device messages from service";
            while (true)
            {
                Message receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;
                var initialLightMeterStatus = lightControl.GetLightMeterStatus();
               

                var message = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                MessageReceived.Text = message;
                Messages.Add($"{message} (received)");

                //TODO: Figure out a better way to determine if laughter is involved, and better eliminate false positives.
                var containsLaughter = message.ToLowerInvariant().Contains("ha ")
                                        || message.ToLowerInvariant().Contains("ha.")
                                        || message.ToLowerInvariant() == "ha";

                if (message.ToLowerInvariant().Contains("ha") && initialLightMeterStatus != LightControl.LightMeterStatus.Filled)
                {
                    var laughterNumber = Regex.Matches(Regex.Escape(message.ToLowerInvariant()), "ha").Count;
                    MessageReceived.Text += $" {laughterNumber}";
                    lightControl.IncrementLightOn();
                    var status = lightControl.GetLightMeterStatus();
                    if (status == LightControl.LightMeterStatus.Filled)
                    {
                        SendFilledMessage();
                    }
                }
                else if (message.ToLowerInvariant().Contains("reset"))
                {
                    MessageReceived.Text = "Reset the lights";
                    lightControl.TurnAllLightsOff();
                }

                await deviceClient.CompleteAsync(receivedMessage);
            }
        }

        private async void SendFilledMessage()
        {
            var str = "Filled!";
            var message = new Message(Encoding.ASCII.GetBytes(str));

            await deviceClient.SendEventAsync(message);
            MessageStatus.Text = "Sent";
            Messages.Add($"{str} (sent)");
        }

        private void NavigateToWifi(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SDKTemplate.WifiConnectPage));
        }
    }
}
