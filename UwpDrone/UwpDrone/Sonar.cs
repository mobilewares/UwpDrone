using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using System.Diagnostics;
using Windows.UI.Xaml;

namespace UwpDrone
{
    public class SonarInTheRound
    {
        // Common Trigger Pin
        const int kTriggerPin = 0;
        const int kEchoFront = 1;
        const int kEchoBack = 2;
        const int kEchoLeft = 3;
        const int kEchoRight = 4;

        GpioPin triggerPin;
        GpioPin echoFrontPin;
        GpioPin echoBackPin;
        GpioPin echoLeftPin;
        GpioPin echoRightPin;


        public double FrontDistance { get; internal set; } = 0.0;
        public double BackDistance { get; internal set; } = 0.0;
        public double LeftDistance { get; internal set; } = 0.0;
        public double RightDistance { get; internal set; } = 0.0;

        Stopwatch frontTime = new Stopwatch();
        Stopwatch backTime = new Stopwatch();
        Stopwatch leftTime = new Stopwatch();
        Stopwatch rightTime = new Stopwatch();

        DispatcherTimer timer = new DispatcherTimer();


        public SonarInTheRound()
        {

        }

        public async Task initialize()
        {
            var controller = await GpioController.GetDefaultAsync();
            if (controller == null)
            {
                // Rogin?
                return;
            }

            Debug.WriteLine($"High Resolution {Stopwatch.IsHighResolution} ");

            triggerPin = controller.OpenPin(kTriggerPin);
            echoFrontPin = controller.OpenPin(kEchoFront);
            echoBackPin = controller.OpenPin(kEchoBack);
            echoLeftPin = controller.OpenPin(kEchoLeft);
            echoRightPin = controller.OpenPin(kEchoRight);

            triggerPin.SetDriveMode(GpioPinDriveMode.Output);
            echoFrontPin.SetDriveMode(GpioPinDriveMode.Input);
            echoBackPin.SetDriveMode(GpioPinDriveMode.Input);
            echoLeftPin.SetDriveMode(GpioPinDriveMode.Input);
            echoRightPin.SetDriveMode(GpioPinDriveMode.Input);

            echoFrontPin.DebounceTimeout = TimeSpan.FromMilliseconds(0);

            echoFrontPin.ValueChanged += EchoFrontPin_ValueChanged;
            echoBackPin.ValueChanged += EchoBackPin_ValueChanged;
            echoLeftPin.ValueChanged += EchoLeftPin_ValueChanged;
            echoRightPin.ValueChanged += EchoRightPin_ValueChanged;

            timer.Interval = TimeSpan.FromMilliseconds(1000); // Every greater than 60 ms per datasheet
            timer.Tick += Timer_Tick;
            timer.Start();

        }

        private void Timer_Tick(object sender, object e)
        {
            // 10Us pulse... 
            triggerPin.Write(GpioPinValue.High);
            triggerPin.Write(GpioPinValue.Low);
        }

        private void EchoRightPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.RisingEdge)
            {
                // Starting a pulse
                rightTime.Start();
            }
            else if (args.Edge == GpioPinEdge.FallingEdge)
            {
                RightDistance = DistanceFromTime(rightTime);
                rightTime.Reset();
            }
        }

        private void EchoLeftPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.RisingEdge)
            {
                // Starting a pulse
                leftTime.Start();
            }
            else if (args.Edge == GpioPinEdge.FallingEdge)
            {
                LeftDistance = DistanceFromTime(leftTime);
                leftTime.Reset();
            }
        }

        private void EchoBackPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.RisingEdge)
            {
                // Starting a pulse
                backTime.Start();
            }
            else if (args.Edge == GpioPinEdge.FallingEdge)
            {
                BackDistance = DistanceFromTime(backTime);

                backTime.Reset();
            }
        }

        private void EchoFrontPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.RisingEdge)
            {
                // Starting a pulse
                frontTime.Start();
            }
            else if (args.Edge == GpioPinEdge.FallingEdge)
            {
                FrontDistance = DistanceFromTime(frontTime);

                frontTime.Reset();
            }
        }

        private double DistanceFromTime(Stopwatch time)
        {
            // Formula: uS / 58 == centimeters
            double microsecondsPerTick = (1000.0 * 1000.0) / (double)Stopwatch.Frequency;
            double deltaInMicroseconds = (double)time.ElapsedTicks * microsecondsPerTick;
            double distance = deltaInMicroseconds / 58.0;

            Debug.WriteLine($"D: {distance}, in {deltaInMicroseconds}, with elapsed ticks {time.ElapsedTicks}");
            return distance;
        }
    }
}
