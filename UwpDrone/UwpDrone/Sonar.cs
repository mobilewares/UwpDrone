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
        // Trigger Pins
        const int kTriggerFront = 16;
        const int kTriggerBack = 15;
        const int kTriggerLeft = 14;
        const int kTriggerRight = 5;

        // Echo Pins
        const int kEchoFront = 6;
        const int kEchoBack = 4;
        const int kEchoLeft = 7;
        const int kEchoRight = 17;

        GpioPin triggerFrontPin;
        GpioPin triggerBackPin;
        GpioPin triggerLeftPin;
        GpioPin triggerRightPin;
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

        long triggerCount = 0;

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

            triggerFrontPin = controller.OpenPin(kTriggerFront);
            triggerBackPin = controller.OpenPin(kTriggerBack);
            triggerLeftPin = controller.OpenPin(kTriggerLeft);
            triggerRightPin = controller.OpenPin(kTriggerRight);
            echoFrontPin = controller.OpenPin(kEchoFront);
            echoBackPin = controller.OpenPin(kEchoBack);
            echoLeftPin = controller.OpenPin(kEchoLeft);
            echoRightPin = controller.OpenPin(kEchoRight);

            triggerFrontPin.SetDriveMode(GpioPinDriveMode.Output);
            triggerBackPin.SetDriveMode(GpioPinDriveMode.Output);
            triggerLeftPin.SetDriveMode(GpioPinDriveMode.Output);
            triggerRightPin.SetDriveMode(GpioPinDriveMode.Output);
            echoFrontPin.SetDriveMode(GpioPinDriveMode.Input);
            echoBackPin.SetDriveMode(GpioPinDriveMode.Input);
            echoLeftPin.SetDriveMode(GpioPinDriveMode.Input);
            echoRightPin.SetDriveMode(GpioPinDriveMode.Input);

            long debounceTicks = Stopwatch.Frequency / (1000 * 1000 * 50);
            echoFrontPin.DebounceTimeout = TimeSpan.FromTicks(debounceTicks);
            echoBackPin.DebounceTimeout = TimeSpan.FromTicks(debounceTicks);
            echoLeftPin.DebounceTimeout = TimeSpan.FromTicks(debounceTicks);
            echoRightPin.DebounceTimeout = TimeSpan.FromTicks(debounceTicks);

            echoFrontPin.ValueChanged += EchoFrontPin_ValueChanged;
            echoBackPin.ValueChanged += EchoBackPin_ValueChanged;
            echoLeftPin.ValueChanged += EchoLeftPin_ValueChanged;
            echoRightPin.ValueChanged += EchoRightPin_ValueChanged;

            timer.Interval = TimeSpan.FromMilliseconds(100); // Every greater than 60 ms per datasheet
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            // 10Us pulse... 
            if (triggerCount == 0)
            {
                triggerFrontPin.Write(GpioPinValue.High);
                triggerFrontPin.Write(GpioPinValue.Low);
            }
            else if (triggerCount == 1)
            {
                triggerBackPin.Write(GpioPinValue.High);
                triggerBackPin.Write(GpioPinValue.Low);
            }
            else if (triggerCount == 2)
            {
                triggerLeftPin.Write(GpioPinValue.High);
                triggerLeftPin.Write(GpioPinValue.Low);
            }
            else if (triggerCount == 3)
            {
                triggerRightPin.Write(GpioPinValue.High);
                triggerRightPin.Write(GpioPinValue.Low);
            }

            
            if (triggerCount++ >= 4)
            {
                triggerCount = 0;
            }
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
            return distance;
        }
    }
}
