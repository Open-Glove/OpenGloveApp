using System;
using System.Collections.Generic;

public class BluetoothEventArgs : EventArgs
{
    public long ThreadId { get; set; }
    public int What { get; set; }
    public string Message { get; set; }

    public IEnumerable<int> Pins { get; set; }
    public IEnumerable<string> ValuesON { get; set; }
    public IEnumerable<string> ValuesOFF { get; set; }
    public IEnumerable<int> FlexorPins { get; set; }
    public IEnumerable<int> FlexorMapping { get; set; }
    public IEnumerable<string> FlexorPinsMode { get; set; }
}