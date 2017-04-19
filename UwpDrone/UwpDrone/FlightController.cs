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
using Windows.Networking;
using Windows.Networking.Connectivity;

namespace UwpDrone
{
    class FlightController
    {
        struct SupportedPort
        {
            public string port;
            public uint baud;
        }

        UwpMavLink mavLink;

        SerialDevice _device;
        DataWriter writer = null;
        DataReader reader = null;

        bool isArmed = false;

        public SonarInTheRound Sonar
        {
            get;
        } = new SonarInTheRound();


        public FlightController()
        {
        }

        public async Task<bool> initialize()
        {
            await Sonar.initialize();
            await ConnectToController();


            if (writer != null && reader != null)
            {
                mavLink = new UwpMavLink();
                mavLink.connectToMavLink(writer, reader);

                foreach (HostName localHostName in NetworkInformation.GetHostNames())
                {
                    if (localHostName.IPInformation != null)
                    {
                        if (localHostName.Type == HostNameType.Ipv4)
                        {
                            mavLink.proxy(localHostName.ToString(), "10.0.0.165", 14550);

                            break;
                        }
                    }
                }

                return true;
            }

            return false;
        }

        internal async Task ConnectToController()
        {
            SupportedPort[] supportedPorts = { new SupportedPort { port = "VID_26AC", baud = 115200 } , new SupportedPort { port = "UART0", baud = 460800 } };
            string selector = SerialDevice.GetDeviceSelector();
            var deviceCollection = await DeviceInformation.FindAllAsync(selector);

            if (deviceCollection.Count == 0)
                return;

            for (int i = 0; i < deviceCollection.Count; ++i)
            {
                foreach (var supportedPort in supportedPorts)
                {
                    if (deviceCollection[i].Name.Contains(supportedPort.port) || deviceCollection[i].Id.Contains(supportedPort.port))
                    {
                        _device = await SerialDevice.FromIdAsync(deviceCollection[i].Id);
                        if (_device != null)
                        {
                            _device.BaudRate = supportedPort.baud;
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
        }

        public void Arm()
        {
            if (mavLink != null)
            {
                mavLink.arm();
                isArmed = true;
            }
        }

        public void Disarm()
        {
            if (mavLink != null)
            {
                mavLink.disarm();
            }
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
            if (mavLink != null)
            {
                mavLink.FlyToHeight(heightInCM);
            }
        }
        
        public void takeoff()
        {
            mavLink.takeoff(0.5f);
        }

        public void land()
        {
            if (mavLink != null)
            {
                mavLink.land();
            }
        }
    }
}
