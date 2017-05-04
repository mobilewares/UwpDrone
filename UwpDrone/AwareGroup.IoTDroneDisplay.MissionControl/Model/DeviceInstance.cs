using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace AwareGroup.IoTDroneDisplay.MissionControl.Model
{
    public class DeviceInstance : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        public DeviceInstance(string name, string ip)
        {
            this.name = name;
            this.ipAddress = ip;
        }

        private string name = String.Empty;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                notifyChange("Name");
            }
        }

        private string ipAddress = String.Empty;
        public string IpAddress
        {
            get
            {
                return ipAddress;
            }
            set
            {
                ipAddress = value;
                notifyChange("IpAddress");
            }
        }

        private PasswordCredential credential;
        public PasswordCredential Credential
        {
            get { return this.credential; }
            set { this.credential = value; }
        }

        public override string ToString()
        {
            if (this.Name.Length > 0 && this.IpAddress.Length > 0)
            {
                return this.Name + " - " + this.IpAddress;
            }
            else if (this.Name.Length > 0)
            {
                return this.Name;
            }
            else
            {
                return this.IpAddress;
            }
        }

        private void notifyChange(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }
    }

}
