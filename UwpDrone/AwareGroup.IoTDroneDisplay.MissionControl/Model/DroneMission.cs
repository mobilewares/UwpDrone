using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;

namespace AwareGroup.IoTDroneDisplay.MissionControl.Model
{
    public class DroneMission
    {
        public string Id { get; set; } = "";
        public string PassthroughId { get; set; } = "";
        public string Title { get; set; } = "";
        public string Summary { get; set; } = "";
        public string Description { get; set; } = "";

        [JsonIgnore()]
        public ImageSource MissionImage => new BitmapImage(new Uri(string.Format("ms-appx:///Missions/Images/Mission{0}.png", Id)));

        [JsonIgnore()]
        public ImageSource ButtonImage => new BitmapImage(new Uri(string.Format("ms-appx:///Missions/Images/Icon{0}.png", Id)));

        [JsonIgnore()]
        public ImageSource ButtonReflectionImage => new BitmapImage(new Uri(string.Format("ms-appx:///Missions/Images/IconReflection{0}.png", Id)));


    }
}
