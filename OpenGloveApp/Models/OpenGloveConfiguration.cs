using System;
using System.Collections.Generic;

namespace OpenGloveApp.Models
{
    public class OpenGloveConfiguration
    {
        public string FileName { get; set; }
        public List<int> Pins;
        public List<int> PositivePins;
        public List<int> NegativePins;
        public List<int> FlexorPins;
        public List<string> ValuesON;  //values: HIGH, LOW, 0-255
        public List<string> ValuesOFF; //values: HIGH, LOW, 0-255
        public List<Actuator> ActuatorsList;
        public Dictionary<int, Actuator> ActuatorsByMapping = new Dictionary<int, Actuator>();
        public Dictionary<int, int> FlexorsByMapping = new Dictionary<int, int>();

        public OpenGloveConfiguration()
        {
            // Vibe board: (+11 y -12), (+10 y -15), (+9 y -16), (+3 y -2), (+6, -8)
            this.Pins = new List<int> { 11, 12}; //{ 11, 12, 10, 15, 9, 16, 3, 2, 6, 8 };
            this.PositivePins  = new List<int> { 11, 10, 9, 3, 6 };
            this.NegativePins = new List<int> { 12, 15, 16, 2, 8 };
            this.FlexorPins = new List<int> { 17 };
            this.ValuesON = new List<string> { "HIGH", "HIGH","HIGH","HIGH", "HIGH" }; //{ "HIGH", "LOW", "HIGH", "LOW", "HIGH", "LOW", "HIGH", "LOW", "HIGH", "LOW" };
            this.ValuesOFF = new List<string> { "LOW","LOW", "LOW", "LOW", "LOW" }; //{ "LOW", "LOW", "LOW", "LOW", "LOW", "LOW", "LOW", "LOW", "LOW", "LOW" };
            //this.ActuatorsList = new List<Actuator> { new Actuator { PositivePin = Pins[0], NegativePin = Pins[1] }, new Actuator { PositivePin = Pins[2], NegativePin = Pins[3] } };
            //this.ActuatorsByMapping.Add((int) AppConstants.PalmarRegion.FingerSmallDistal, ActuatorsList[0]); 
            //this.ActuatorsByMapping.Add((int) AppConstants.PalmarRegion.FingerRingDistal, ActuatorsList[1]);

            //this.FlexorsByMapping.Add((int) AppConstants.FlexorsRegion.ThumbInterphalangealJoint, FlexorPins[0]);
        }
    }
}
