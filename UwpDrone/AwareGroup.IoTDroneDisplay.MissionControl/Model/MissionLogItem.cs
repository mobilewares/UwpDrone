using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwareGroup.IoTDroneDisplay.MissionControl.Model
{
    public class MissionLogItem
    {
        public DateTime TimeStamp { get; set; }
        public string LogText { get; set; }
        public string LogStatus { get; set; }
        public string DisplayTimeStamp => TimeStamp.ToString("ddd HH:mm:ss");
    }
}
