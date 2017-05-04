using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AwareGroup.IoTDroneDisplay.MissionControl.Common;
using AwareGroup.IoTDroneDisplay.MissionControl.Model;

namespace AwareGroup.IoTDroneDisplay.MissionControl.Services
{
    public class AppSettingsService : Bindable
    {
        private readonly ApplicationSettingsString _ipAddressSetting = new ApplicationSettingsString("IPADDRESS", "");
        private readonly ApplicationSettingsBool _showMissionbuttons = new ApplicationSettingsBool("SHOWMISSIONBUTTONS", true);
        private readonly ApplicationSettingsBool _projectOutput = new ApplicationSettingsBool("PROJECTOUTPUT", true);
        private readonly ApplicationSettingsBool _launchRemoteDesktop = new ApplicationSettingsBool("LAUNCHREMOTEDESKTOP", true);
        private readonly ApplicationSettingsString _connectionMode = new ApplicationSettingsString("CONNECTIONMODE", "HTTP");

        public string IpAddress
        {
            get { return _ipAddressSetting.GetValue(); }
            set
            {
                _ipAddressSetting.SetValue(value??""); 
                NotifyPropertyChanged("IpAddress");
            }
        }

        public bool LaunchRemoteDesktop
        {
            get { return _launchRemoteDesktop.GetValue(); }
            set
            {
                _launchRemoteDesktop.SetValue(value);
                NotifyPropertyChanged("LaunchRemoteDesktop");
            }
        }

        public bool ShowMissionButtons
        {
            get { return _showMissionbuttons.GetValue(); }
            set
            {
                _showMissionbuttons.SetValue(value);
                NotifyPropertyChanged("ShowMissionButtons");
            }
        }

        public ClientMode ConnectionMode
        {
            get
            {
                string tmp = _connectionMode.GetValue()??"";
                if (tmp=="UDP")
                    return ClientMode.UDPServer;
                else if (tmp=="HTTP")
                    return ClientMode.HttpDirect;
                else
                    return ClientMode.None;
            }
            set
            {
                switch (value)
                {
                    case ClientMode.UDPServer: _connectionMode.SetValue("UDP");
                        break;
                    case ClientMode.HttpDirect:
                        _connectionMode.SetValue("HTTP");
                        break;
                    case ClientMode.None:
                        _connectionMode.SetValue("NONE");
                        break;
                }
                NotifyPropertyChanged("ConnectionMode");
            }
        }


    }
}
