//using OpenGlove_API_C_Sharp_HL.ServiceReference1;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using WebSocketSharp;

namespace OpenGlove_API_C_Sharp_HL
{
    public class DataReceiver
    {
        public string WebSocketPort;
        public WebSocket WebSocketClient;
        public bool WebSocketActive = false;
        public Task webSocketTask;

        public DataReceiver(string WebSocketPort)
        {
            this.WebSocketPort = WebSocketPort;
            WebSocketClient = new WebSocket("ws://localhost:" + WebSocketPort);
            try
            {
                webSocketTask = Task.Run(() =>
                {
                    ReadData();
                });
            }
            catch
            {
                Console.WriteLine("Error");
            }
        }

       
        public event EventHandler<DataReceiverEventArgs> FingerMovement;
        public event EventHandler<DataReceiverEventArgs> AccelerometerValues;
        public event EventHandler<DataReceiverEventArgs> GyroscopeValues;
        public event EventHandler<DataReceiverEventArgs> MagnometerValues;
        public event EventHandler<DataReceiverEventArgs> AllIMUValues;

        protected virtual void OnFingerMovement(int region, int value)
        {
            FingerMovement?.Invoke(this, new DataReceiverEventArgs()
            { Region = region, Value = value });
        }
        protected virtual void OnAccelerometerValues(float ax, float ay, float az)
        {
            AccelerometerValues?.Invoke(this, new DataReceiverEventArgs()
            { Ax = ax, Ay = ay, Az = az });
        }
        protected virtual void OnGyroscopeValues(float gx, float gy, float gz)
        {
            GyroscopeValues?.Invoke(this, new DataReceiverEventArgs()
            { Gx = gx, Gy = gy, Gz = gx });
        }
        protected virtual void OnMagnometerValues(float mx, float my, float mz)
        {
            MagnometerValues?.Invoke(this, new DataReceiverEventArgs() { Mx = mx, My = my, Mz = mz });
        }
        protected virtual void OnAllIMUValues(float ax, float ay, float az, float gx, float gy, float gz, float mx, float my, float mz)
        {
            AllIMUValues?.Invoke(this, new DataReceiverEventArgs()
            { Ax = ax, Ay = ay, Az = az, Gx = gx, Gy = gy, Gz = gz, Mx = mx, My = my, Mz = mz });
        }


        public void ReadData()
        {
            using (WebSocketClient)
            {
                WebSocketClient.OnOpen += (object sender, EventArgs e) => Debug.WriteLine("WebSocketClient: Socket Open!");
                WebSocketClient.OnClose += (object sender, CloseEventArgs e) => Debug.WriteLine("WebSocketClient: Socket Close!");

                WebSocketClient.OnMessage += (sender, e) => {
                    HandleMessage(e);
                };

                WebSocketClient.Connect();
                WebSocketActive = true;
                //while (WebSocketActive == true) { }
            }
        }

        public void HandleMessage(MessageEventArgs e)
        {
            int mapping, value;
            float valueX, valueY, valueZ;
            string[] words;

            if (e.Data != null)
            {
                words = e.Data.Split(',');
                Debug.WriteLine($"WebSocketClient: {e.Data}");
                try
                {
                    switch (words[0])
                    {
                        case "f":
                            mapping = Int32.Parse(words[1]);
                            value = Int32.Parse(words[2]);
                            OnFingerMovement(mapping, value);
                            break;
                        case "a":
                            valueX = float.Parse(words[1], CultureInfo.InvariantCulture);
                            valueY = float.Parse(words[2], CultureInfo.InvariantCulture);
                            valueZ = float.Parse(words[3], CultureInfo.InvariantCulture);
                            OnAccelerometerValues(valueX, valueY, valueZ);
                            break;
                        case "g":
                            valueX = float.Parse(words[1], CultureInfo.InvariantCulture);
                            valueY = float.Parse(words[2], CultureInfo.InvariantCulture);
                            valueZ = float.Parse(words[3], CultureInfo.InvariantCulture);
                            OnGyroscopeValues(valueX, valueY, valueZ);
                            break;
                        case "m":
                            valueX = float.Parse(words[1], CultureInfo.InvariantCulture);
                            valueY = float.Parse(words[2], CultureInfo.InvariantCulture);
                            valueZ = float.Parse(words[3], CultureInfo.InvariantCulture);
                            OnMagnometerValues(valueX, valueY, valueZ);
                            break;
                        case "z":
                            OnAllIMUValues(float.Parse(words[1], CultureInfo.InvariantCulture), float.Parse(words[2], CultureInfo.InvariantCulture), float.Parse(words[3], CultureInfo.InvariantCulture), float.Parse(words[4], CultureInfo.InvariantCulture), float.Parse(words[5], CultureInfo.InvariantCulture), float.Parse(words[6], CultureInfo.InvariantCulture), float.Parse(words[7], CultureInfo.InvariantCulture), float.Parse(words[8], CultureInfo.InvariantCulture), float.Parse(words[9], CultureInfo.InvariantCulture));
                            break;
                        default:
                            break;
                    }

                }
                catch
                {
                    Console.WriteLine("ERROR: BAD FORMAT");
                }
            }
        }
    }
}
