using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using AwareGroup.IoTDroneDisplay.MissionControl.Model;
using Newtonsoft.Json;

namespace AwareGroup.IoTDroneDisplay.MissionControl.Services
{
    public class DroneMissionService
    {
        public const string DroneMissionsPath = "AwareGroup.IoTDroneDisplay.MissionControl.Missions.Missions.json";

        public List<DroneMission> LoadDroneMissionsFromResource(string srcPath)
        {

            if (DesignMode.DesignModeEnabled)
            {
                string jsonData =
                    "[ { \"Id\": \"1\", \"Title\": \"MISSION 1\", \"Summary\": \"Find the Red Ball\", \"Description\": \"Mission 1 - Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\" }, { \"Id\": \"2\", \"Title\": \"MISSION 2\", \"Summary\": \"Find the Blue Ball\", \"Description\": \"Mission 2 - Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.\" }, { \"Id\": \"3\", \"Title\": \"MISSION 3\", \"Summary\": \"Find All the Balls\", \"Description\": \"Mission 3 - Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\" } ]";
                return JsonConvert.DeserializeObject<List<DroneMission>>(jsonData);
            }

            List<DroneMission> allSkins = new List<DroneMission>();
            try
            {
                var assembly = this.GetType().GetTypeInfo().Assembly;
                var stream = assembly.GetManifestResourceStream(srcPath);

                JsonSerializer jsonSerializer = new JsonSerializer();
                var serializer = new JsonSerializer();

                using (var sr = new StreamReader(stream))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    var results = serializer.Deserialize<List<DroneMission>>(jsonTextReader);
                    allSkins.AddRange(results);
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;

            }
            return allSkins;
        }
    }
}
