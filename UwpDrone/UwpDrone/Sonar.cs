using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using System.Diagnostics;
using Windows.UI.Xaml;
using MathNet.Filtering;

namespace UwpDrone
{
    public class SonarInTheRound
    {
        // Trigger Pins
        const int kTriggerFront = 5;
        const int kTriggerBack = 15;
        const int kTriggerLeft = 14;
        const int kTriggerRight = 16;

        // Echo Pins
        const int kEchoFront = 17;
        const int kEchoBack = 4;
        const int kEchoLeft = 7;
        const int kEchoRight = 6;

        GpioPin triggerFrontPin;
        GpioPin triggerBackPin;
        GpioPin triggerLeftPin;
        GpioPin triggerRightPin;
        GpioPin echoFrontPin;
        GpioPin echoBackPin;
        GpioPin echoLeftPin;
        GpioPin echoRightPin;


        public double FrontDistance = 0.0;
        public double BackDistance = 0.0;
        public double LeftDistance = 0.0;
        public double RightDistance = 0.0;

        OnlineFilter frontFilter;
        OnlineFilter backFilter;
        OnlineFilter leftFilter;
        OnlineFilter rightFilter;


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

            frontFilter = OnlineFilter.CreateDenoise();
            backFilter = OnlineFilter.CreateDenoise();
            rightFilter = OnlineFilter.CreateDenoise();
            leftFilter = OnlineFilter.CreateDenoise();


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

            // Every greater than 60 ms per datasheet due to echo.
            // Round robin, so servicing each sonar every 120ms, but 
            // allowing for echo by servicing opposite sides.
            timer.Interval = TimeSpan.FromMilliseconds(30);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            // 10Us pulse... 
            // If back collides with left (or front with right), then add an intermediate state.
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
                DistanceFromTime(rightTime, rightFilter, out RightDistance);
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
                DistanceFromTime(leftTime, leftFilter, out LeftDistance);
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
                DistanceFromTime(backTime, backFilter, out BackDistance);

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
                DistanceFromTime(frontTime, frontFilter, out FrontDistance);
                frontTime.Reset();
            }
        }

        private void DistanceFromTime(Stopwatch time, OnlineFilter filter, out double distance)
        {
            // Formula: uS / 58 == centimeters
            double microsecondsPerTick = (1000.0 * 1000.0) / (double)Stopwatch.Frequency;
            double deltaInMicroseconds = (double)time.ElapsedTicks * microsecondsPerTick;
            double measuredDistance = deltaInMicroseconds / 58.0;

            distance = filter.ProcessSample(measuredDistance);
        }
    }
}
