using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using AwareGroup.IoTDroneDisplay.MissionControl.Common;
using AwareGroup.IoTDroneDisplay.MissionControl.Model;
using GalaSoft.MvvmLight.Command;

namespace AwareGroup.IoTDroneDisplay.MissionControl.ViewModel
{
    public class SelectDeviceViewModel : Bindable
    {
        public event EventHandler RequestUpdateEndpoints;

        private bool _showNoResultsMessage = true;
        public ObservableCollection<DeviceEndpoint> Endpoints { get; set; } = new ObservableCollection<DeviceEndpoint>();
        public DeviceEndpoint SelectedEndpoint { get; set; } = null;

        public bool ShowNoResultsMessage
        {
            get { return _showNoResultsMessage; }
            set
            {
                _showNoResultsMessage = value; 
                NotifyPropertyChanged("ShowNoResultsMessage");
            }
        }

        private RelayCommand _refreshCommand;
        public RelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand
                       ?? (_refreshCommand = new RelayCommand(
                           () =>
                           {
                               if (RequestUpdateEndpoints!=null)
                                   RequestUpdateEndpoints.Invoke(this, null);
                           }));
            }
        }


        public void AddEndpoint(DeviceEndpoint endpoint)
        {
            Endpoints.Add(endpoint);
            ShowNoResultsMessage = false;
        }

        public void ClearEndpoints()
        {
            SelectedEndpoint = null;
            Endpoints.Clear();
            ShowNoResultsMessage = true;
        }

        public SelectDeviceViewModel()
        {

            if (DesignMode.DesignModeEnabled)
            {
                AddEndpoint(new DeviceEndpoint("PROJECT ARYA", "127.0.0.1"));
                AddEndpoint(new DeviceEndpoint("PROJECT DAENERYS", "127.0.0.2"));
                AddEndpoint(new DeviceEndpoint("PROJECT CERSEI", "127.0.0.3"));
                SelectedEndpoint = Endpoints[0];
            }
        }

        
    }
}
