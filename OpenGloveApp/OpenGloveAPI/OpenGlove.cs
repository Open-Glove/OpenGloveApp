﻿using System.Collections.Generic;
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

        public OpenGlove()
            : base ()
        {
        }

        public OpenGlove(OpenGloveConfiguration configuration)
            : base()
        {
            this.Configuration = configuration;
        }

        public OpenGlove(string bluetoothDeviceName, OpenGloveConfiguration configuration)
            : base()    
        {
            this.BluetoothDeviceName = bluetoothDeviceName;
            this.Configuration = configuration;
        }

        public void InitializeActuators()
        {
            if (BooleanStatements.NoNullAndCountGreaterThanZero(this.Configuration.ActuatorsByMapping))
            {
                List<int> positivePins = new List<int>();
                List<int> negativePins = new List<int>();

                foreach (Actuator actuator in this.Configuration.ActuatorsByMapping.Values)
                {
                    positivePins.Add(actuator.PositivePin);
                    negativePins.Add(actuator.NegativePin);
                }

                this.LegacyOpenGlove.InitializeMotor(positivePins);
                this.LegacyOpenGlove.InitializeMotor(negativePins);
            }
        }

        public void ActivateActuators(List<int> regions, List<string> intensities)
        {
            if(BooleanStatements.NoNullAndEqualCount(regions, intensities))
            {
                List<int> positivePins = new List<int>();

                foreach (int region in regions)
                {
                    positivePins.Add(this.Configuration.ActuatorsByMapping[region].PositivePin);
                }

                this.LegacyOpenGlove.ActivateMotor(positivePins, intensities);
            }
        }

        public void AddFlexor(int pin, int region)
        {
            Configuration.FlexorPins.Add(pin);
            Configuration.FlexorsByMapping.Add(region, pin);
            //this.addFlexor(pin, region);
            this.LegacyOpenGlove.addFlexor(pin, region);
        }

        public void AddFlexors(List<int> pins, List<int> regions)
        {
            if (BooleanStatements.NoNullAndEqualCount(regions, pins))
            {
                for (int i = 0; i < regions.Count; i++)
                {
                    Configuration.FlexorPins.Add(pins[i]);
                    Configuration.FlexorsByMapping.Add(regions[i], pins[i]);
                    //this.addFlexor(pins[i], regions[i]);
                    this.LegacyOpenGlove.addFlexor(pins[i], regions[i]);
                }
            }
        }

        public void RemoveFlexor(int region)
        {
            Configuration.FlexorPins.Remove(region);
            Configuration.FlexorsByMapping.Remove(region);
            //this.removeFlexor(region);
            this.LegacyOpenGlove.removeFlexor(region);
        }

        public void RemoveFlexors(List<int> regions)
        {
            foreach (int region in regions)
            {
                Configuration.FlexorPins.Remove(region);
                Configuration.FlexorsByMapping.Remove(region);
                //this.removeFlexor(region);
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
            Configuration.FlexorPins.Clear();
            Configuration.FlexorsByMapping.Clear();
            this.LegacyOpenGlove.resetFlexors();
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
            return this.LegacyOpenGlove.communication.GetAllPairedDevices();
        }

        public void OpenDeviceConnection(BluetoothDevices bluetoothDevice)
        {
            this.LegacyOpenGlove.communication.OpenDeviceConnection(bluetoothDevice);
        }

        // TODO quit this, only fir prototypes aplications
        public void OpenDeviceConnection(ContentPage contentPage, BluetoothDevices bluetoothDevice)
        {
            this.LegacyOpenGlove.communication.OpenDeviceConnection(contentPage, bluetoothDevice);
        }
    }
}
