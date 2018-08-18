using System;
using System.Collections.Generic;

namespace OpenGlove_API_C_Sharp_HL.OpenGloveAPI_HL
{
    public class OpenGlove
    {
        public string Name { get; set; }
        public string BluetoothDeviceName { get { return this.BluetoothDeviceName; } set { this.BluetoothDeviceName = value; this.Communication.MessageGenerator.BluetoothDeviceName = value; } }
        public Communication Communication { get; set; }
        public bool IsConnected { get; set; }

        // TODO Need add ...

        public OpenGlove(string name, string bluetoothDeviceName, string url)
        {
            this.Name = name;
            this.BluetoothDeviceName = bluetoothDeviceName;
            this.Communication = new Communication(bluetoothDeviceName, url);
            this.IsConnected = false;
        }

        public void AddOpenGloveDevice()
        {
            this.Communication.AddOpenGloveDevice();
        }

        public void RemoveOpenGloveDevice()
        {
            this.Communication.AddOpenGloveDevice();
        }
        public void SaveOpenGloveDevice()
        {
            this.Communication.MessageGenerator.SaveOpenGloveDevice();
        }

        public void ConnectToBluetoothDevice()
        {
            this.Communication.MessageGenerator.ConnectToBluetoothDevice();
        }

        public void DisconnectFromBluetoothDevice()
        {
            this.Communication.MessageGenerator.DisconnectFromBluetoothDevice();
        }

        public void StartCaptureDataFromServer()
        {
            this.Communication.MessageGenerator.StartCaptureDataFromServer();
        }

        public void StopCaptureDataFromServer()
        {
            this.Communication.MessageGenerator.StopCaptureDataFromServer();
        }

        public void AddActuator(int region, int positivePin, int negativePin)
        {
            this.Communication.AddActuator(region, positivePin, negativePin);
        }

        public void AddActuators(List<int> regions, List<int> positivePins, List<int> negativePins)
        {
            this.Communication.AddActuators(regions, positivePins, negativePins);
        }

        public void RemoveActuator(int region)
        {
            this.Communication.RemoveActuator(region);
        }

        public void RemoveActuators(List<int> regions)
        {
            this.Communication.RemoveActuators(regions);
        }

        public void ActivateActuators(List<int> regions, List<string> intensities)
        {
            this.Communication.ActivateActuators(regions, intensities);
        }

        public void TurnOnActuators()
        {
            this.Communication.TurnOnActuators();
        }

        public void TurnOffActuators()
        {
            this.Communication.TurnOffFlexors();
        }

        public void TurnOnFlexors()
        {
            this.Communication.TurnOnFlexors();
        }

        public void TurnOffFlexors()
        {
            this.Communication.TurnOffFlexors();
        }

        public void ResetActuators()
        {
            this.Communication.ResetActuators();
        }

        public void AddFlexor(int region, int pin)
        {
            this.Communication.AddFlexor(region, pin);
        }

        public void AddFlexors(List<int> regions, List<int> pins)
        {
            this.Communication.AddFlexors(regions, pins);
        }

        public void RemoveFlexor(int region)
        {
            this.Communication.RemoveFlexor(region);
        }

        public void RemoveFlexors(List<int> regions)
        {
            this.Communication.RemoveFlexors(regions);
        }

        public void CalibrateFlexors()
        {
            this.Communication.CalibrateFlexors();
        }

        public void ConfirmCalibration()
        {
            this.Communication.ConfirmCalibration();
        }

        public void SetThreshold(int value)
        {
            this.Communication.SetThreshold(value);
        }

        public void ResetFlexors()
        {
            this.Communication.ResetFlexors();   
        }

        public void StartIMU()
        {
            this.Communication.StartIMU();
        }

        public void SetIMUStatus(bool status)
        {
            this.Communication.SetIMUStatus(status);
        }

        public void SetRawData(bool status)
        {
            this.Communication.SetRawData(status); 
        }

        /* 0 : a        Accelerometer
         * 1 : g        Gyroscope
         * 2 : m        Magnetometer
         * 3 : r        Attitude
         * default : z  Accelerometer, Gyroscope and Magnetometer
        */
        public void SetIMUChoosingData(int value)
        {
            this.Communication.SetIMUChoosingData(value);
        }

        public void ReadOnlyAccelerometerFromIMU()
        {
            this.SetIMUChoosingData(0);
        }

        public void ReadOnlyGyroscopeFromIMU()
        {
            this.SetIMUChoosingData(1);
        }

        public void ReadOnlyMagnetometerFromIMU()
        {
            this.SetIMUChoosingData(2);
        }

        public void ReadOnlyAttitudeFromIMU()
        {
            this.SetIMUChoosingData(3);
        }

        public void ReadAllDataFromIMU()
        {
            this.SetIMUChoosingData(-1);
        }

        public void CalibrateIMU()
        {
            throw new NotImplementedException();
            // Need Implement this on OpenGlove Aplication, see SwitchOpenGloveServer in OpenGloveServer class
            // Communication and MessageGenerator methods is OK
            //this.Communication.CalibrateIMU();
        }

        public void SetLoopDelay(int value)
        {
            this.Communication.SetLoopDelay(value);
        }

        public void ConnectToWebSocketServer()
        {
            this.Communication.ConnectToWebSocketServer();
        }

        public void DisconnectFromWebSocketServer()
        {
            this.Communication.DisconnectFromWebSocketServer();
        }
    }

    /*
     * Create your custom Regions for other types of Bluetooth wereables 
     * how helmets, general joints capture and actuators zones.
     * 
     * Palmar, Dorsal and Flexors Regions is for hands Mappings.
     * Enjoy!
    */

    public enum PalmarRegion
    {
        FingerSmallDistal,
        FingerRingDistal,
        FingerMiddleDistal,
        FingerIndexDistal,

        FingerSmallMiddle,
        FingerRingMiddle,
        FingerMiddleMiddle,
        FingerIndexMiddle,

        FingerSmallProximal,
        FingerRingProximal,
        FingerMiddleProximal,
        FingerIndexProximal,

        PalmSmallDistal,
        PalmRingDistal,
        PalmMiddleDistal,
        PalmIndexDistal,

        PalmSmallProximal,
        PalmRingProximal,
        PalmMiddleProximal,
        PalmIndexProximal,

        HypoThenarSmall,
        HypoThenarRing,
        ThenarMiddle,
        ThenarIndex,

        FingerThumbProximal,
        FingerThumbDistal,

        HypoThenarDistal,
        Thenar,

        HypoThenarProximal
    }

    public enum DorsalRegion
    {
        FingerSmallDistal = 29,
        FingerRingDistal,
        FingerMiddleDistal,
        FingerIndexDistal,

        FingerSmallMiddle,
        FingerRingMiddle,
        FingerMiddleMiddle,
        FingerIndexMiddle,

        FingerSmallProximal,
        FingerRingProximal,
        FingerMiddleProximal,
        FingerIndexProximal,

        PalmSmallDistal,
        PalmRingDistal,
        PalmMiddleDistal,
        PalmIndexDistal,

        PalmSmallProximal,
        PalmRingProximal,
        PalmMiddleProximal,
        PalmIndexProximal,

        HypoThenarSmall,
        HypoThenarRing,
        ThenarMiddle,
        ThenarIndex,

        FingerThumbProximal,
        FingerThumbDistal,

        HypoThenarDistal,
        Thenar,

        HypoThenarProximal
    }

    public enum FlexorsRegion
    {
        ThumbInterphalangealJoint = 0,
        IndexInterphalangealJoint,
        MiddleInterphalangealJoint,
        RingInterphalangealJoint,
        SmallInterphalangealJoint,

        ThumbMetacarpophalangealJoint,
        IndexMetacarpophalangealJoint,
        MiddleMetacarpophalangealJoint,
        RingMetacarpophalangealJoint,
        SmallMetacarpophalangealJoint
    }
}
