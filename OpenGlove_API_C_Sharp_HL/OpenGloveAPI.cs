using System;
using System.Collections.Generic;
using System.Linq;
using WebSocketSharp;

namespace OpenGlove_API_C_Sharp_HL
{
    public class OpenGloveAPI : IOpenGloveAPI
    {
        /// <summary>
        /// Singleton instance of the API
        /// </summary>
        private static OpenGloveAPI instance;
        // bool WebSocketActive;
        /// <summary>
        /// Client for the WCF service
        /// </summary>

        private List<DataReceiver> DataReceivers;

        OpenGloveAPI()
        {
            DataReceivers = new List<DataReceiver>();
            //  WebSocketActive = false;
        }
        /// <summary>
        /// Gets the current API instance
        /// </summary>
        /// <returns>Current API instance</returns>
        public static OpenGloveAPI GetInstance()
        {
            if (instance == null)
            {
                instance = new OpenGloveAPI();
            }
            return instance;
        }

        public void Activate(Glove selectedGlove, int region, int intensity)
        {
            throw new NotImplementedException();
        }

        public void Activate(Glove selectedGlove, List<int> regions, List<int> intensityList)
        {
            throw new NotImplementedException();
        }

        public int AddFlexor(Glove selectedGlove, int flexor, int mapping)
        {
            throw new NotImplementedException();
        }

        public void CalibrateFlexors(Glove selectedGlove)
        {
            throw new NotImplementedException();
        }

        public void ConfirmCalibration(Glove selectedGlove)
        {
            throw new NotImplementedException();
        }

        public int Connect(Glove selectedGlove)
        {
            throw new NotImplementedException();
        }

        public int Disconnect(Glove selectedGlove)
        {
            throw new NotImplementedException();
        }

        public DataReceiver GetDataReceiver(Glove selectedGlove)
        {
            throw new NotImplementedException();
        }

        public int RemoveFlexor(Glove selectedGlove, int mapping)
        {
            throw new NotImplementedException();
        }

        public void ResetFlexors(Glove selectedGlove)
        {
            throw new NotImplementedException();
        }

        public void SaveGlove(Glove selectedGlove)
        {
            throw new NotImplementedException();
        }

        public void SetIMUStatus(Glove selectedGlove, bool value)
        {
            throw new NotImplementedException();
        }

        public void SetLoopDelay(Glove selectedGlove, int value)
        {
            throw new NotImplementedException();
        }

        public void SetRawData(Glove selectedGlove, bool value)
        {
            throw new NotImplementedException();
        }

        public void SetThreshold(Glove selectedGlove, int value)
        {
            throw new NotImplementedException();
        }

        public void StartCaptureData(Glove selectedGlove)
        {
            throw new NotImplementedException();
        }

        public void StartIMU(Glove selectedGlove)
        {
            throw new NotImplementedException();
        }

        public void StopCaptureData(Glove selectedGlove)
        {
            throw new NotImplementedException();
        }

        public List<Glove> UpdateDevices()
        {
            throw new NotImplementedException();
        }
    }
}
