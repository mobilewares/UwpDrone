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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using AwareGroup.IoTDroneDisplay.DroneControls.Common;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AwareGroup.IoTDroneDisplay.DroneControls.Controls
{
    public sealed partial class TitlePanelControl : UserControl
    {
        public event EventHandler ButtonClicked;

        private const string _batteryImagePath = "ms-appx:///AwareGroup.IoTDroneDisplay.DroneControls/Assets/Battery-{0}.png";

        public TitlePanelControl()
        {
            InitializeComponent();
            FixValues();
            PowerButton.Click += delegate (object sender, RoutedEventArgs args)
             {
                 if (ButtonClicked != null)
                     ButtonClicked.Invoke(this, null);
             };
        }

        public void FixValues()
        {
            try
            {
                BatteryLevel = 100.0;
                Title = "SCANNING";
            }
            catch
            {
            }
        }

        /// <summary>
        /// Battery Level of the Drone (0..100)
        /// </summary>
        public static readonly DependencyProperty BatteryLevelProperty = DependencyProperty.Register(
            "BatteryLevel", typeof(double), typeof(TitlePanelControl), new PropertyMetadata(100, BatteryLevelPropertyChangedCallback));


        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(TitlePanelControl), new PropertyMetadata("SCANNING", TitlePropertyChangedCallback));

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        private static void TitlePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (TitlePanelControl)dependencyObject;
                string val = (string)dependencyPropertyChangedEventArgs.NewValue;
                ctl.TitleText.Text = val ?? "";
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        /// <summary>
        /// Battery Level of the Drone (0..100)
        /// </summary>
        public double BatteryLevel
        {
            get { return (double)GetValue(BatteryLevelProperty); }
            set { SetValue(BatteryLevelProperty, value); }
        }

        private static void BatteryLevelPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (TitlePanelControl)dependencyObject;
                double val = (double)dependencyPropertyChangedEventArgs.NewValue;
                val = val.Clamp(0, 100.0);
                if (val>90.0)
                    ctl.BatteryImage.Source = new BitmapImage(new Uri(String.Format(_batteryImagePath, "100"), UriKind.RelativeOrAbsolute));
                else if (val > 70.0)
                    ctl.BatteryImage.Source = new BitmapImage(new Uri(String.Format(_batteryImagePath, "80"), UriKind.RelativeOrAbsolute));
                else if (val > 50.0)
                    ctl.BatteryImage.Source = new BitmapImage(new Uri(String.Format(_batteryImagePath, "60"), UriKind.RelativeOrAbsolute));
                else if (val > 30.0)
                    ctl.BatteryImage.Source = new BitmapImage(new Uri(String.Format(_batteryImagePath, "40"), UriKind.RelativeOrAbsolute));
                else if (val > 10.0)
                    ctl.BatteryImage.Source = new BitmapImage(new Uri(String.Format(_batteryImagePath, "20"), UriKind.RelativeOrAbsolute));
                else 
                    ctl.BatteryImage.Source = new BitmapImage(new Uri(String.Format(_batteryImagePath, "00"), UriKind.RelativeOrAbsolute));
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

    }
}
