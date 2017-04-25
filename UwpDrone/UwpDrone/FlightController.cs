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

        DispatcherTimer controlLoopTimer = new DispatcherTimer();


        // This flight controller is designed to work in an 8'x8'x12' cage,
        // ~274x274x365
        const double kMaxViableSonarPing = 400.0;   // 400cm is beyond the max we should ever see in the cage.
        const double kMinViableSonarPing = 10.0;   // 10cm is inside the rotors, so bad news if we see this.

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


            if (writer == null || reader == null)
            {
                return false;
            }


            mavLink = new UwpMavLink();
            mavLink.connectToMavLink(writer, reader);

            controlLoopTimer.Interval = TimeSpan.FromMilliseconds(30);
            controlLoopTimer.Tick += ControlLoopTimer_Tick;
            controlLoopTimer.Start();

            return true;

        }

        private void ControlLoopTimer_Tick(object sender, object e)
        {
            synthesizeGPSPositionFromSonar();


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

        public void Proxy(string ipToProxyTo)
        {
            foreach (HostName localHostName in NetworkInformation.GetHostNames())
            {
                if (localHostName.IPInformation != null)
                {
                    if (localHostName.Type == HostNameType.Ipv4)
                    {
                        try
                        {
                            mavLink.proxy(localHostName.ToString(), ipToProxyTo, 14550);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine($"Exception: {e.Message} -\n {e.StackTrace}");
                        }

                        break;
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

        bool SonarPingIsViable(double distance)
        {
            return (distance > kMinViableSonarPing &&
                distance < kMaxViableSonarPing);
        }

        internal void synthesizeGPSPositionFromSonar()
        {
            if (SonarPingIsViable(Sonar.FrontDistance) &&
                SonarPingIsViable(Sonar.BackDistance) &&
                SonarPingIsViable(Sonar.LeftDistance) &&
                SonarPingIsViable(Sonar.RightDistance))
            {
                // Left & back is 'zero'

                double x = Sonar.LeftDistance;
                double y = Sonar.BackDistance;
                double alt = mavLink.getAltitude();
                

                // Might be unstable right as we start, but should stabilize.
                mavLink.setGPS(0, 0, 0);
            }
            else
            {
                // TODO: IF we're flying and some time delta
                // we should land.
            }
        }
    }
}
