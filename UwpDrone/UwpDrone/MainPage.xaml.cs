using System;
using System.Collections.Generic;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UwpDrone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MSP _msp = new MSP();
        Gamepad _controller;
        DispatcherTimer _timer = new DispatcherTimer();

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //await _msp.connect();
            _timer.Interval = TimeSpan.FromMilliseconds(300);
            _timer.Tick += _timer_Tick;
            _timer.Start();
            Gamepad.GamepadAdded += Gamepad_GamepadAdded;
            Gamepad.GamepadRemoved += Gamepad_GamepadRemoved;

        }
        private void _timer_Tick(object sender, object e)
        {
            if (_controller == null)
            {
                return;
            }

            GamepadReading reading = _controller.GetCurrentReading();

            if (reading.Buttons.HasFlag(GamepadButtons.A))
            {
                _msp.ToggleArm();
            }
        }

        private void Gamepad_GamepadRemoved(object sender, Gamepad e)
        {
            if (_controller == e)
            {
                _controller = null;
            }
        }

        private void Gamepad_GamepadAdded(object sender, Gamepad e)
        {
            if (_controller == null)
            {
                _controller = e;
            }
        }
    }
}
