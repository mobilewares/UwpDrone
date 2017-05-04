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
        private readonly ApplicationSettingsString _connectionMode = new ApplicationSettingsString("CONNECTIONMODE", "HTTP");
        private readonly ApplicationSettingsString _ipAddressHttpSetting = new ApplicationSettingsString("IPADDRESSHTTP", "192.168.1.9");
        private readonly ApplicationSettingsString _titleHttpSetting = new ApplicationSettingsString("TITLEHTTP", "PROJECT ARYA");
        private readonly ApplicationSettingsString _portHttpSetting = new ApplicationSettingsString("PORTHTTP", "3000");
        private readonly ApplicationSettingsString _portBroadcastSetting = new ApplicationSettingsString("PORTBROADCAST", "3000");
        private readonly ApplicationSettingsString _titleNoneSetting = new ApplicationSettingsString("TITLENONE", "PROJECT ARYA");
        private readonly ApplicationSettingsBool _hideMissionbuttons = new ApplicationSettingsBool("HIDEMISSIONBUTTONS", true);
        private readonly ApplicationSettingsBool _projectOutput = new ApplicationSettingsBool("PROJECTOUTPUT", true);
        private readonly ApplicationSettingsBool _launchRemoteDesktop = new ApplicationSettingsBool("LAUNCHREMOTEDESKTOP", true);

        public string IpAddressHttp
        {
            get { return _ipAddressHttpSetting.GetValue(); }
            set
            {
                _ipAddressHttpSetting.SetValue(value??""); 
                NotifyPropertyChanged("IpAddressHttp");
            }
        }

        public string PortHttp
        {
            get { return _portHttpSetting.GetValue(); }
            set
            {
                _portHttpSetting.SetValue(value ?? "");
                NotifyPropertyChanged("PortHttp");
            }
        }

        public string PortBroadcast
        {
            get { return _portBroadcastSetting.GetValue(); }
            set
            {
                _portBroadcastSetting.SetValue(value ?? "");
                NotifyPropertyChanged("PortBroadcast");
            }
        }

        public string TitleHttp
        {
            get { return _titleHttpSetting.GetValue(); }
            set
            {
                _titleHttpSetting.SetValue(value ?? "");
                NotifyPropertyChanged("TitleHttp");
            }
        }

        public string TitleNone
        {
            get { return _titleNoneSetting.GetValue(); }
            set
            {
                _titleNoneSetting.SetValue(value ?? "");
                NotifyPropertyChanged("TitleNone");
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

        public bool HideMissionButtons
        {
            get { return _hideMissionbuttons.GetValue(); }
            set
            {
                _hideMissionbuttons.SetValue(value);
                NotifyPropertyChanged("HideMissionButtons");
            }
        }

        public bool ProjectOutput
        {
            get { return _projectOutput.GetValue(); }
            set
            {
                _projectOutput.SetValue(value);
                NotifyPropertyChanged("ProjectOutput");
            }
        }

        public ClientMode ConnectionMode
        {
            get
            {
                string tmp = _connectionMode.GetValue()??"";
                if (tmp=="UDP")
                    return ClientMode.Broadcast;
                else if (tmp=="HTTP")
                    return ClientMode.HttpDirect;
                else
                    return ClientMode.None;
            }
            set
            {
                switch (value)
                {
                    case ClientMode.Broadcast: _connectionMode.SetValue("UDP");
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
