using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using MavLinkUwp;
using Windows.Devices.SerialCommunication;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace UwpDrone
{
    class FlightController
    {
        UwpMavLink mavLink;

        SerialDevice _device;
        DataWriter writer = null;
        DataReader reader = null;

        bool isArmed = false;


        public FlightController()
        {
        }

        public async Task initialize()
        {
            await ConnectToController();

            mavLink = new UwpMavLink();
            mavLink.connectToMavLink(writer, reader);
            mavLink.proxy("10.0.1.5", "10.0.1.3", 14550);
        }

        internal async Task ConnectToController()
        {
            string identifyingSubStr = "VID_26AC";
            string selector = SerialDevice.GetDeviceSelector();
            var deviceCollection = await DeviceInformation.FindAllAsync(selector);

            if (deviceCollection.Count == 0)
                return;

            for (int i = 0; i < deviceCollection.Count; ++i)
            {
                if (deviceCollection[i].Name.Contains(identifyingSubStr) || deviceCollection[i].Id.Contains(identifyingSubStr))
                {
                    _device = await SerialDevice.FromIdAsync(deviceCollection[i].Id);
                    if (_device != null)
                    {
                        _device.BaudRate = 460800;
                        _device.Parity = SerialParity.None;
                        _device.DataBits = 8;
                        _device.StopBits = SerialStopBitCount.One;
                        _device.Handshake = SerialHandshake.None;
                        _device.ReadTimeout = TimeSpan.FromSeconds(5);
                        _device.WriteTimeout = TimeSpan.FromSeconds(5);
                        //_device.IsRequestToSendEnabled = false;
                        //_device.IsDataTerminalReadyEnabled = false;


                        writer = new DataWriter(_device.OutputStream);
                        reader = new DataReader(_device.InputStream);
                        //reader.InputStreamOptions = InputStreamOptions.Partial;

                        return;
                    }
                }
            }
        }

        public void Arm()
        {
            mavLink.arm();
            isArmed = true;
        }

        public void Disarm()
        {
            mavLink.disarm();
            isArmed = false;
        }

        public void ToggleArm()
        {
            if (isArmed)
            {
                Disarm();
            }
            else
            {
                Arm();
            }
        }

        public void flyToHeight(float heightInCM)
        {
            mavLink.FlyToHeight(heightInCM);
        }
        
        public void takeoff()
        {
            mavLink.takeoff(2);
        }

        public void land()
        {
            mavLink.land();
        }
    }
}
