using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using AwareGroup.IoTDroneDisplay.MissionControl.Model;

namespace AwareGroup.IoTDroneDisplay.MissionControl.Services
{
    internal class DeviceWatcherService : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<DeviceInstance> DeviceInstances = new ObservableCollection<DeviceInstance>();
        public Dictionary<string, DeviceInstance> DeviceInstanceMap = new Dictionary<string, DeviceInstance>();

        DeviceWatcher _watcher = null;

        public DeviceWatcherService()
        {

        }

        public object objLock = new object();
        private bool _isRunning = false;

        public List<DeviceEndpoint> GetDeviceEndpoints()
        {
            try
            {
                var newEndpoints = DeviceInstances.Select(o =>
                                                            new DeviceEndpoint()
                                                            {
                                                                IsEnabled = true,
                                                                Name = o.Name,
                                                                IpAddress = o.IpAddress                                                               
                                                            }
                ).ToList();
                return newEndpoints;
            }
            catch
            {
            }
            return new List<DeviceEndpoint>();
        }

        public bool IsRunning
        {
            get
            {
                bool running = false;
                lock (objLock)
                {
                    running = _isRunning;
                }
                return running;
            }
        }

        public void Start()
        {
            if (IsRunning)
            {
                //already running - exit
                return;
            }

            lock (objLock)
            {
                _isRunning = false;
            }
            string queryString = "System.Devices.AepService.ProtocolId:={4526e8c1-8aac-4153-9b16-55e86ada0e54}" +
                                 "AND System.Devices.Dnssd.Domain:=\"local\"" +
                                 "AND System.Devices.Dnssd.ServiceName:=\"_sshsvc._tcp\"";

            _watcher = DeviceInformation.CreateWatcher(
                queryString,
                new string[]
                {
                    "System.Devices.Dnssd.HostName",
                    "System.Devices.Dnssd.ServiceName",
                    "System.Devices.IpAddress"
                },
                DeviceInformationKind.AssociationEndpointService);

            _watcher.Removed += async (DeviceWatcher deviceSender, DeviceInformationUpdate args) =>
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () =>
                    {
                        if (DeviceInstanceMap.ContainsKey(args.Id) == true)
                        {
                            DeviceInstances.Remove(DeviceInstanceMap[args.Id]);
                            DeviceInstanceMap.Remove(args.Id);
                        }

                        NotifyChange("iotCoreDevices");
                    });
            };

            _watcher.Updated += async (DeviceWatcher deviceSender, DeviceInformationUpdate args) =>
            {
                object value;
                string name = String.Empty;
                string ip = String.Empty;

                if (args.Properties.TryGetValue("System.Devices.Dnssd.HostName", out value))
                {
                    string local = ".local";

                    name = value as String;

                    // remove .local from the name
                    int index = name.IndexOf(local);
                    if (index > 0)
                    {
                        name = name.Remove(index, local.Length);
                    }
                }

                var newIp = IpFromDictionary(args.Properties);
                if (newIp != String.Empty)
                {
                    ip = newIp;
                }

                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () =>
                    {
                        DeviceInstance instance;
                        if (DeviceInstanceMap.TryGetValue(args.Id, out instance))
                        {
                            if (name != String.Empty)
                            {
                                instance.Name = name;
                            }

                            if (ip != String.Empty)
                            {
                                instance.IpAddress = ip;
                            }
                        }
                    });
            };

            _watcher.Added += async (DeviceWatcher deviceSender, DeviceInformation args) =>
            {
                string name = args.Name;
                string ip = IpFromDictionary(args.Properties);
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () =>
                    {
                        if (DeviceInstanceMap.ContainsKey(args.Id) == false)
                        {
                            var device = new DeviceInstance(name, ip);
                            DeviceInstanceMap.Add(args.Id, device);
                            DeviceInstances.Add(device);
                        }

                        NotifyChange("iotCoreDevices");
                    });
            };

            _watcher.Start();
            lock (objLock)
            {
                _isRunning = true;
            }
        }

        private string IpFromDictionary(IReadOnlyDictionary<String, object> dictionary)
        {
            object value;
            if (dictionary.TryGetValue("System.Devices.IpAddress", out value))
            {
                var ips = value as string[];
                if (ips != null && ips.Length > 0)
                {
                    return ips[0];
                }
            }

            return String.Empty;
        }

        public void Stop()
        {

            try
            {
                if (IsRunning)
                    if (_watcher != null)
                    {
                        _watcher.Stop();
                    }
            }
            catch 
            {
            }
            lock (objLock)
            {
                _isRunning = false;
            }
            DeviceInstances.Clear();
        }

        private void NotifyChange(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }
    }
}
