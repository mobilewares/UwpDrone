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
using GalaSoft.MvvmLight.Command;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AwareGroup.IoTDroneDisplay.MissionControl.Controls
{
    public sealed partial class MissionSelectControl : UserControl
    {
        public MissionSelectControl()
        {
            this.InitializeComponent();
            InitializeControl();
        }

        public void InitializeControl()
        {
            Title = "MISSION";
            IconImage = null;
            IconReflectionImage =  null;
            VisualStateManager.GoToState(this, "MissionDeselected", false);
            MissionButton.Click+= delegate(object sender, RoutedEventArgs args)
            {
                try
                {
                    if (SelectMissionCommand != null)
                    {
                        SelectMissionCommand.Execute(null);
                    }
                }
                catch (Exception e)
                {
                }
            };
        }

        public static readonly DependencyProperty MissionIdProperty = DependencyProperty.Register(
            "MissionId", typeof(string), typeof(MissionSelectControl), new PropertyMetadata(""));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(MissionSelectControl), new PropertyMetadata("TITLE", TitlePropertyChangedCallback));

        public static readonly DependencyProperty IconImageProperty = DependencyProperty.Register(
            "IconImage", typeof(ImageSource), typeof(MissionSelectControl), new PropertyMetadata(default(ImageSource), IconImagePropertyChangedCallback));

        public static readonly DependencyProperty IconReflectionImageProperty = DependencyProperty.Register(
            "IconReflectionImage", typeof(ImageSource), typeof(MissionSelectControl), new PropertyMetadata(default(ImageSource), IconReflectionImagePropertyChangedCallback));

        public static readonly DependencyProperty SelectMissionCommandProperty = DependencyProperty.Register(
            "SelectMissionCommand", typeof(RelayCommand), typeof(MissionSelectControl), new PropertyMetadata(default(RelayCommand)));


        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register(
            "Selected", typeof(bool), typeof(MissionSelectControl), new PropertyMetadata(false, SelectedPropertyChangedCallback));

        public bool Selected
        {
            get { return (bool) GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }

        private static void SelectedPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (MissionSelectControl) dependencyObject;
                bool val = (bool) dependencyPropertyChangedEventArgs.NewValue;
                VisualStateManager.GoToState(ctl, val ? "MissionSelected" : "MissionDeselected", true);
                //ctl.XX = val;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }


        public string MissionId
        {
            get { return (string)GetValue(MissionIdProperty); }
            set { SetValue(MissionIdProperty, value); }
        }

        public RelayCommand SelectMissionCommand
        {
            get { return (RelayCommand) GetValue(SelectMissionCommandProperty); }
            set { SetValue(SelectMissionCommandProperty, value); }
        }   
        
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
                var ctl = (MissionSelectControl) dependencyObject;
                string val = (string) dependencyPropertyChangedEventArgs.NewValue;
                ctl.MissionButton.Content = val??"";
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }


        public ImageSource IconReflectionImage
        {
            get { return (ImageSource) GetValue(IconReflectionImageProperty); }
            set { SetValue(IconReflectionImageProperty, value); }
        }

        private static void IconReflectionImagePropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (MissionSelectControl) dependencyObject;
                ImageSource val = (ImageSource) dependencyPropertyChangedEventArgs.NewValue;
                if (val != null)
                {
                    ctl.MissionReflection.Source = val;
                }
                //ctl.XX = val;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public string IconImage
        {
            get { return (string) GetValue(IconImageProperty); }
            set { SetValue(IconImageProperty, value); }
        }

        private static void IconImagePropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (MissionSelectControl) dependencyObject;
                ImageSource val = (ImageSource) dependencyPropertyChangedEventArgs.NewValue;
                if (val!=null)
                    ctl.MissionIcon.Source = val;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

    }
}
