using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using AwareGroup.IoTDroneDisplay.MissionControl.Model;

namespace AwareGroup.IoTDroneDisplay.MissionControl.ViewModel
{
    public class SelectDeviceViewModel
    {
        public ObservableCollection<NanoServerInstance> NanoServerInstances { get; set; } = new ObservableCollection<NanoServerInstance>();
        public NanoServerInstance SelectedInstance { get; set; } = null;
        public SelectDeviceViewModel()
        {
            if (DesignMode.DesignModeEnabled)
            {
                NanoServerInstances.Add(new NanoServerInstance("PROJECT ARYA", "127.0.0.1"));
                NanoServerInstances.Add(new NanoServerInstance("PROJECT DAENERYS", "127.0.0.2"));
                NanoServerInstances.Add(new NanoServerInstance("PROJECT CERSEI", "127.0.0.3"));

                SelectedInstance = NanoServerInstances[0];

            }
        }
    }
}
