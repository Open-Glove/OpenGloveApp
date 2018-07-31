using System;
using System.Collections.Generic;

namespace OpenGloveApp.Models
{
    public class OpenGloveConfiguration
    {
        public List<int> pins;
        public List<string> valuesON;  //values: HIGH, LOW, 0-255
        public List<string> valuesOFF; //values: HIGH, LOW, 0-255

        public OpenGloveConfiguration()
        {
            // Vibe board: (+11 y -12), (+10 y -15), (+9 y -16), (+3 y -2), (+6, -8)
            this.pins = new List<int> { 11, 12, 10, 15, 9, 16, 3, 2, 6, 8 };
            this.valuesON = new List<string> { "HIGH", "LOW", "HIGH", "LOW", "HIGH", "LOW", "HIGH", "LOW", "HIGH", "LOW" };
            this.valuesOFF = new List<string> { "LOW", "LOW", "LOW", "LOW", "LOW", "LOW", "LOW", "LOW", "LOW", "LOW" };
        }
    }
}
