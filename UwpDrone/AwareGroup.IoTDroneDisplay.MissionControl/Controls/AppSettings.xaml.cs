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
using AwareGroup.IoTDroneDisplay.MissionControl.Model;
using AwareGroup.IoTDroneDisplay.MissionControl.Services;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AwareGroup.IoTDroneDisplay.MissionControl.Controls
{
    public sealed partial class AppSettings : ContentDialog
    {
        public AppSettings()
        {
            this.InitializeComponent();
        }

        public void PopulateCurrentSettings(AppSettingsService currentSettings)
        {
            switch (currentSettings.ConnectionMode)
            {
                case ClientMode.HttpDirect:
                    rdHttp.IsChecked = true;
                    break;
                case ClientMode.Broadcast:
                    rdBroadcast.IsChecked = true;
                    break;
                case ClientMode.None:
                    rdNone.IsChecked = true;
                    break;

            }

            txtHttpTitle.Text = currentSettings.TitleHttp??"PROJECT ARYA";
            txtHttpAddress.Text = currentSettings.IpAddressHttp??"192.168.1.9";
            txtHttpPort.Text = currentSettings.PortHttp??"3000";

            txtBroadcastPort.Text = currentSettings.PortBroadcast??"3000";

            txtTitleNone.Text = currentSettings.TitleNone ?? "PROJECT ARYA";

            chkHideMissionControls.IsChecked = currentSettings.HideMissionButtons;
            chkLaunchIoTRemote.IsChecked = currentSettings.LaunchRemoteDesktop;
            chkLaunchProjector.IsChecked = currentSettings.ProjectOutput;

        }

        private bool RadioButtonIsChecked(RadioButton rb)
        {
            if (rb.IsChecked.HasValue)
                return rb.IsChecked.Value;
            return false;
        }

        private bool CheckBoxIsChecked(CheckBox cb)
        {
            if (cb.IsChecked.HasValue)
                return cb.IsChecked.Value;
            return false;
        }

        public void SaveCurrentSettings(AppSettingsService currentSettings)
        {
            if (RadioButtonIsChecked(rdBroadcast))
            {
                currentSettings.ConnectionMode= ClientMode.Broadcast;
            }
            else if (RadioButtonIsChecked(rdHttp))
            {
                currentSettings.ConnectionMode = ClientMode.HttpDirect;
            }
            else
            {
                currentSettings.ConnectionMode = ClientMode.None;
            }

            currentSettings.TitleHttp  = txtHttpTitle.Text ?? "PROJECT ARYA";
            currentSettings.IpAddressHttp  = txtHttpAddress.Text ?? "192.168.1.9";
            currentSettings.PortHttp = txtHttpPort.Text ?? "3000";

            currentSettings.PortBroadcast = txtBroadcastPort.Text ?? "3000";

            currentSettings.TitleNone  = txtTitleNone.Text ?? "PROJECT ARYA";

            currentSettings.HideMissionButtons = CheckBoxIsChecked(chkHideMissionControls);
            currentSettings.LaunchRemoteDesktop = CheckBoxIsChecked(chkLaunchIoTRemote);
            currentSettings.ProjectOutput = CheckBoxIsChecked(chkLaunchProjector);

        }


        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
