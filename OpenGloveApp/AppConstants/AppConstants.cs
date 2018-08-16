using System;
using Xamarin.Forms;

namespace OpenGloveApp.AppConstants
{
    public static class AppConstants
    {
        public static readonly Thickness PagePadding = GetPagePadding();
        public static readonly Thickness ChildrenPagePadding = GetChildrenPagePadding();
        public static readonly string DeviceRuntime = GetDeviceRuntime();
        public const string Device_iOS = Device.iOS;
        public const string Device_Android = Device.Android;

        private static Thickness GetPagePadding()
        {
            double topPadding;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    topPadding = 20;
                    break;
                default:
                    topPadding = 0;
                    break;
            }

            return new Thickness(0, topPadding, 0, 0);
        }

        private static Thickness GetChildrenPagePadding(){
            return new Thickness(10,10,10,0); //left, top, right, bottom 
        }

        private static string GetDeviceRuntime()
        {
            return Device.RuntimePlatform;    
        }
    }

    // Colors for use inside the Xamarin.Forms C# 
    public static class Colors
    {
        public static readonly string ColorPrimary =  "#07bf94";
        public static readonly string ColorPrimaryDark = "#1e3740";
        public static readonly string ColorAccent = "#FF4081";
        public static readonly string ColorText = "#737474";
    }

    public enum OpenGloveActions
    {
        AddOpenGloveDevice = 0,
        RemoveOpenGloveDevice,
        SaveOpenGloveDevice,
        Connect,
        Disconnect,
        StartCaptureDataFromServer,
        StopCaptureDataFromServer,

        AddActuator = 7,
        AddActuators,
        RemoveActuator,
        RemoveActuators,
        ActivateActuators,
        ResetActuators,

        AddFlexor = 13,
        AddFlexors,
        RemoveFlexor,
        RemoveFlexors,
        CalibrateFlexors,
        ConfirmCalibration,
        SetThreshold,
        ResetFlexors,

        StartIMU = 21,
        SetIMUStatus,
        SetRawData,
        CalibrateIMU,

        SetLoopDelay = 25,
    }

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
