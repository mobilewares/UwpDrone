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

            await _msp.connect();
            _msp.ChannelDelegate += _msp_ChannelDelegate;
            _timer.Interval = TimeSpan.FromMilliseconds(300);
            _timer.Tick += _timer_Tick;
            _timer.Start();
            Gamepad.GamepadAdded += Gamepad_GamepadAdded;
            Gamepad.GamepadRemoved += Gamepad_GamepadRemoved;

        }

        private void _msp_ChannelDelegate()
        {
            var ignore = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ArmProgress.Value = _msp.receiver[MSP.Channels.Arm];
                ArmValue.Text = ArmProgress.Value.ToString();
                ThrottleProgress.Value = _msp.receiver[MSP.Channels.Throttle];
                ThrottleValue.Text = ThrottleProgress.Value.ToString();
            });
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

        private void ArmButton_Click(object sender, RoutedEventArgs e)
        {
            _msp.ToggleArm();
        }

        double XFromJoystick(VirtualJoystick.VirtualJoystick vj)
        {
            double x = Math.Sin(DegToRad(vj.Angle)) * vj.Distance;

            return x;
        }

        double YFromJoystick(VirtualJoystick.VirtualJoystick vj)
        {
            double y = Math.Cos(DegToRad(vj.Angle)) * vj.Distance;

            return y;
        }

        private void VirtualJoystick_LeftStickMove(object sender, EventArgs e)
        {
            double x = XFromJoystick(JoystickLeft);
            double y = YFromJoystick(JoystickLeft);

            // x is yaw.
            // y is throttle. Centered is Zero, top is 100%.
            if (y > -double.Epsilon)
            {
                double throttle = y / 100.0;
                Debug.WriteLine("Throttle: " + throttle.ToString());

                _msp.Throttle = throttle;
            }
        }

        private void VirtualJoystick_RightStickMove(object sender, EventArgs e)
        {
            //AngleCurr.Text = "Angle : " + Joystick.Angle.ToString();
            //DistCurr.Text = "Distance : " + Joystick.Distance.ToString();
        }

        private void Joystick_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private double DegToRad(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
