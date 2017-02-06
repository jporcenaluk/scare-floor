using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ScareFloor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public AudioGraph ScareAudioGraph { get; set; }
        public AudioDeviceInputNode ScareDeviceInputNode { get; set; }

        public MainPage()
        {
            InitializeComponent();
            InitAudioGraph();
            CreateDeviceInputNode();
        }


        private async Task InitAudioGraph()
        {

            AudioGraphSettings settings = new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media);

            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);
            if (result.Status != AudioGraphCreationStatus.Success)
            {
                ShowErrorMessage("AudioGraph creation error: " + result.Status.ToString());
            }

            ScareAudioGraph = result.Graph;

        }

        private async Task CreateDeviceInputNode()
        {
            // Create a device output node
            CreateAudioDeviceInputNodeResult result = await ScareAudioGraph.CreateDeviceInputNodeAsync(Windows.Media.Capture.MediaCategory.Media);

            if (result.Status != AudioDeviceNodeCreationStatus.Success)
            {
                // Cannot create device output node
                ShowErrorMessage(result.Status.ToString());
                return;
            }

            ScareDeviceInputNode = result.DeviceInputNode;
        }

        private void ShowErrorMessage(string v)
        {
            this.HelloMessage.Text = v;
        }

        private void ClickMe_Click(object sender, RoutedEventArgs e)
        {
            this.HelloMessage.Text = "Hey I'm from Windows 10 IoT Core!";
        }
    }
}
