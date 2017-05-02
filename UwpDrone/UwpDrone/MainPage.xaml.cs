using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MavLinkUwp;
using Windows.Devices.Gpio;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.System;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UwpDrone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        static string[] laws =
        {
            "A drone may not injure a human being or, through inaction, allow a human being to come to harm.",
            "A drone must obey orders given it by human beings except where such orders would conflict with the First Law.",
            "A drone must protect its own existence as long as such protection does not conflict with the First or Second Law."
        };
        //          - Isaac Asimov


        FlightController fc = new FlightController();
        DroneHttpServer server;
        DispatcherTimer _timer = new DispatcherTimer();
        CameraHandler _handler = new CameraHandler();

        public DroneControlViewModel VM { get; set; }

        public string[] directives
        {
            get
            {
                return laws;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void _timer_Tick(object sender, object e)
        {
            var ignore = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var xInFeet = fc.XInCM / 2.54 / 12.0;
                var yInFeet = fc.YInCM / 2.54 / 12.0;

                VM.FeetX = xInFeet;
                VM.FeetY = yInFeet;
                VM.Heading = fc.Heading - 180.0;    // -180 - 180
                VM.BatteryLevel = fc.BatteryLevel;
                VM.Roll = fc.Roll;
            });
        }

        private void Arm_Click(object sender, RoutedEventArgs e)
        {
            fc.ToggleArm();
        }

        private void HoldAt15_Click(object sender, RoutedEventArgs e)
        {
            fc.flyToHeight(0.5f);
        }

        private void Land_Click(object sender, RoutedEventArgs e)
        {
            fc.land();
        }

        private async void Takeoff_Click(object sender, RoutedEventArgs e)
        {
            await fc.takeoff();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ProxyIp.Text) == false)
            {
                fc.Proxy(ProxyIp.Text);
            }
        }

        private void FlyForward_Click(object sender, RoutedEventArgs e)
        {
            fc.flyForward(10.0);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            VM = new DroneControlViewModel();
            this.DataContext = VM;

            //Handle Power Button
            DroneOverlay.PowerButtonClicked += delegate (object s, EventArgs args)
            {
                new System.Threading.Tasks.Task(() =>
                {
                    //ShutdownManager.BeginShutdown(ShutdownKind.Shutdown, TimeSpan.FromSeconds(0));
                }).Start();
            };

            await fc.initialize();

            server = new DroneHttpServer(3000, fc);

            server.StartServer();

            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += _timer_Tick;
            _timer.Start();

            var hostNames = NetworkInformation.GetHostNames();
            var hostName = hostNames.FirstOrDefault(name => name.Type == HostNameType.DomainName)?.DisplayName ?? "???";
            VM.Title = hostName;
            VM.Objective = "Objective: not fall out of the sky";
            VM.Status = "Hello";

            try
            {
                await _handler.initialize(CameraDisplay);
            }
            catch (Exception ex)
            {
                // sometimes...
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
