using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwareGroup.IoTDroneDisplay.MissionControl.Model
{
    public class DeviceEndpoint
    {
        public DeviceEndpoint()
        {
        }

        public DeviceEndpoint(string name, string ipAddress)
        {
            IpAddress = ipAddress;
            Name = name;
        }

        public bool IsEnabled { get; set; } = false;
        public string Name { get; set; } = "(Unknown)";
        public string IpAddress { get; set; } = "127.0.0.1";
        public string Port { get; set; } = "3000";
    }
}
