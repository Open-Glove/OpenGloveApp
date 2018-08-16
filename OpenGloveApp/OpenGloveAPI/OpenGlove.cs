using System.Collections.Generic;
using OpenGloveApp.Models;
using OpenGloveApp.Utils;
using Xamarin.Forms;

namespace OpenGloveApp.OpenGloveAPI
{
    public class OpenGlove
    {
        public string BluetoothDeviceName { get; set; }
        public OpenGloveConfiguration Configuration { get; set; }
        public LegacyOpenGlove LegacyOpenGlove = new LegacyOpenGlove();
        public bool IsConnected { get; set; }

        public OpenGlove(string bluetoothDeviceName)
            : base ()
        {
            this.BluetoothDeviceName = bluetoothDeviceName;
            this.Configuration = new OpenGloveConfiguration();
        }

        public OpenGlove(string bluetoothDeviceName, OpenGloveConfiguration configuration)
            : base()    
        {
            this.BluetoothDeviceName = bluetoothDeviceName;
            this.Configuration = configuration;
        }

        public void TurnOffActuators()
        {
            List<int> regions = new List<int>();
            List<string> intensities = new List<string>();

            foreach (var entry in Configuration.ActuatorsByRegion)
            {
                regions.Add(entry.Key);
                intensities.Add("0"); // 0 or LOW for turn off
            }

            this.ActivateActuators(regions, intensities); //Turn Off Actuators
        }

        public void TurnOnFlexors()
        {
            InitializeFlexors();
        }

        public void TurnOffFlexors()
        {
            foreach (var entry in Configuration.FlexorsByRegion)
            {
                this.LegacyOpenGlove.removeFlexor(entry.Key);
            }
        }

        public void InitializeActuators()
        {
            if (BooleanStatements.NoNullAndCountGreaterThanZero(this.Configuration.ActuatorsByRegion))
            {
                List<int> positivePins = new List<int>();
                List<int> negativePins = new List<int>();

                foreach (Actuator actuator in this.Configuration.ActuatorsByRegion.Values)
                {
                    positivePins.Add(actuator.PositivePin);
                    negativePins.Add(actuator.NegativePin);
                }

                this.LegacyOpenGlove.InitializeMotor(positivePins);
                this.LegacyOpenGlove.InitializeMotor(negativePins);
            }
        }

        public void InitializeFlexors()
        {
            if(BooleanStatements.NoNullAndCountGreaterThanZero(this.Configuration.FlexorsByRegion))
            {
                foreach (var flexorByRegion in this.Configuration.FlexorsByRegion)
                {
                    this.LegacyOpenGlove.addFlexor(flexorByRegion.Value, flexorByRegion.Key);
                }
            }
        }

        public void AddActuator(int region, int positivePin, int negativePin)
        {
            if(!this.Configuration.ActuatorsByRegion.ContainsKey(region))
            {
                this.Configuration.ActuatorsByRegion.Add(region, new Actuator()
                {
                    PositivePin = positivePin,
                    NegativePin = negativePin
                });
                InitializeActuators();
            }
        }

        public void AddActuators(List<int> regions, List<int> positivePins, List<int> negativePins)
        {
            bool added = false;
            if(BooleanStatements.NoNullAndEqualCount(regions, positivePins, negativePins))
            {
                for (int i = 0; i < regions.Count; i++ )
                {
                    if (!this.Configuration.ActuatorsByRegion.ContainsKey(regions[i]))
                    {
                        this.Configuration.ActuatorsByRegion.Add(regions[i], new Actuator()
                        {
                            PositivePin = positivePins[i],
                            NegativePin = negativePins[i]
                        });
                        added = true;
                    }

                }
                if(added)
                    InitializeActuators();
            }
        }

        public void RemoveActuator(int region)
        {
            if (this.Configuration.ActuatorsByRegion.Remove(region))
                InitializeActuators();
        }

        public void RemoveActuators(List<int> regions)
        {
            bool removed = false;
            foreach(int region in regions)
            {
                if (this.Configuration.ActuatorsByRegion.Remove(region))
                    removed = true;
            }
            if (removed)
                InitializeActuators();
        }

        public void ActivateActuators(List<int> regions, List<string> intensities)
        {
            if(BooleanStatements.NoNullAndEqualCount(regions, intensities))
            {
                List<int> positivePins = new List<int>();

                foreach (int region in regions)
                {
                    positivePins.Add(this.Configuration.ActuatorsByRegion[region].PositivePin);
                }

                this.LegacyOpenGlove.ActivateMotor(positivePins, intensities);
            }
        }

        public void AddFlexor(int region, int pin)
        {
            if (!Configuration.FlexorsByRegion.ContainsKey(region))
            {
                Configuration.FlexorsByRegion.Add(region, pin);
                this.LegacyOpenGlove.addFlexor(pin, region);
            }
        }

        public void AddFlexors(List<int> regions, List<int> pins)
        {
            if (BooleanStatements.NoNullAndEqualCount(regions, pins))
            {
                for (int i = 0; i < regions.Count; i++)
                {
                    if (!Configuration.FlexorsByRegion.ContainsKey(regions[i]))
                    {
                        Configuration.FlexorsByRegion.Add(regions[i], pins[i]);
                        this.LegacyOpenGlove.addFlexor(pins[i], regions[i]);   
                    }
                }
            }
        }

        public void RemoveFlexor(int region)
        {
            if (Configuration.FlexorsByRegion.Remove(region))
                this.LegacyOpenGlove.removeFlexor(region);
        }

        public void RemoveFlexors(List<int> regions)
        {
            foreach (int region in regions)
            {
                if (Configuration.FlexorsByRegion.Remove(region))
                    this.LegacyOpenGlove.removeFlexor(region);
            }
        }

        public void CalibrateFlexors()
        {
            this.LegacyOpenGlove.calibrateFlexors();   
        }

        public void ConfirmCalibration()
        {
            this.LegacyOpenGlove.confirmCalibration();
        }

        public void SetThreshold(int value)
        {
            this.LegacyOpenGlove.setThreshold(value);
        }

        public void ResetFlexors()
        {
            Configuration.FlexorsByRegion.Clear();
            this.LegacyOpenGlove.resetFlexors();
        }

        public void ResetActuators()
        {
            List<int> regions = new List<int>();
            List<string> intensities = new List<string>();

            foreach(var entry in Configuration.ActuatorsByRegion)
            {
                regions.Add(entry.Key);
                intensities.Add("0"); // 0 or LOW for turn off
            }

            this.ActivateActuators(regions, intensities); //Turn Off Actuators

            Configuration.ActuatorsByRegion.Clear();      //Clear mapping
        }

        public void SetLoopDelay(int value)
        {
            this.LegacyOpenGlove.setLoopDelay(value);
        }

        public void StartIMU()
        {
            this.LegacyOpenGlove.startIMU();
        }

        public void SetIMUStatus(bool status)
        {
            int integerStatus = (status) ? 1 : 0;
            this.LegacyOpenGlove.setIMUStatus(integerStatus);
        }

        public void SetRawData(bool status)
        {
            int integerStatus = (status) ? 1 : 0;
            this.LegacyOpenGlove.setRawData(integerStatus);
        }

        public List<BluetoothDevices> GetAllPairedDevices()
        {
            // TODO this need await a async method
            return this.LegacyOpenGlove.communication.GetAllPairedDevices();
        }

        public void OpenDeviceConnection()
        {
            this.LegacyOpenGlove.communication.OpenDeviceConnection(this.BluetoothDeviceName);
            //TODO this need await  bool OpenDeviceConnection()
            this.IsConnected = true;
        }

        public void CloseDeviceConnection()
        {
            this.TurnOffFlexors();
            this.TurnOffActuators();
            this.LegacyOpenGlove.communication.CloseDeviceConnection(); //TODO this need await bool CloseDeviceConnection()
            this.IsConnected = false;
        }
    }
}
