using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using AwareGroup.IoTDroneDisplay.MissionControl.Common;
using AwareGroup.IoTDroneDisplay.MissionControl.Controls;
using AwareGroup.IoTDroneDisplay.MissionControl.Model;
using AwareGroup.IoTDroneDisplay.MissionControl.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

namespace AwareGroup.IoTDroneDisplay.MissionControl.ViewModel
{
    public class MainPageViewModel : ViewModelBase
    {
        private const bool SimulateBroadcastService = true;

        private AppSettingsService _settings = new AppSettingsService();
        private DeviceWatcherService _deviceWatcher = new DeviceWatcherService();
        private DroneMissionService _droneMissionService = new DroneMissionService();
        private DroneHttpClientService _droneHttpClientService = new DroneHttpClientService();

        private List<DroneMission> _missions = new List<DroneMission>();
        private DroneMission _currentMission = null;
        private DeviceEndpoint _currentEndpoint = new DeviceEndpoint();

        private ClientMode _lastClientMode = ClientMode.None;

        private string _title = "PROJECT ARYA";
        private bool _showMissionControls = true;

        private RelayCommand _selectDeviceCommand;
        private RelayCommand _showSettingsCommand;
        private RelayCommand _mission1Command;
        private RelayCommand _mission2Command;
        private RelayCommand _mission3Command;
        private RelayCommand _startMissionCommand;
        private RelayCommand _stopMissionCommand;

    
        public RelayCommand StopMissionCommand
        {
            get
            {
                return _stopMissionCommand
                       ?? (_stopMissionCommand = new RelayCommand(
                           async () =>
                           {
                               if (_currentEndpoint.IsEnabled)
                               {
                                   try
                                   {
                                       bool success = await _droneHttpClientService.RequestStopMission(_currentEndpoint.IpAddress, _currentEndpoint.Port);
                                       if (!success)
                                           ShowMessage("Command Failed", "We were unable to send Stop Mission command to the vehicle. Please check you have valid a endpoint set.");
                                   }
                                   catch (Exception e)
                                   {
                                       ShowMessage("Stop Mission Command Exception", "We were unable to send Stop Mission command to the vehicle.\n\nError Message Received :\n\n" + e.Message);
                                   }
                               }
                           }));
            }
        }

        public RelayCommand StartMissionCommand
        {
            get
            {
                return _startMissionCommand
                       ?? (_startMissionCommand = new RelayCommand(
                           async () =>
                           {
                               if (_currentEndpoint.IsEnabled)
                                   if (_currentMission!=null)
                                   {
                                       try
                                       {
                                           bool success = await _droneHttpClientService.RequestStartMission(_currentEndpoint.IpAddress, _currentEndpoint.Port, _currentMission.PassthroughId);
                                           if (!success)
                                               ShowMessage("Start Mission Command Failed", "We were unable to send Start Mission command to the vehicle. Please check you have valid a endpoint set.");
                                       }
                                       catch (Exception e)
                                       {
                                           ShowMessage("Start Mission Command Exception", "We were unable to send Start Mission command to the vehicle.\n\nError Message Received :\n\n" + e.Message );
                                       }
                                   }
                           }));
            }
        }


        public RelayCommand SelectDeviceCommand
        {
            get
            {
                return _selectDeviceCommand
                       ?? (_selectDeviceCommand = new RelayCommand(
                           () =>
                           {
                               DispatcherHelper.CheckBeginInvokeOnUI(async () =>
                               {
                                   SelectDevice deviceSettings = new SelectDevice();

                                    //Test Code...
                                    var dc = new SelectDeviceViewModel();

                                   if (SimulateBroadcastService)
                                   {
                                       dc.AddEndpoint(new DeviceEndpoint("PROJECT ARYA", "192.168.1.9"));
                                       dc.AddEndpoint(new DeviceEndpoint("PROJECT DAENERYS", "127.0.0.2"));
                                       dc.AddEndpoint(new DeviceEndpoint("PROJECT CERSEI", "127.0.0.3"));
                                   }
                                   else
                                   {
                                       if (_deviceWatcher.IsRunning)
                                           foreach (var svc in _deviceWatcher.GetDeviceEndpoints())
                                           {
                                               dc.Endpoints.Add(svc);
                                           }
                                   }

                                   if (_currentEndpoint.IsEnabled)
                                   {
                                       dc.SelectedEndpoint = dc.Endpoints.FirstOrDefault(o => (o.IpAddress == _currentEndpoint.IpAddress));
                                   }
                                   else
                                        dc.SelectedEndpoint = null;

                                   deviceSettings.DataContext = dc;

                                   dc.RequestUpdateEndpoints += delegate(object sender, EventArgs args)
                                   {
                                       DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                       {
                                           dc.ClearEndpoints();
                                           if (_deviceWatcher.IsRunning)
                                               foreach (var svc in _deviceWatcher.GetDeviceEndpoints())
                                               {
                                                   dc.Endpoints.Add(svc);
                                               }
                                       });
                                   };
                                   

                                   //Show Dialog
                                   var res = await deviceSettings.ShowAsync();
                                   if (res == ContentDialogResult.Primary)
                                    {
                                        _currentEndpoint.IsEnabled = false;
                                        if (dc.SelectedEndpoint != null)
                                        {
                                            _currentEndpoint.IpAddress = dc.SelectedEndpoint.IpAddress;
                                            _currentEndpoint.Port = _settings.PortBroadcast;
                                            _currentEndpoint.Name = dc.SelectedEndpoint.Name;
                                            _currentEndpoint.IsEnabled = true;
                                        }
                                       FixEndpoint();
                                       UpdateEndpoint();
                                   }

                               });
                           }));
            }
        }

        private void Dc_RequestUpdateEndpoints(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public RelayCommand ShowSettingsCommand
        {
            get
            {
                return _showSettingsCommand
                       ?? (_showSettingsCommand = new RelayCommand(
                           () =>
                           {
                               DispatcherHelper.CheckBeginInvokeOnUI(async () =>
                               {
                                   ClientMode lastConnectionMode = _settings.ConnectionMode;

                                   AppSettings appSettings = new AppSettings();
                                   appSettings.PopulateCurrentSettings(_settings);
                                   var res = await appSettings.ShowAsync();
                                   if (res == ContentDialogResult.Primary)
                                   {
                                       appSettings.SaveCurrentSettings(_settings);
                                       if ((_settings.ConnectionMode == ClientMode.Broadcast)&&(lastConnectionMode != ClientMode.Broadcast))
                                        {
                                           //If switching to Broadcast + Last mode wasn't broadcast then invalidate the connection.
                                            _currentEndpoint.IsEnabled = false;
                                        }                                           
                                       FixEndpoint();
                                       UpdateEndpoint();
                                   }
                               });

                           }));
            }
        }

        

        public RelayCommand Mission1Command
        {
            get
            {
                return _mission1Command
                       ?? (_mission1Command = new RelayCommand(
                           () =>
                           {
                               if (Mission1 != null)
                                   CurrentMission = Mission1;
                               UpdateSelected();
                           }));
            }
        }

        public RelayCommand Mission2Command
        {
            get
            {
                return _mission2Command
                       ?? (_mission2Command = new RelayCommand(
                           () =>
                           {
                               if (Mission2 != null)
                                   CurrentMission = Mission2;
                               UpdateSelected();
                           }));
            }
        }

        public RelayCommand Mission3Command
        {
            get
            {
                return _mission3Command
                       ?? (_mission3Command = new RelayCommand(
                           () =>
                           {
                               if (Mission3 != null)
                                   CurrentMission = Mission3;
                               UpdateSelected();
                           }));
            }
        }

        public bool ShowMissionControls
        {
            get { return _showMissionControls; }
            set
            {
                _showMissionControls = value;
                RaisePropertyChanged("ShowMissionControls");
            }
        }


        public bool ShowVehicleSelect
        {
            get
            {
                if (_settings!=null)
                    return (_settings.ConnectionMode== ClientMode.Broadcast);
                return false;
            }
        }


        

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged("Title");
            }
        }

        public DroneMission CurrentMission
        {
            get { return _currentMission; }
            set
            {
                _currentMission = value;
                RaisePropertyChanged("CurrentMission");
            }
        }

        public DroneMission Mission1 => (_missions.Count > 0) ? _missions[0] : null;
        public DroneMission Mission2 => (_missions.Count > 1) ? _missions[1] : null;
        public DroneMission Mission3 => (_missions.Count > 2) ? _missions[2] : null;

        public bool Mission1Selected
            => ((_currentMission == null) || (Mission1 == null)) ? false : (_currentMission.Id == Mission1.Id);
        public bool Mission2Selected
            => ((_currentMission == null) || (Mission2 == null)) ? false : (_currentMission.Id == Mission2.Id);
        public bool Mission3Selected
            => ((_currentMission == null) || (Mission3 == null)) ? false : (_currentMission.Id == Mission3.Id);


        #region  "Logging"

        //public const int maxLogItems = 50;
        //public ObservableCollection<MissionLogItem> LogItems { get; private set; } = new ObservableCollection<MissionLogItem>();
        //private object _logItemsLock = new object();

        //public void ClearLogItems()
        //{
        //    lock (_logItemsLock)
        //    {
        //        LogItems.Clear();
        //    }
        //}

        //public void AddLogItem(DateTime timestamp, string logText, string logStatus)
        //{
        //    var item = new MissionLogItem() { LogText = logText, LogStatus = logStatus, TimeStamp = timestamp };
        //    lock (_logItemsLock)
        //    {
        //        if (LogItems.Count > 0)
        //        {
        //            while (LogItems.Count >= maxLogItems)
        //                LogItems.RemoveAt(0);
        //            LogItems.Insert(0, item);
        //        }
        //        else
        //            LogItems.Add(item);
        //    }
        //}


        #endregion


        public MainPageViewModel()
        {
            //Initialize Endpoint from Settings
            FixEndpoint();
            UpdateEndpoint();

            //Load Missions
            _missions = _droneMissionService.LoadDroneMissionsFromResource(DroneMissionService.DroneMissionsPath);
            CurrentMission = _missions.FirstOrDefault();
            UpdateMissions();
        }

        

        private void FixEndpoint()
        {
            switch (_settings.ConnectionMode)
            {
                case ClientMode.Broadcast:
                    _currentEndpoint.Port = _settings.PortBroadcast;
                    if (!_currentEndpoint.IsEnabled)
                    {
                        _currentEndpoint.Name = "(SELECT VEHICLE)";
                        _currentEndpoint.IpAddress = "";                        
                    }
                    if (_lastClientMode != ClientMode.Broadcast)
                        _deviceWatcher.Start();
                    break;
                case ClientMode.HttpDirect:
                    _currentEndpoint.Name = _settings.TitleHttp;
                    _currentEndpoint.IpAddress = _settings.IpAddressHttp;
                    _currentEndpoint.Port = _settings.PortHttp;
                    _currentEndpoint.IsEnabled = true;
                    if (_lastClientMode == ClientMode.Broadcast)
                        _deviceWatcher.Stop();
                    break;
                case ClientMode.None:
                    _currentEndpoint.Name = _settings.TitleNone;
                    _currentEndpoint.IpAddress = "";
                    _currentEndpoint.Port = "";
                    _currentEndpoint.IsEnabled = false;
                    if (_lastClientMode == ClientMode.Broadcast)
                        _deviceWatcher.Stop();
                    break;
            }

            _lastClientMode = _settings.ConnectionMode;
        }

        private void ShowMessage(string title, string message)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                MessageDialog dlg = new MessageDialog("+++ " + (title ?? "") + " +++" + "\n\n" + (message ?? ""));
                dlg.ShowAsync();
            });
        }

        private void UpdateMissions()
        {
            RaisePropertyChanged("Mission1");
            RaisePropertyChanged("Mission2");
            RaisePropertyChanged("Mission3");
            UpdateSelected();
        }

        private void UpdateSelected()
        {
            RaisePropertyChanged("Mission1Selected");
            RaisePropertyChanged("Mission2Selected");
            RaisePropertyChanged("Mission3Selected");
        }


        private void UpdateEndpoint()
        {
            ShowMissionControls = (!_settings.HideMissionButtons) && (_currentEndpoint.IsEnabled);
            Title = _currentEndpoint.Name;
            RaisePropertyChanged("ShowVehicleSelect");
        }
    }
}
