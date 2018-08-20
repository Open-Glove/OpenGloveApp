using System;
using System.Collections.Generic;

namespace OpenGloveApp.Models
{
    public class OpenGloveConfiguration
    {
        public string Name { get; set; }
        public string FileExtension { get; set; }
        //public List<Actuator> ActuatorsList;
        public Dictionary<int, Actuator> ActuatorsByRegion = new Dictionary<int, Actuator>();
        public Dictionary<int, int> FlexorsByRegion = new Dictionary<int, int>();
        public int Threshold { get; set; }
        public bool IMUStatus { get; set; }
        public bool IMURawData { get; set; }
        public int IMUChoosingData { get; set; }
        public int SetLoopDelay { get; set; } //Miliseconds
        //TODO Calibration of flexors is always necesary to start ... set the calibration on configuration ... Future Work

        public OpenGloveConfiguration()
        {
            this.Threshold = 0;
            this.IMUStatus = true;
            this.IMUChoosingData = -1; //Get All Data Accelerometer, Gyroscope and Magnetometer
            this.IMURawData = false;
            this.SetLoopDelay = 60;
        }

        public OpenGloveConfiguration(string name)
        {
            this.Name = name;
            this.Threshold = 0;
            this.IMUStatus = true;
            this.IMUChoosingData = -1; //Get All Data Accelerometer, Gyroscope and Magnetometer
            this.IMURawData = false;
            this.SetLoopDelay = 60;
        }
    }
}
