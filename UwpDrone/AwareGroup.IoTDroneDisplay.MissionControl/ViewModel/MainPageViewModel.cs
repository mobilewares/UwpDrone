using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AwareGroup.IoTDroneDisplay.MissionControl.Common;
using AwareGroup.IoTDroneDisplay.MissionControl.Controls;
using AwareGroup.IoTDroneDisplay.MissionControl.Model;
using AwareGroup.IoTDroneDisplay.MissionControl.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

namespace AwareGroup.IoTDroneDisplay.MissionControl.ViewModel
{
    public class MainPageViewModel :ViewModelBase
        {
        public const int maxLogItems = 50;
        public ObservableCollection<MissionLogItem> LogItems { get; private set; } = new ObservableCollection<MissionLogItem>();
        private object _logItemsLock = new object();


            private RelayCommand _selectDeviceCommand;

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
                                       dc.NanoServerInstances.Add(new NanoServerInstance("PROJECT ARYA", "127.0.0.1"));
                                       dc.NanoServerInstances.Add(new NanoServerInstance("PROJECT DAENERYS", "127.0.0.2"));
                                       dc.NanoServerInstances.Add(new NanoServerInstance("PROJECT CERSEI", "127.0.0.3"));
                                       dc.SelectedInstance = dc.NanoServerInstances[0];
                                       deviceSettings.DataContext = dc;

                                       //Show Dialog
                                       var res = await deviceSettings.ShowAsync();
                                       var n = res.ToString();
                                   });
                               }));
                }
            }

        private RelayCommand _showSettingsCommand;
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
                                   AppSettings appSettings = new AppSettings();
                                   var res = await appSettings.ShowAsync();

                               });
                               
                           }));
            }
        }


        private RelayCommand _mission1Command;
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

        private RelayCommand _mission2Command;
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

        private RelayCommand _mission3Command;
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


            private void UpdateSelected()
        {
            RaisePropertyChanged("Mission1Selected");
            RaisePropertyChanged("Mission2Selected");
            RaisePropertyChanged("Mission3Selected");
        }


        private List<DroneMission> _missions = new List<DroneMission>();
        private DroneMission _currentMission = null;
        private string _title = "PROJECT ARYA";
        private bool _showMissionControls=true;

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


        public void ClearLogItems()
        {
            lock (_logItemsLock)
            {
                LogItems.Clear();
            }
        }

        public void AddLogItem(DateTime timestamp, string logText, string logStatus)
        {
            var item = new MissionLogItem() {LogText = logText, LogStatus = logStatus, TimeStamp = timestamp};
            lock (_logItemsLock)
            {
                if (LogItems.Count > 0)
                {
                    while (LogItems.Count>=maxLogItems)
                        LogItems.RemoveAt(0);
                    LogItems.Insert(0,item);
                }
                else
                    LogItems.Add(item);
            }
        }

        public MainPageViewModel()
        {
            var svc = new DroneMissionService();
            _missions = svc.LoadDroneMissionsFromResource(DroneMissionService.DroneMissionsPath);
            CurrentMission = _missions.FirstOrDefault();
            RaisePropertyChanged("Mission1");
            RaisePropertyChanged("Mission2");
            RaisePropertyChanged("Mission3");
            UpdateSelected();
        }
    }
}
