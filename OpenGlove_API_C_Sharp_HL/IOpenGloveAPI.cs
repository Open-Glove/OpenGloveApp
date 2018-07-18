using System;
using System.Collections.Generic;

namespace OpenGlove_API_C_Sharp_HL
{
    public interface IOpenGloveAPI
    {
        DataReceiver GetDataReceiver(Glove selectedGlove);

        void StartCaptureData(Glove selectedGlove);

        void StopCaptureData(Glove selectedGlove);
        /// <summary>
        /// Refreshes the current devices list
        /// </summary>
        /// <returns></returns>
        List<Glove> UpdateDevices();

        int AddFlexor(Glove selectedGlove, int flexor, int mapping);

        int RemoveFlexor(Glove selectedGlove, int mapping);

        void CalibrateFlexors(Glove selectedGlove);

        void ConfirmCalibration(Glove selectedGlove);

        void SetThreshold(Glove selectedGlove, int value);

        void ResetFlexors(Glove selectedGlove);

        void StartIMU(Glove selectedGlove);

        void SetIMUStatus(Glove selectedGlove, bool value);

        void SetRawData(Glove selectedGlove, bool value);

        void SetLoopDelay(Glove selectedGlove, int value);

        /// <summary>
        /// Establishes connection with a glove
        /// </summary>
        /// <param name="selectedGlove">A Glove object to be connected</param>
        /// <returns>Result code</returns>
        int Connect(Glove selectedGlove);

        /// <summary>
        /// Closes a connection with a glove
        /// </summary>
        /// <param name="selectedGlove">A Glove object to be connected</param>
        /// <returns>Result code</returns>
        int Disconnect(Glove selectedGlove);

        /// <summary>
        /// Activates a region with certain intensity
        /// </summary>
        /// <param name="selectedGlove"></param>
        /// <param name="region"></param>
        /// <param name="intensity"></param>
        void Activate(Glove selectedGlove, int region, int intensity);

        void Activate(Glove selectedGlove, List<int> regions, List<int> intensityList);

        void SaveGlove(Glove selectedGlove);
    }
}
