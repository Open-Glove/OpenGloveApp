﻿using System.Collections.Generic;
using System.Threading;
using OpenGloveApp.Models;
using OpenGloveApp.Utils;
using Xamarin.Forms;

namespace OpenGloveApp.OpenGloveAPI
{
    public class OpenGloveDevice
    {
        public string BluetoothDeviceName { get; set; }
        public OpenGloveConfiguration Configuration { get; set; }
        public LegacyOpenGlove LegacyOpenGlove = new LegacyOpenGlove();
        public bool IsConnected { get; set; }
        private int MillisecondsBetweenConfigurationMessages = 1; // For fix issue lost messages to device for many messages in a short time

        public OpenGloveDevice(string bluetoothDeviceName)
            : base()
        {
            this.BluetoothDeviceName = bluetoothDeviceName;
            this.Configuration = new OpenGloveConfiguration();
        }

        public OpenGloveDevice(string bluetoothDeviceName, OpenGloveConfiguration configuration)
            : base()
        {
            this.BluetoothDeviceName = bluetoothDeviceName;
            this.Configuration = configuration;
        }

        public void TurnOnAllOpenGloveComponents()
        {
            this.TurnOnIMU();
            this.TurnOnFlexors();
            this.TurnOnActuators();
        }

        public void TurnOffAllOpenGloveComponents()
        {
            this.TurnOffIMU();
            this.TurnOffFlexors();
            this.TurnOffActuators();
        }

        public void InitializeOpenGloveConfigurationOnDevice()
        {
            this.InitializeGlobalSettings();
            this.InitializeIMU();
            this.InitializeActuators();
            this.InitializeFlexors();
        }

        public void InitializeActuators()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
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
                    Thread.Sleep(MillisecondsBetweenConfigurationMessages);
                    this.LegacyOpenGlove.InitializeMotor(negativePins);
                    Thread.Sleep(MillisecondsBetweenConfigurationMessages);
                }
            }).Start();
        }

        public void InitializeFlexors()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                if (BooleanStatements.NoNullAndCountGreaterThanZero(this.Configuration.FlexorsByRegion))
                {
                    this.SetThreshold(this.Configuration.Threshold);
                    foreach (var flexorByRegion in this.Configuration.FlexorsByRegion)
                    {
                        this.LegacyOpenGlove.addFlexor(flexorByRegion.Value, flexorByRegion.Key);
                        Thread.Sleep(MillisecondsBetweenConfigurationMessages);
                    }
                }
            }).Start();
        }

        public void InitializeIMU()
        {
            if (this.Configuration != null)
            {
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    this.LegacyOpenGlove.startIMU();
                    Thread.Sleep(MillisecondsBetweenConfigurationMessages);
                    this._SetIMUChoosingData(this.Configuration.IMUChoosingData);
                    Thread.Sleep(MillisecondsBetweenConfigurationMessages);
                    this._SetRawData(this.Configuration.IMURawData);
                    Thread.Sleep(MillisecondsBetweenConfigurationMessages);
                    this._SetIMUStatus(this.Configuration.IMUStatus);
                    Thread.Sleep(MillisecondsBetweenConfigurationMessages);
                }).Start();

            }
        }

        public void InitializeGlobalSettings()
        {
            
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                this.LegacyOpenGlove.setLoopDelay(this.Configuration.LoopDelay);
                Thread.Sleep(MillisecondsBetweenConfigurationMessages);
            }
            ).Start();
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
                //InitializeActuators();
            }
        }

        public void AddActuators(List<int> regions, List<int> positivePins, List<int> negativePins)
        {
            //bool added = false;
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
                        //added = true;
                    }

                }
                //if(added)
                  //InitializeActuators();
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

        public void ActivateActuatorsTimeTest(List<int> regions, List<string> intensities)
        {
            if (BooleanStatements.NoNullAndEqualCount(regions, intensities))
            {
                List<int> positivePins = new List<int>();

                foreach (int region in regions)
                {
                    positivePins.Add(this.Configuration.ActuatorsByRegion[region].PositivePin);
                }

                this.LegacyOpenGlove.ActivateMotorTimeTest(positivePins, intensities);
            }
        }

        public void TurnOnActuators()
        {
            List<int> regions = new List<int>();
            List<string> intensities = new List<string>();

            foreach (var entry in Configuration.ActuatorsByRegion)
            {
                regions.Add(entry.Key);
                intensities.Add("255"); // 255 or HIGH for turn On on MAX
            }

            this.ActivateActuators(regions, intensities); //Turn ON Actuators
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

        public void ResetActuators()
        {
            List<int> regions = new List<int>();
            List<string> intensities = new List<string>();

            foreach (var entry in Configuration.ActuatorsByRegion)
            {
                regions.Add(entry.Key);
                intensities.Add("0"); // 0 or LOW for turn off
            }

            this.ActivateActuators(regions, intensities); //Turn Off Actuators

            Configuration.ActuatorsByRegion.Clear();      //Clear mapping
        }

        public void AddFlexor(int region, int pin)
        {
            if (!Configuration.FlexorsByRegion.ContainsKey(region))
            {
                Configuration.FlexorsByRegion.Add(region, pin);
                //this.LegacyOpenGlove.addFlexor(pin, region);
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
                        //this.LegacyOpenGlove.addFlexor(pins[i], regions[i]);   
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
            this.Configuration.Threshold = value;
        }

        public void TurnOnFlexors()
        {
            InitializeFlexors();
        }

        public void TurnOffFlexors()
        {
            this.LegacyOpenGlove.resetFlexors();
        }

        public void ResetFlexors()
        {
            Configuration.FlexorsByRegion.Clear();
            this.LegacyOpenGlove.resetFlexors();
        }

        public void StartIMU()
        {
            this.LegacyOpenGlove.startIMU();
        }

        public void SetIMUStatus(bool status)
        {
            this.Configuration.IMUStatus = status;
        }

        private void _SetIMUStatus(bool status)
        {
            int integerStatus = (status) ? 1 : 0;
            this.LegacyOpenGlove.setIMUStatus(integerStatus);
        }

        public void TurnOnIMU()
        {
            this._SetIMUStatus(true);
        }

        public void TurnOffIMU()
        {
            this._SetIMUStatus(false);
        }

        public void SetRawData(bool status)
        {
            this.Configuration.IMURawData = status;
        }

        private void _SetRawData(bool status)
        {
            int integerStatus = (status) ? 1 : 0;
            this.LegacyOpenGlove.setRawData(integerStatus);
        }

        private void _SetIMUChoosingData(int value)
        {
            this.LegacyOpenGlove.setChoosingData(value);
        }

        public void SetIMUChoosingData(int value)
        {
            this.Configuration.IMUChoosingData = value;
        }

        public void ReadOnlyAccelerometerFromIMU()
        {
            this._SetIMUChoosingData(0);
        }

        public void ReadOnlyGyroscopeFromIMU()
        {
            this._SetIMUChoosingData(1);
        }

        public void ReadOnlyMagnetometerFromIMU()
        {
            this._SetIMUChoosingData(2);
        }

        public void ReadOnlyAttitudeFromIMU()
        {
            this._SetIMUChoosingData(3);
        }

        public void ReadAllDataFromIMU()
        {
            this._SetIMUChoosingData(-1);
        }

        public void SetLoopDelay(int value)
        {
            this.Configuration.LoopDelay = value;
        }

        public List<BluetoothDevices> GetAllPairedDevices()
        {
            return this.LegacyOpenGlove.communication.GetAllPairedDevices();
        }

        public void OpenDeviceConnection()
        {
            this.LegacyOpenGlove.communication.OpenDeviceConnection(this.BluetoothDeviceName);
        }

        public void CloseDeviceConnection()
        {
            this.TurnOffFlexors();
            this.TurnOffActuators();
            this.TurnOffIMU();
            this.LegacyOpenGlove.communication.CloseDeviceConnection();
        }

        public void GetOpenGloveArduinoSofwareVersion()
        {
            this.LegacyOpenGlove.GetOpenGloveArduinoSoftwareVersion();
        }
    }
}
