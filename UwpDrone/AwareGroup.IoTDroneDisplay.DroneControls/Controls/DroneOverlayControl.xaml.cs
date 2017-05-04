using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AwareGroup.IoTDroneDisplay.DroneControls.Model;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AwareGroup.IoTDroneDisplay.DroneControls.Controls
{
    public sealed partial class DroneOverlayControl : UserControl
    {

        public event EventHandler PowerButtonClicked;

        public DroneOverlayControl()
        {
            InitializeComponent();
            InitializeDefaults();
            DroneTitlePanel.ButtonClicked += DroneTitlePanelOnPowerButtonClicked;
        }

        private void DroneTitlePanelOnPowerButtonClicked(object sender, EventArgs eventArgs)
        {
            //Raise Event to the Parent Host.
            if (PowerButtonClicked != null)
                PowerButtonClicked.Invoke(this, null);
        }

        public void InitializeDefaults()
        {
            try
            {
                Roll = 0.0;
                Heading = 0.0;
                X = 0.0;
                Y = 0.0;
                Title = "PROJECT ARYA";
                Status = "";
                Objective = "Find the Red Ball";
                BatteryLevel = 50.0;
                ShowGimbal = true;
            }
            catch
            {
            }
        }

        public static readonly DependencyProperty RollProperty = DependencyProperty.Register(
            "Roll", typeof(double), typeof(DroneOverlayControl), new PropertyMetadata(0, RollPropertyChangedCallback));

        public double Roll
        {
            get { return (double)GetValue(RollProperty); }
            set { SetValue(RollProperty, value); }
        }

        private static void RollPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (DroneOverlayControl)dependencyObject;
                double val = (double)dependencyPropertyChangedEventArgs.NewValue;
                ctl.DroneGimbalControl.Roll = val;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register(
            "Heading", typeof(double), typeof(DroneOverlayControl), new PropertyMetadata(0, HeadingPropertyChangedCallback));

        public double Heading
        {
            get { return (double)GetValue(HeadingProperty); }
            set { SetValue(HeadingProperty, value); }
        }

        private static void HeadingPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (DroneOverlayControl)dependencyObject;
                double val = (double)dependencyPropertyChangedEventArgs.NewValue;
                ctl.DroneRoomMap.Heading = val;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }


        public static readonly DependencyProperty AltitudeProperty = DependencyProperty.Register(
            "Altitude", typeof(double), typeof(DroneOverlayControl), new PropertyMetadata(0, AltitudePropertyChangedCallback));

        public double Altitude
        {
            get { return (double)GetValue(AltitudeProperty); }
            set { SetValue(AltitudeProperty, value); }
        }

        private static void AltitudePropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (DroneOverlayControl)dependencyObject;
                double val = (double)dependencyPropertyChangedEventArgs.NewValue;
                //ctl.XX = val;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public static readonly DependencyProperty PitchProperty = DependencyProperty.Register(
            "Pitch", typeof(double), typeof(DroneOverlayControl), new PropertyMetadata(0, PitchPropertyChangedCallback));

        public double Pitch
        {
            get { return (double)GetValue(PitchProperty); }
            set { SetValue(PitchProperty, value); }
        }

        private static void PitchPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (DroneOverlayControl)dependencyObject;
                double val = (double)dependencyPropertyChangedEventArgs.NewValue;
                //ctl.XX = val;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public static readonly DependencyProperty XProperty = DependencyProperty.Register(
            "X", typeof(double), typeof(DroneOverlayControl), new PropertyMetadata(0, FeetXPropertyChangedCallback));

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        private static void FeetXPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (DroneOverlayControl)dependencyObject;
                double val = (double)dependencyPropertyChangedEventArgs.NewValue;
                ctl.DroneRoomMap.X = val;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public static readonly DependencyProperty YProperty = DependencyProperty.Register(
            "Y", typeof(double), typeof(DroneOverlayControl), new PropertyMetadata(0, FeetYPropertyChangedCallback));

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        private static void FeetYPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (DroneOverlayControl)dependencyObject;
                double val = (double)dependencyPropertyChangedEventArgs.NewValue;
                ctl.DroneRoomMap.Y = val;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(DroneOverlayControl), new PropertyMetadata("DRONE TITLE", TitlePropertyChangedCallback));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        private static void TitlePropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (DroneOverlayControl)dependencyObject;
                string val = (string)dependencyPropertyChangedEventArgs.NewValue;
                ctl.DroneTitlePanel.Title = val ?? "";
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public static readonly DependencyProperty ObjectiveProperty = DependencyProperty.Register(
            "Objective", typeof(string), typeof(DroneOverlayControl), new PropertyMetadata("Mission Objective", ObjectivePropertyChangedCallback));

        public string Objective
        {
            get { return (string)GetValue(ObjectiveProperty); }
            set { SetValue(ObjectiveProperty, value); }
        }

        private static void ObjectivePropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (DroneOverlayControl)dependencyObject;
                string val = (string)dependencyPropertyChangedEventArgs.NewValue;
                ctl.DroneMissionObjective.Text = val ?? "";
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(
            "Status", typeof(string), typeof(DroneOverlayControl), new PropertyMetadata("", StatusPropertyChangedCallback));

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        private static void StatusPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (DroneOverlayControl)dependencyObject;
                string val = (string)dependencyPropertyChangedEventArgs.NewValue;
                ctl.DroneMissionStatus.Text = val ?? "";
                ctl.DroneMissionStatus.Visibility = ((val ?? "") == "") ? Visibility.Collapsed : Visibility.Visible;

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public static readonly DependencyProperty BatteryLevelProperty = DependencyProperty.Register(
            "BatteryLevel", typeof(double), typeof(DroneOverlayControl), new PropertyMetadata(50, BatteryLevelPropertyChangedCallback));

        public double BatteryLevel
        {
            get { return (double)GetValue(BatteryLevelProperty); }
            set { SetValue(BatteryLevelProperty, value); }
        }

        private static void BatteryLevelPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (DroneOverlayControl)dependencyObject;
                double val = (double)dependencyPropertyChangedEventArgs.NewValue;
                ctl.DroneTitlePanel.BatteryLevel = val;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public static readonly DependencyProperty ShowGimbalProperty = DependencyProperty.Register(
            "ShowGimbal", typeof(bool), typeof(DroneOverlayControl), new PropertyMetadata(true, ShowGimbalPropertyChangedCallback));

        public bool ShowGimbal
        {
            get { return (bool) GetValue(ShowGimbalProperty); }
            set { SetValue(ShowGimbalProperty, value); }
        }

        private static void ShowGimbalPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (DroneOverlayControl) dependencyObject;
                bool val = (bool) dependencyPropertyChangedEventArgs.NewValue;
                ctl.DroneGimbalControl.Visibility = val ? Visibility.Visible  : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }   
        


    }
}
