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

        private async Task<bool> SendCommandToBot(string ipAddress, string command, string paramName = "", string value = "")
        {
            if (command != null)
            {
                string url = "http://" + ipAddress + "/bot?cmd=" + command;
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

        public async Task<bool> RequestStartMission(string ipAddress, int missionId)
        {
            try
            {
                bool res = await SendCommandToBot(ipAddress, StartMissionCommand, StartMissionCommandParameter, missionId.ToString());
                return res;
            }
            catch (Exception e)
            {

            }
            return false;
        }

        public async Task<bool> RequestStopMission(string ipAddress)
        {
            try
            {
                bool res = await SendCommandToBot(ipAddress, StopMissionCommand);
                return res;
            }
            catch (Exception e)
            {

            }
            return false;
        }

        //public async Task<bool> HandleArm(LuisResult result, object context)
        //{
        //    LUISIntentStatus usingIntentRouter = (LUISIntentStatus)context;

        //    await usingIntentRouter.service.SpeakAsync("Preparing to Arm");
        //    await Task.Delay(1000);
        //    await usingIntentRouter.service.SpeakAsync("please stand back");

        //    await Task.Delay(3000);

        //    var succeeded = await sendCommandToBot("arm");
        //    if (succeeded)
        //    {
        //        await usingIntentRouter.service.SpeakAsync("I've sucessfully armed");

        //    }
        //    else
        //    {
        //        await usingIntentRouter.service.SpeakAsync("I've failed to arm");
        //    }

        //    usingIntentRouter.handled = true;
        //    usingIntentRouter.Success = true;
        //    return true;
        //}


        //public async Task<bool> HandleDisarm(LuisResult result, object context)
        //{
        //    LUISIntentStatus usingIntentRouter = (LUISIntentStatus)context;

        //    var succeeded = await sendCommandToBot("disarm");
        //    if (succeeded)
        //    {
        //        usingIntentRouter.SpeechRespose = "I've sucessfully disarmed";

        //    }
        //    else
        //    {
        //        usingIntentRouter.SpeechRespose = "Unable to disarm";
        //    }


        //    usingIntentRouter.Success = true;
        //    return true;
        //}

        //[IntentHandler(0.7, Name = "Fly to Height")]
        //public async Task<bool> HandleFlyToHeight(LuisResult result, object context)
        //{
        //    LUISIntentStatus usingIntentRouter = (LUISIntentStatus)context;

        //    var entity = result.Entities.Where(e => e.Key == "height").Select(e => e.Value).FirstOrDefault();
        //    if (entity == null)
        //    {
        //        usingIntentRouter.Success = false;
        //        return false;
        //    }

        //    var height = entity.First().Value;

        //    var succeeded = await sendCommandToBot("flyHeight", "height", height.ToString());
        //    if (succeeded)
        //    {
        //        usingIntentRouter.SpeechRespose = "I've sucessfully disarmed";

        //    }
        //    else
        //    {
        //        usingIntentRouter.SpeechRespose = "Unable to disarm";
        //    }


        //    usingIntentRouter.Success = true;
        //    return true;
        //}

        //[IntentHandler(0.65, Name = "None")]
        //public async Task<bool> HandleNone(LuisResult result, object context)
        //{
        //    LUISIntentStatus usingIntentRouter = (LUISIntentStatus)context;
        //    await Task.Delay(0);
        //    usingIntentRouter.Success = false;
        //    return true;
        //}
    }
}
