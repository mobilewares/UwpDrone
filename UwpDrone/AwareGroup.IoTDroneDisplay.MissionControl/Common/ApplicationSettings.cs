using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace AwareGroup.IoTDroneDisplay.MissionControl.Common
{
    public static class ApplicationSettings
    {

        static readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        static readonly StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;

        public static void SetSetting(string key, string value)
        {
            try
            {
                if (LocalSettings.Values.Keys.Contains(key))
                    LocalSettings.Values[key] = value;
                else
                {
                    LocalSettings.Values.Add(new KeyValuePair<string, object>(key, value));
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        public static string GetSetting(string key, string defaultValue)
        {
            try
            {
                if (LocalSettings.Values.Keys.Contains(key))
                    return (LocalSettings.Values[key].ToString() ?? defaultValue);

            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            return defaultValue;
        }

        public async static Task SetFileSetting(string fileName, string value)
        {
            try
            {
                StorageFile outputFile = await LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(outputFile, value);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        public async static Task<string> GetFileSetting(string fileName, string defaultValue)
        {
            try
            {
                StorageFile inputFile = await LocalFolder.GetFileAsync(fileName);
                String data = await FileIO.ReadTextAsync(inputFile);
                return data;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            return defaultValue;
        }

        public static async Task<T> GetTypedFileSetting<T>(string fileName)
        {
            try
            {
                string jsonData = await GetFileSetting(fileName, "") ?? "";
                if (jsonData != "")
                {
                    T item = JsonConvert.DeserializeObject<T>(jsonData);
                    return item;
                }

            }
            catch (Exception ee)
            {
                string message = ee.Message;
            }
            return default(T);
        }

        public static async Task<bool> SetTypedFileSetting<T>(string fileName, T item)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(item) ?? "";
                if (jsonData != "")
                {
                    await SetFileSetting(fileName, jsonData);
                    return true;
                }
            }
            catch (Exception ee)
            {
                string message = ee.Message;
            }
            return false;
        }

    }

    public abstract class ApplicationSettingsBase
    {
        private string SettingsKey { get; set; } = "Unknown";
        private string DefaultValue { get; set; } = "";

        protected void Initialize(string settingsKey, string defaultValue)
        {
            SettingsKey = settingsKey;
            DefaultValue = defaultValue;
        }


        protected string GetStringValue()
        {
            try
            {

                string tmp = ApplicationSettings.GetSetting(SettingsKey, DefaultValue);

                if (string.IsNullOrEmpty(tmp))
                    return DefaultValue;
                else
                {
                    return tmp;
                }
            }
            catch (Exception)
            {

            }
            return DefaultValue;
        }


        protected void SetStringValue(string value)
        {
            try
            {
                ApplicationSettings.SetSetting(SettingsKey, value ?? DefaultValue);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

    }

    public class ApplicationSettingsString : ApplicationSettingsBase
    {
        public ApplicationSettingsString(string settingsKey, string defaultValue)
        {
            Initialize(settingsKey, defaultValue);
        }

        public string GetValue()
        {
            return GetStringValue();
        }


        public void SetValue(string value)
        {
            SetStringValue(value);
        }
    }

    public class ApplicationSettingsBool : ApplicationSettingsBase
    {
        public ApplicationSettingsBool(string settingsKey, bool defaultValue)
        {
            Initialize(settingsKey, defaultValue ? "true" : "false");
        }

        public bool GetValue()
        {
            return (GetStringValue() ?? "").ToLower() == "true";
        }


        public void SetValue(bool value)
        {
            SetStringValue(value ? "true" : "false");
        }

    }

    public class ApplicationSettingsDouble : ApplicationSettingsBase
    {
        private string _conversionMask = "#######0.0####";

        public ApplicationSettingsDouble(string settingsKey, double defaultValue)
        {
            Initialize(settingsKey,  defaultValue.ToString(_conversionMask));
        }

        public double GetValue()
        {
            return ConvertStringToDouble(GetStringValue() ?? "",0.0);
        }


        public void SetValue(double value)
        {
            SetStringValue(value.ToString(_conversionMask));
        }



        static double ConvertStringToDouble(string sourceValue, double defaultValue)
        {
            double res = defaultValue;
            string tmp = sourceValue;
            if (string.IsNullOrEmpty(tmp)) return defaultValue;
            try
            {
                if (!double.TryParse(tmp, out res)) return defaultValue;
            }
            catch (Exception ex)
            {
                res = defaultValue;
            }
            return res;
        }

    }
}
