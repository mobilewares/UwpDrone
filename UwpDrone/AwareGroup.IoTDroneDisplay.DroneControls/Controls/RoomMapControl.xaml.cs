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
using AwareGroup.IoTDroneDisplay.DroneControls.Common;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AwareGroup.IoTDroneDisplay.DroneControls.Controls
{
    public sealed partial class RoomMapControl : UserControl
    {
        public RoomMapControl()
        {
            this.InitializeComponent();
            FixValues();
        }

        const double _widthPixels = 200.0;
        const double _heightPixels = 300.0;
        const double _widthFeet = 8.0;
        const double _heightFeet = 12.0;

        const double _pixelsPerFeetX = _widthPixels / _widthFeet;
        const double _pixelsPerFeetY = _heightPixels / _heightFeet;

        const double _pixelsPerMeterX = _pixelsPerFeetX / 0.3048;
        const double _pixelsPerMeterY = _pixelsPerFeetY / 0.3048;

        public void FixValues()
        {
            Heading = 0.0;
            X = 0.0;
            Y = 0.0;
        }

        /// <summary>
        /// Rotation Angle (Heading) of the Drone (Clamps to +/- 360.0)
        /// </summary>
        public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register(
            "Heading", typeof(double), typeof(RoomMapControl), new PropertyMetadata(0, HeadingPropertyChangedCallback));

        /// <summary>
        /// Rotation Angle of the Gimbal Crosshair (Clamps to +/- 180.0)
        /// </summary>
        public double Heading
        {
            get { return (double)GetValue(HeadingProperty); }
            set { SetValue(HeadingProperty, value); }
        }

        private static void HeadingPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (RoomMapControl)dependencyObject;
                double val = (double)dependencyPropertyChangedEventArgs.NewValue;
                val = val.Clamp(-360.0, 360.0);
                ctl.RotateDrone.Angle = val;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public static readonly DependencyProperty XProperty = DependencyProperty.Register(
           "X", typeof(double), typeof(RoomMapControl), new PropertyMetadata(0, XPropertyChangedCallback));

        /// <summary>
        /// Horizontal/X Position (in Feet) of Drone (0..8)
        /// </summary>
        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        private static void XPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (RoomMapControl)dependencyObject;
                double val = (double)dependencyPropertyChangedEventArgs.NewValue;
                val = (_pixelsPerMeterX * val).Clamp(0, 200.0);
                ctl.TranslateDrone.X = val;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public static readonly DependencyProperty YProperty = DependencyProperty.Register(
           "Y", typeof(double), typeof(RoomMapControl), new PropertyMetadata(0, YPropertyChangedCallback));

        /// <summary>
        /// Vertical/Y Position (in Feet) of Drone (0..8)
        /// </summary>
        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        private static void YPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (RoomMapControl)dependencyObject;
                double val = (double)dependencyPropertyChangedEventArgs.NewValue;
                val = (val * _pixelsPerMeterY).Clamp(0, 300.0);
                ctl.TranslateDrone.Y = val;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

    }
}
