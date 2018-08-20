using System;
using System.Collections.Generic;

namespace OpenGlove_API_C_Sharp_HL.OpenGloveAPI_HL
{
    public class MessageGenerator
    {
        public enum OpenGloveActions
        {
            StartOpenGlove = 0,
            StopOpenGlove,
            AddOpenGloveDevice,
            RemoveOpenGloveDevice,
            SaveOpenGloveDevice,
            ConnectToBluetoothDevice,
            DisconnectFromBluetoothDevice,
            ConnectToWebSocketServer,
            DisconnecFromWebSocketServer,
            StartCaptureDataFromServer,
            StopCaptureDataFromServer,

            AddActuator = 11,
            AddActuators,
            RemoveActuator,
            RemoveActuators,
            ActivateActuators,
            TurnOnActuators,
            TurnOffActuators,
            ResetActuators,

            AddFlexor = 19,
            AddFlexors,
            RemoveFlexor,
            RemoveFlexors,
            CalibrateFlexors,
            ConfirmCalibration,
            SetThreshold,
            TurnOnFlexors,
            TurnOffFlexors,
            ResetFlexors,

            StartIMU = 29,
            SetIMUStatus,
            SetRawData,
            SetIMUChoosingData,
            ReadOnlyAccelerometerFromIMU,
            ReadOnlyGyroscopeFromIMU,
            ReadOnlyMagnetometerFromIMU,
            ReadOnlyAttitudeFromIMU,
            ReadAllDataFromIMU,
            CalibrateIMU,

            SetLoopDelay = 39,
        }

        public string BluetoothDeviceName { get; set; }
        public string ConfigurationName { get; set; }
        public string MainSeparator { get; set; }
        public string SecondarySeparator { get; set; }
        public string Empty { get; set; }

        public MessageGenerator(string bluetoothDeviceName, string configurationName, string mainSeparator, string secondarySeparator, string empty)
        {
            this.BluetoothDeviceName = bluetoothDeviceName;
            this.ConfigurationName = configurationName;
            this.MainSeparator = mainSeparator;
            this.SecondarySeparator = secondarySeparator;
            this.Empty = empty;
        }

        /* For implement this Join to other specific languages (C# String.Join using params object[] and Java String.Join using Object ...)
         * 
         * C#: String.Join https://docs.microsoft.com/en-us/dotnet/api/system.string.join?view=netframework-4.7.2#System_String_Join_System_String_System_Object___
         * You can test your C# code or other in:
         *  https://www.tutorialspoint.com/codingground.htm
         *  https://www.tutorialspoint.com/compile_csharp_online.php
        */
        private string Join(string separator, params object[] list)
        {
            return String.Join(separator, list);
        }

        private string BooleanString(bool value)
        {
            string booleanString = (value) ? "True" : "False";
            return booleanString;
        }

        public string StartOpenGlove()
        {
            return Join(MainSeparator, OpenGloveActions.StartOpenGlove, BluetoothDeviceName, Empty, ConfigurationName, Empty);
        }

        public string StopOpenGlove()
        {
            return Join(MainSeparator, OpenGloveActions.StopOpenGlove, BluetoothDeviceName, Empty, Empty, Empty);
        }

        public string AddOpenGloveDevice()
        {
            return Join(MainSeparator, OpenGloveActions.AddOpenGloveDevice, BluetoothDeviceName, Empty, Empty, Empty);
        }

        public string RemoveOpenGloveDevice()
        {
            return Join(MainSeparator, OpenGloveActions.RemoveOpenGloveDevice, BluetoothDeviceName, Empty, Empty, Empty);
        }
        public string SaveOpenGloveDevice()
        {
            return Join(MainSeparator, OpenGloveActions.SaveOpenGloveDevice, BluetoothDeviceName, Empty, Empty, Empty);
        }

        public string ConnectToBluetoothDevice()
        {
            return Join(MainSeparator, OpenGloveActions.ConnectToBluetoothDevice, BluetoothDeviceName, Empty, Empty, Empty);
        }

        public string DisconnectFromBluetoothDevice()
        {
            return Join(MainSeparator, OpenGloveActions.DisconnectFromBluetoothDevice, BluetoothDeviceName, Empty, Empty, Empty);
        }

        public string StartCaptureDataFromServer()
        {
            return Join(MainSeparator, OpenGloveActions.StartCaptureDataFromServer, BluetoothDeviceName, Empty, Empty, Empty);
        }

        public string StopCaptureDataFromServer()
        {
            return Join(MainSeparator, OpenGloveActions.StopCaptureDataFromServer, BluetoothDeviceName, Empty, Empty, Empty);
        }

        public string AddActuator(int region, int positivePin, int negativePin)
        {
            return Join(MainSeparator, OpenGloveActions.AddActuator, BluetoothDeviceName, region, positivePin, negativePin);
        }

        public string AddActuators(List<int> regions, List<int> positivePins, List<int> negativePins)
        {
            return Join(MainSeparator, OpenGloveActions.AddActuators, BluetoothDeviceName, Join(SecondarySeparator,regions), Join(SecondarySeparator, positivePins), Join(SecondarySeparator, negativePins));
        }

        public string RemoveActuator(int region)
        {
            return Join(MainSeparator, OpenGloveActions.RemoveActuator, BluetoothDeviceName, region, Empty, Empty);
        }

        public string RemoveActuators(List<int> regions)
        {
            return Join(MainSeparator, OpenGloveActions.RemoveActuators, BluetoothDeviceName, Join(SecondarySeparator, regions), Empty, Empty);
        }

        public string ActivateActuators(List<int> regions, List<string> intensities)
        {
            return Join(MainSeparator, OpenGloveActions.ActivateActuators, BluetoothDeviceName, Join(SecondarySeparator, regions), Join(SecondarySeparator, intensities), Empty);
        }

        public string TurnOnActuators()
        {
            return Join(MainSeparator, OpenGloveActions.TurnOnActuators, BluetoothDeviceName, Empty, Empty, Empty);
        }

        public string TurnOffActuators()
        {
            return Join(MainSeparator, OpenGloveActions.TurnOffActuators, BluetoothDeviceName, Empty, Empty, Empty);
        }

        public string ResetActuators()
        {
            return Join(MainSeparator, OpenGloveActions.ResetActuators, BluetoothDeviceName, Empty, Empty, Empty);
        }

        public string AddFlexor(int region, int pin)
        {
            return Join(MainSeparator, OpenGloveActions.AddFlexor, BluetoothDeviceName, region, pin, Empty);
        }

        public string AddFlexors(List<int> regions, List<int> pins)
        {
            return Join(MainSeparator, OpenGloveActions.AddFlexors, BluetoothDeviceName, Join(SecondarySeparator, regions), Join(SecondarySeparator, pins), Empty);
        }

        public string RemoveFlexor(int region)
        {
            return Join(MainSeparator, OpenGloveActions.RemoveFlexor, BluetoothDeviceName, region, Empty, Empty);
        }

        public string RemoveFlexors(List<int> regions)
        {
            return Join(MainSeparator, OpenGloveActions.RemoveFlexors, BluetoothDeviceName, Join(SecondarySeparator, regions), Empty, Empty);
        }

        public string CalibrateFlexors()
        {
            return Join(MainSeparator, OpenGloveActions.CalibrateFlexors, BluetoothDeviceName, Empty, Empty, Empty);
        }

        public string ConfirmCalibration()
        {
            return Join(MainSeparator, OpenGloveActions.ConfirmCalibration, BluetoothDeviceName, Empty, Empty, Empty);
        }

        public string SetThreshold(int value)
        {
            return Join(MainSeparator, OpenGloveActions.SetThreshold, BluetoothDeviceName, Empty, value, Empty);
        }

        public string TurnOnFlexors()
        {
            return Join(MainSeparator, OpenGloveActions.TurnOnFlexors, BluetoothDeviceName, Empty, Empty, Empty);
        }

        public string TurnOffFlexors()
        {
            return Join(MainSeparator, OpenGloveActions.TurnOffFlexors, BluetoothDeviceName, Empty, Empty, Empty);
        }

        public string ResetFlexors()
        {
            return Join(MainSeparator, OpenGloveActions.ResetFlexors, BluetoothDeviceName, Empty, Empty, Empty);
        }

        public string StartIMU()
        {
            return Join(MainSeparator, OpenGloveActions.StartIMU, BluetoothDeviceName, Empty, Empty, Empty);
        }

        public string SetIMUStatus(bool status)
        {
            return Join(MainSeparator, OpenGloveActions.SetIMUStatus, BluetoothDeviceName, Empty, BooleanString(status), Empty);
        }

        public string SetRawData(bool status)
        {
            return Join(MainSeparator, OpenGloveActions.SetRawData, BluetoothDeviceName, Empty, BooleanString(status), Empty);
        }

        public string SetIMUChoosingData(int value)
        {
            return Join(MainSeparator, OpenGloveActions.SetIMUChoosingData, BluetoothDeviceName, Empty, value, Empty);
        }

        public string CalibrateIMU()
        {
            return Join(MainSeparator, OpenGloveActions.CalibrateIMU, BluetoothDeviceName, Empty, Empty, Empty);
        }

        public string SetLoopDelay(int value)
        {
            return Join(MainSeparator, OpenGloveActions.SetLoopDelay, BluetoothDeviceName, Empty, value, Empty);
        }
    }
}
