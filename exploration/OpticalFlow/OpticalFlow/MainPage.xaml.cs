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
using Windows.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OpticalFlow
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        OpticalFlowCamera _flow;
        private DispatcherTimer refresh = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1.0 / 10.0) };

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            _flow = new OpticalFlowCamera();
            await _flow.initialize(colorPreviewImage);

            refresh.Tick += Refresh_Tick;
            refresh.Start();
        }

        void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            foreach (var point in _flow.interestPoints)
            {
                args.DrawingSession.DrawCircle(new Vector2((float)point.X, (float)point.Y), 1.0f, Colors.Green);
            }
        }

        private void Refresh_Tick(object sender, object e)
        {
            InterestPointDisplay.Invalidate();
        }
    }
}
