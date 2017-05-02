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
            this.InitializeComponent();
            InitializeDefaults();
            DroneTitlePanel.PowerButtonClicked += DroneTitlePanelOnPowerButtonClicked;
        }

        private void DroneTitlePanelOnPowerButtonClicked(object sender, EventArgs eventArgs)
        {
            //Raise Event to the Parent Host.
            if (PowerButtonClicked!=null)
                PowerButtonClicked.Invoke(this, new EventArgs());
        }

        public void InitializeDefaults()
        {
            Roll = 0.0;
            Heading = 0.0;
            FeetX = 0.0;
            FeetY = 0.0;
            Title = "PROJECT ARYA";
            Status = "";
            Objective = "Find the Red Ball";
            BatteryLevel = 50.0;
        }

        public static readonly DependencyProperty RollProperty = DependencyProperty.Register(
            "Roll", typeof(double), typeof(DroneOverlayControl), new PropertyMetadata(0, RollPropertyChangedCallback));

        public double Roll
        {
            get { return (double) GetValue(RollProperty); }
            set { SetValue(RollProperty, value); }
        }

        private static void RollPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (DroneOverlayControl) dependencyObject;
                double val = (double) dependencyPropertyChangedEventArgs.NewValue;
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
            get { return (double) GetValue(HeadingProperty); }
            set { SetValue(HeadingProperty, value); }
        }

        private static void HeadingPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (DroneOverlayControl) dependencyObject;
                double val = (double) dependencyPropertyChangedEventArgs.NewValue;
                ctl.DroneRoomMap.Heading = val;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public static readonly DependencyProperty FeetXProperty = DependencyProperty.Register(
            "FeetX", typeof(double), typeof(DroneOverlayControl), new PropertyMetadata(0, FeetXPropertyChangedCallback));

        public double FeetX
        {
            get { return (double) GetValue(FeetXProperty); }
            set { SetValue(FeetXProperty, value); }
        }

        private static void FeetXPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (DroneOverlayControl) dependencyObject;
                double val = (double) dependencyPropertyChangedEventArgs.NewValue;
                ctl.DroneRoomMap.X = val;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public static readonly DependencyProperty FeetYProperty = DependencyProperty.Register(
            "FeetY", typeof(double), typeof(DroneOverlayControl), new PropertyMetadata(0, FeetYPropertyChangedCallback));

        public double FeetY
        {
            get { return (double) GetValue(FeetYProperty); }
            set { SetValue(FeetYProperty, value); }
        }

        private static void FeetYPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (DroneOverlayControl) dependencyObject;
                double val = (double) dependencyPropertyChangedEventArgs.NewValue;
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
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        private static void TitlePropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (DroneOverlayControl) dependencyObject;
                string val = (string) dependencyPropertyChangedEventArgs.NewValue;
                ctl.DroneTitlePanel.Title = val??"";
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
            get { return (string) GetValue(ObjectiveProperty); }
            set { SetValue(ObjectiveProperty, value); }
        }

        private static void ObjectivePropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (DroneOverlayControl) dependencyObject;
                string val = (string) dependencyPropertyChangedEventArgs.NewValue;
                ctl.DroneMissionObjective.Text = val??"";
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
            get { return (string) GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        private static void StatusPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (DroneOverlayControl) dependencyObject;
                string val = (string) dependencyPropertyChangedEventArgs.NewValue;
                ctl.DroneMissionStatus.Text = val??"";
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
            get { return (double) GetValue(BatteryLevelProperty); }
            set { SetValue(BatteryLevelProperty, value); }
        }

        private static void BatteryLevelPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (DroneOverlayControl) dependencyObject;
                double val = (double) dependencyPropertyChangedEventArgs.NewValue;
                ctl.DroneTitlePanel.BatteryLevel = val;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }






        //public static readonly DependencyProperty RollProperty = DependencyProperty.Register(
        //    "Roll", typeof(double), typeof(DroneOverlayControl), new PropertyMetadata(default(double)));

        //public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register(
        //    "Heading", typeof(double), typeof(DroneOverlayControl), new PropertyMetadata(default(double)));

        //public static readonly DependencyProperty FeetXProperty = DependencyProperty.Register(
        //    "FeetX", typeof(double), typeof(DroneOverlayControl), new PropertyMetadata(default(double)));

        //public static readonly DependencyProperty FeetYProperty = DependencyProperty.Register(
        //    "FeetY", typeof(double), typeof(DroneOverlayControl), new PropertyMetadata(default(double)));

        //public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        //    "Title", typeof(string), typeof(DroneOverlayControl), new PropertyMetadata(default(string)));

        //public static readonly DependencyProperty ObjectiveProperty = DependencyProperty.Register(
        //    "Objective", typeof(string), typeof(DroneOverlayControl), new PropertyMetadata(default(string)));

        //public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(
        //    "Status", typeof(string), typeof(DroneOverlayControl), new PropertyMetadata(default(string)));

        //public static readonly DependencyProperty BatteryLevelProperty = DependencyProperty.Register(
        //    "BatteryLevel", typeof(double), typeof(DroneOverlayControl), new PropertyMetadata(default(double)));

        //public double BatteryLevel
        //{
        //    get { return (double) GetValue(BatteryLevelProperty); }
        //    set { SetValue(BatteryLevelProperty, value); }
        //}

        //public string Status
        //{
        //    get { return (string) GetValue(StatusProperty); }
        //    set { SetValue(StatusProperty, value); }
        //}

        //public string Objective
        //{
        //    get { return (string) GetValue(ObjectiveProperty); }
        //    set { SetValue(ObjectiveProperty, value); }
        //}

        //public string Title
        //{
        //    get { return (string) GetValue(TitleProperty); }
        //    set { SetValue(TitleProperty, value); }
        //}

        //public double FeetY
        //{
        //    get { return (double) GetValue(FeetYProperty); }
        //    set { SetValue(FeetYProperty, value); }
        //}

        //public double FeetX
        //{
        //    get { return (double) GetValue(FeetXProperty); }
        //    set { SetValue(FeetXProperty, value); }
        //}

        //public double Heading
        //{
        //    get { return (double) GetValue(HeadingProperty); }
        //    set { SetValue(HeadingProperty, value); }
        //}

        //public double Roll
        //{
        //    get { return (double) GetValue(RollProperty); }
        //    set { SetValue(RollProperty, value); }
        //}


    }
}
