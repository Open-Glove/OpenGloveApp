using System;
using System.Collections.Generic;
using System.Diagnostics;
using WebSocketSharp;

namespace OpenGlove_API_C_Sharp_HL.OpenGloveAPI_HL
{
    public class Communication
    {
        public WebSocket WebSocket { get; set; }
        public MessageGenerator MessageGenerator { get; set; }
        private bool _IsConnectedToBluetoothDevice { get; set; }
        public bool IsConnectedToBluetoohDevice { get { return _IsConnectedToBluetoothDevice; } }

        public Communication(string bluetoothDeviceName, string configurationName, string url)
        {
            WebSocket = new WebSocket(url);
            WebSocket.OnOpen += OnOpen;         
            WebSocket.OnMessage += OnMessage;
            WebSocket.OnClose += OnClose;
            WebSocket.OnError += OnError;
            _IsConnectedToBluetoothDevice = false;

            MessageGenerator = new MessageGenerator(bluetoothDeviceName, configurationName, ";", ",", "");
        }
        private void OnOpen(object sender, EventArgs e)
        {
            Debug.WriteLine($"Websocket Connected to Server!");
            this.AddOpenGloveDevice();
            Debug.WriteLine($"Added this OpenGloveDevice to Server");
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            //TODO update _IsConnectedToBluetoothDevice if Connected or Disconnected
            //TODO this listener Flexors and IMU messages
            Debug.WriteLine($"Received from Server: {e.Data}");
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            Debug.WriteLine($"WebSocket Closed: {e.Code}, {e.Reason}");
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Debug.WriteLine($"WebSocket Error: {e.Exception}, {e.Message}");
        }

        public void StartOpenGlove()
        {
            this.WebSocket.Send(MessageGenerator.StartOpenGlove());
        }

        public void StopOpenGlove()
        {
            this.WebSocket.Send(MessageGenerator.StopOpenGlove());
        }

        public void AssignOpenGloveConfiguration(string configurationName)
        {
            //this.WebSocket.Send(MessageGenerator);
        }

        public void InitializeOpenGloveConfigurationOnDevice()
        {
            //TODO add this action 
        }

        public void AddOpenGloveDevice()
        {
            this.WebSocket.Send(MessageGenerator.AddOpenGloveDevice());
        }

        public void RemoveOpenGloveDevice()
        {
            this.WebSocket.Send(MessageGenerator.AddOpenGloveDevice());
        }
        public void SaveOpenGloveConfiguration()
        {
            this.WebSocket.Send(MessageGenerator.SaveOpenGloveConfiguration());
        }

        public void ConnectToBluetoothDevice()
        {
            this.WebSocket.Send(MessageGenerator.ConnectToBluetoothDevice());
        }

        public void DisconnectFromBluetoothDevice()
        {
            this.WebSocket.Send(MessageGenerator.DisconnectFromBluetoothDevice());
        }

        public void StartCaptureDataFromServer()
        {
            this.WebSocket.Send(MessageGenerator.StartCaptureDataFromServer());
        }

        public void StopCaptureDataFromServer()
        {
            this.WebSocket.Send(MessageGenerator.StopCaptureDataFromServer());
        }

        public void AddActuator(int region, int positivePin, int negativePin)
        {
            this.WebSocket.Send(MessageGenerator.AddActuator(region, positivePin, negativePin));
        }

        public void AddActuators(List<int> regions, List<int> positivePins, List<int> negativePins)
        {
            this.WebSocket.Send(MessageGenerator.AddActuators(regions, positivePins, negativePins));
        }

        public void RemoveActuator(int region)
        {
            this.WebSocket.Send(MessageGenerator.RemoveActuator(region));
        }

        public void RemoveActuators(List<int> regions)
        {
            this.WebSocket.Send(MessageGenerator.RemoveActuators(regions));
        }

        public void ActivateActuators(List<int> regions, List<string> intensities)
        {
            this.WebSocket.Send(MessageGenerator.ActivateActuators(regions, intensities));
        }

        public void TurnOnActuators()
        {
            this.WebSocket.Send(MessageGenerator.TurnOnActuators());
        }

        public void TurnOffActuators()
        {
            this.WebSocket.Send(MessageGenerator.TurnOffActuators());
        }

        public void ResetActuators()
        {
            this.WebSocket.Send(MessageGenerator.ResetActuators());
        }

        public void AddFlexor(int region, int pin)
        {
            this.WebSocket.Send(MessageGenerator.AddFlexor(region, pin));
        }

        public void AddFlexors(List<int> regions, List<int> pins)
        {
            this.WebSocket.Send(MessageGenerator.AddFlexors(regions, pins));
        }

        public void RemoveFlexor(int region)
        {
            this.WebSocket.Send(MessageGenerator.RemoveFlexor(region));
        }

        public void RemoveFlexors(List<int> regions)
        {
            this.WebSocket.Send(MessageGenerator.RemoveFlexors(regions));
        }

        public void CalibrateFlexors()
        {
            this.WebSocket.Send(MessageGenerator.CalibrateFlexors());
        }

        public void ConfirmCalibration()
        {
            this.WebSocket.Send(MessageGenerator.ConfirmCalibration());
        }

        public void SetThreshold(int value)
        {
            this.WebSocket.Send(MessageGenerator.SetThreshold(value));
        }

        public void TurnOnFlexors()
        {
            this.WebSocket.Send(MessageGenerator.TurnOnFlexors());
        }

        public void TurnOffFlexors()
        {
            this.WebSocket.Send(MessageGenerator.TurnOffFlexors());
        }

        public void ResetFlexors()
        {
            this.WebSocket.Send(MessageGenerator.ResetFlexors());
        }

        public void StartIMU()
        {
            this.WebSocket.Send(MessageGenerator.StartIMU());
        }

        public void SetIMUStatus(bool status)
        {
            this.WebSocket.Send(MessageGenerator.SetIMUStatus(status));
        }

        public void SetRawData(bool status)
        {
            this.WebSocket.Send(MessageGenerator.SetRawData(status));
        }

        public void SetIMUChoosingData(int value)
        {
            this.WebSocket.Send(MessageGenerator.SetIMUChoosingData(value));
        }

        public void CalibrateIMU()
        {
            this.WebSocket.Send(MessageGenerator.CalibrateIMU());
        }

        public void SetLoopDelay(int value)
        {
            this.WebSocket.Send(MessageGenerator.SetLoopDelay(value));
        }

        public void ConnectToWebSocketServer()
        {
            this.WebSocket.Connect();
        }

        public void DisconnectFromWebSocketServer()
        {
            this.WebSocket.Close();
        }
    }
}
