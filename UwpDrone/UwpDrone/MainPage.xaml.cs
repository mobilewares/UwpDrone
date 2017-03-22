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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UwpDrone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        FlightController fc = new FlightController();
        DroneHttpServer server;
        DispatcherTimer _timer = new DispatcherTimer();

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //await _msp.connect();

            await fc.initialize();

            server = new DroneHttpServer(3000, fc);

            server.StartServer();

            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void _timer_Tick(object sender, object e)
        {
            /*
            var ignore = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                float volt = (float)fc.voltage;
                volt /= 10.0f;
                AltValue.Text = fc.Altitude.ToString();
                BatValue.Text = volt.ToString();
            });
            */
        }

        private void Arm_Click(object sender, RoutedEventArgs e)
        {
            fc.ToggleArm();
        }

        private void HoldAt15_Click(object sender, RoutedEventArgs e)
        {
            fc.flyToHeight(15);
        }

        private void Land_Click(object sender, RoutedEventArgs e)
        {
            fc.land();
        }
    }
}
