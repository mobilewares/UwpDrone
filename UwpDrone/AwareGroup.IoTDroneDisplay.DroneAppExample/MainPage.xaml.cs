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
using AwareGroup.IoTDroneDisplay.DroneAppExample.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AwareGroup.IoTDroneDisplay.DroneAppExample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public DroneControlViewModel VM { get; set; }
        private DispatcherTimer tmr = null;

        public MainPage()
        {
            this.InitializeComponent();

            //Assign View Model to Data Context
            VM = new DroneControlViewModel();
            this.DataContext = VM;


            //Handle Power Button
            DroneOverlay.PowerButtonClicked += delegate(object sender, EventArgs args)
            {
                //Handle the Power Button Here..
            };


            //Fake Timer to pretend stuff is happening..
            //this.Loaded += delegate(object sender, RoutedEventArgs args)
            //{
            //    tmr = new DispatcherTimer();
            //    tmr.Interval = TimeSpan.FromMilliseconds(500.0);
            //    tmr.Tick += TmrOnTick;
            //    tmr.Start();
            //};

            //this.Unloaded += delegate(object sender, RoutedEventArgs args)
            //{
            //    if (tmr!=null)
            //        if (tmr.IsEnabled)
            //            tmr.Stop();
            //    tmr = null;
            //};
            

        }

        //private void TmrOnTick(object sender, object o)
        //{
            
        //}
    }
}
