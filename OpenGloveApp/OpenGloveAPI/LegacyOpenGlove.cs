﻿using System.Collections.Generic;
using OpenGloveApp.Models;
using Xamarin.Forms;

namespace OpenGloveApp.OpenGloveAPI
{
    /// <summary>
    /// Represents an OpenGlove device instance. Provide methods for communication with the device, initialize and activate vibration motors, besides others actuators and sensors
    /// </summary>
    public class LegacyOpenGlove
    {
        /// <summary>
        /// An OpenGlove communication module instance.
        /// </summary>
        ICommunication communication = DependencyService.Get<ICommunication>();
        /// <summary>
        /// An OpenGlove message generator module instance.
        /// </summary>
        MessageGenerator messageGenerator = new MessageGenerator();

        /// <summary>
        /// Open the communication with the port and baudrate specified
        /// </summary>
        ///<param name = "portName" > Name of the serial port to open a communication</param>
        /// <param name="baudRate">Data rate in bits per second. Use one of these values: 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 57600, or 115200</param>
        public void OpenPort(string portName, int baudRate)
        {
            communication.OpenPort(portName, baudRate);
        }
        /// <summary>
        /// Close the current active serial communication
        /// </summary>
        public void ClosePort()
        {
            communication.ClosePort();
        }
        /// <summary>
        /// List all active serial ports names
        /// </summary>
        /// <returns>An array with the names of all active serial ports</returns>
        public string[] GetPortNames()
        {
            return communication.GetPortNames();
        }
        /// <summary>
        /// Initialize pins like motors in the control software
        /// </summary>
        /// <param name="pins">List of pins that  are initialized</param>
        public void InitializeMotor(IEnumerable<int> pins)
        {
            string message = messageGenerator.InitializeMotor(pins);
            communication.Write(message);
        }
        /// <summary>
        /// Activate motors with analog or digital values. Each motor is activated with the value with the same index
        /// </summary>
        /// <param name="pins">List of pins where are connected the motors</param>
        /// <param name="values">List with the intensities to activate the motors. It can be "HIGH" or "LOW" in digital mode or a number between 0 and 255 in analog mode</param>
        public void ActivateMotor(IEnumerable<int> pins, IEnumerable<string> values)
        {
            string message = messageGenerator.ActivateMotor(pins, values);
            communication.Write(message);
        }
        /// <summary>
        /// Read the input buffet until a next line character
        /// </summary>
        /// <returns>A string without the next line character</returns>
        public string ReadLine()
        {
            return communication.ReadLine();
        }
        /// <summary>
        /// Send the string trough serial port
        /// </summary>
        /// <param name="data">String to be sent</param>
        public void Write(string data)
        {
            communication.Write(data);
        }
        /// <summary>
        /// Returns the input voltage from a analog pin
        /// </summary>
        /// <param name="pin">Number of the analog pin to be readed</param>
        /// <returns>The input voltage readed form the analog pin, between 0 and 1023. This value must be converted to be used like temperature, etc. </returns>
        public string AnalogRead(int pin)
        {
            string message = messageGenerator.AnalogRead(pin);
            communication.Write(message);
            string valueString = communication.ReadLine();
            return valueString;
        }

        /// <summary>
        /// Returns the value from a digital pin
        /// </summary>
        /// <param name="pin">Number of the digital pin to be readed</param>
        /// <returns>1 for "HIGH" or 0 for "LOW"</returns>
        public string DigitalRead(int pin)
        {
            string message = messageGenerator.DigitalRead(pin);
            communication.Write(message);
            string valueString = communication.ReadLine();
            return valueString;
        }
        /// <summary>
        /// Initialize a pin in input or output mode
        /// </summary>
        /// <param name="pin">Number of the pin to be initialized</param>
        /// <param name="mode">Mode to initialize the pin, it can be "INPUT" or "OUTPUT"</param>
        public void PinMode(int pin, string mode)
        {
            string message = messageGenerator.PinMode(pin, mode);
            communication.Write(message);
        }
        /// <summary>
        /// Initialize multiples pins in input or output mode. Each pin is initialized with the mode in the same index
        /// </summary>
        /// <param name="pins">List with the numbers of the pins to be initialized</param>
        /// <param name="modes">List with the modes to initialize the pins, it can be "INPUT" or "OUTPUT"</param>
        public void PinMode(IEnumerable<int> pins, IEnumerable<string> modes)
        {
            string message = messageGenerator.PinMode(pins, modes);
            communication.Write(message);
        }
        /// <summary>
        ///  Write a value to a digital pin
        /// </summary>
        /// <param name="pin">Number of the pin to be writed</param>
        /// <param name="value">Value to be write in the pin, it can be "HIGH" or "LOW"</param>
        public void DigitalWrite(int pin, string value)
        {
            string message = messageGenerator.DigitalWrite(pin, value);
            communication.Write(message);
        }

        /// <summary>
        /// Write values on digital pins. Each motor is activated with the value with the same index
        /// </summary>
        /// <param name="pins">List with the numbers of the pins to be initialized</param>
        /// <param name="values">List with the values to write on the pins, it can be "HIGH" or "LOW"</param>
        public void DigitalWrite(IEnumerable<int> pins, IEnumerable<string> values)
        {
            string message = messageGenerator.DigitalWrite(pins, values);
            communication.Write(message);
        }

        /// <summary>
        ///  Write an analog value to a pin
        /// </summary>
        /// <param name="pin">Number of the pin to be writed</param>
        /// <param name="value">Value to be write in the pin, it can be between 0 (always off) and 255 (always on)</param>
        public void AnalogWrite(int pin, int value)
        {
            string message = messageGenerator.AnalogWrite(pin, value);
            communication.Write(message);
        }
        /// <summary>
        /// Write analog values to pins. Each motor is activated with the value with the same index
        /// </summary>
        /// <param name="pins">List with the numbers of the pins to be writed</param>
        /// <param name="values">List with the values to write on the pins, it can be between 0 (always off) and 255 (always on)</param>
        public void AnalogWrite(IEnumerable<int> pins, IEnumerable<int> values)
        {
            string message = messageGenerator.AnalogWrite(pins, values);
            communication.Write(message);

        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="mapping"></param>
        public void addFlexor(int pin, int mapping)
        {
            string message = messageGenerator.addFlexor(pin, mapping);
            communication.Write(message);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="mapping"></param>
        public void removeFlexor(int mapping)
        {
            string message = messageGenerator.removeFlexor(mapping);
            communication.Write(message);
        }
        //calibrateFlexors()

        /// <summary>
        ///  
        /// </summary>
        public void calibrateFlexors()
        {
            string message = messageGenerator.calibrateFlexors();
            communication.Write(message);
        }

        /// <summary>
        ///  
        /// </summary>
        public void confirmCalibration()
        {
            string message = messageGenerator.confirmCalibration();
            communication.Write(message);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="value"></param>
        public void setThreshold(int value)
        {
            string message = messageGenerator.setThreshold(value);
            communication.Write(message);
        }

        /// <summary>
        ///  
        /// </summary>
        public void resetFlexors()
        {
            string message = messageGenerator.resetFlexors();
            communication.Write(message);
        }
        /////////////////
        //IMU FUNCTIONS//
        /////////////////

        /// <summary>
        ///  
        /// </summary>
        public void startIMU()
        {
            string message = messageGenerator.startIMU();
            communication.Write(message);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="status"></param>
        public void setIMUStatus(int status)
        {
            string message = messageGenerator.setIMUStatus(status);
            communication.Write(message);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="status"></param>
        public void setRawData(int status)
        {
            string message = messageGenerator.setRawData(status);
            communication.Write(message);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="value"></param>
        public void setLoopDelay(int value)
        {
            string message = messageGenerator.setLoopDelay(value);
            communication.Write(message);
        }

        public List<BluetoothDevices> GetAllPairedDevices()
        {
            return communication.GetAllPairedDevices();
        }

        public void OpenDeviceConnection(ContentPage contentPage, BluetoothDevices bluetoothDevice){
            communication.OpenDeviceConnection(contentPage, bluetoothDevice);
        }
    }
}