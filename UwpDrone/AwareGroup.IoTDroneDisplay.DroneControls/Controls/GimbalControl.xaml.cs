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
    public sealed partial class GimbalControl : UserControl
    {
        public GimbalControl()
        {
            this.InitializeComponent();
            FixValues();
        }

        public void FixValues()
        {
            Roll = 0.0;
        }

        /// <summary>
        /// Rotation Roll of the Gimbal Crosshair (Clamps to +/- 180.0)
        /// </summary>
        public static readonly DependencyProperty RollProperty = DependencyProperty.Register(
            "Roll", typeof(double), typeof(GimbalControl), new PropertyMetadata(0, RollPropertyChangedCallback));

        /// <summary>
        /// Rotation Roll of the Gimbal Crosshair (Clamps to +/- 180.0)
        /// </summary>
        public double Roll
        {
            get { return (double) GetValue(RollProperty); }
            set { SetValue(RollProperty, value); }
        }

        private static void RollPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (GimbalControl)dependencyObject;
                double val = (double) dependencyPropertyChangedEventArgs.NewValue;
                val = val.Clamp(-180.0, 180.0);
                ctl.rtCrossHair.Angle = val;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }



    }
}
