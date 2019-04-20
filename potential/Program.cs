using System;
using System.Linq;
using System.Threading;
using HidSharp;
using static potential.RazerDeviceHelper;
using static potential.RazerKeyboard;
using System.Windows.Forms;

namespace potential
{
    unsafe class Program
    {
        private const string VERSION_TAG = "0.1a";

        #region logging
        private const string MAIN_TAG = "main";
        static void Log(string message, params object[] para)
        {
            Util.Log(Util.LOG_LEVEL.INFO, MAIN_TAG, message, para);
        }

        static void Warn(string message, params object[] para)
        {
            Util.Log(Util.LOG_LEVEL.WARN, MAIN_TAG, message, para);
        }

        static void Success(string message, params object[] para)
        {
            Util.Log(Util.LOG_LEVEL.SUCCESS, MAIN_TAG, message, para);
        }
        static void Error(string message, params object[] para)
        {
            Util.Log(Util.LOG_LEVEL.ERROR, MAIN_TAG, message, para);
        }
        #endregion
        
        static void Main(string[] args)
        {
            // we no longer need the console version - now we have a UI
            /*Log("welcome to potential {0}", VERSION_TAG);
            Log("searching for compatible razer devices...");
            HidDevice razerDevice = EnumerateRazerDevices().First();

            if (razerDevice == null)
            {
                Error("no compatible razer device found, exiting.");
                return;
            }

            Success("found razer device {0} [{1}]!", razerDevice.DevicePath, razerDevice.GetProductName());
            Log("setting purple keyboard @ 75%...");
            RazerAttrWriteModeStatic(razerDevice, new RazerRgb(192, 0, 192));
            RazerAttrWriteSetBrightness(razerDevice, 195);
            Log("all done - have a nice day!");*/
            Application.EnableVisualStyles();
            Application.Run(new Interface());
        }
    }
}
