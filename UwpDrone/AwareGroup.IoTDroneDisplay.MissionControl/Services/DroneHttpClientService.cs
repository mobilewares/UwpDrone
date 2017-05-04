using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AwareGroup.IoTDroneDisplay.MissionControl.Services
{
    public class DroneHttpClientService
    {
        private const string StartMissionCommand = "mission";
        private const string StartMissionCommandParameter = "mission";
        private const string StopMissionCommand = "stop";

        private async Task<bool> SendCommandToBot(string ipAddress, string port, string command, string paramName = "", string value = "")
        {
            if (command != null)
            {
                string url = "http://" + ipAddress + ":" + port + "/bot?cmd=" + command;
                if (!string.IsNullOrEmpty(paramName) && !string.IsNullOrEmpty(value))
                {
                    url = url + "&" + paramName + "=" + value;
                }

                HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                webRequest.Method = "GET";
                var response = await Task<WebResponse>.Factory.FromAsync(webRequest.BeginGetResponse, webRequest.EndGetResponse, webRequest);
                var httpWebResponse = response as HttpWebResponse;
                if (httpWebResponse != null)
                {
                    if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> RequestStartMission(string ipAddress, string port, string missionId)
        {
            try
            {
                bool res = await SendCommandToBot(ipAddress, port, StartMissionCommand, StartMissionCommandParameter, missionId);
                return res;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return false;
        }

        public async Task<bool> RequestStopMission(string ipAddress, string port)
        {
            try
            {
                bool res = await SendCommandToBot(ipAddress, port, StopMissionCommand);
                return res;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return false;
        }

     
    }
}
