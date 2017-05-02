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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AwareGroup.IoTDroneDisplay.DroneControls.Controls
{
    public sealed partial class MissionStatusControl : UserControl
    {
        public MissionStatusControl()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(MissionStatusControl), new PropertyMetadata("", TextPropertyChangedCallback));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void TextPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (MissionStatusControl)dependencyObject;
                string val = (string)dependencyPropertyChangedEventArgs.NewValue;
                ctl.TitleText.Text = val ?? "";
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

    }
}
