﻿using System;
namespace OpenGlove_API_C_Sharp_HL
{
    public class DataReceiverEventArgs : EventArgs
    {
        public int Region { get; set; }
        public int Value { get; set; }
        public float Ax { get; set; }
        public float Ay { get; set; }
        public float Az { get; set; }
        public float Gx { get; set; }
        public float Gy { get; set; }
        public float Gz { get; set; }
        public float Mx { get; set; }
        public float My { get; set; }
        public float Mz { get; set; }
    }
}
