using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;
using HidSharp;

namespace potential
{
    unsafe class RazerDeviceHelper
    {
        #region definitions

        public const int RAZER_USB_REPORT_LEN = 0x5A;

        public enum LED_STATE : byte { OFF , ON }

        public enum LEDS : byte
        {
            ZERO_LED           = 0x00,
            SCROLL_WHEEL_LED   = 0x01,
            BATTERY_LED        = 0x03,
            LOGO_LED           = 0x04,
            BACKLIGHT_LED      = 0x05,
            MACRO_LED          = 0x07, 
            GAME_LED           = 0x08,
            RED_PROFILE_LED    = 0x0C,
            GREEN_PROFILE_LED  = 0x0D,
            BLUE_PROFILE_LED   = 0x0E
        }

        public enum LED_STORAGE : byte { NOSTORE , VARSTORE }

        public enum LED_EFFECT : byte
        {
            LED_STATIC           = 0x00,
            LED_BLINKING         = 0x01,
            LED_PULSATING        = 0x02,
            LED_SPECTRUM_CYCLING = 0x04
        }

        public enum REPORT_RESPONSE : byte
        {
            RAZER_CMD_BUSY          = 0x01,
            RAZER_CMD_SUCCESSFUL    = 0x02,
            RAZER_CMD_FAILURE       = 0x03,
            RAZER_CMD_TIMEOUT       = 0x04,
            RAZER_CMD_NOT_SUPPORTED = 0x05
        }

        public struct RazerRgb
        {
            public byte r, g, b;

            public RazerRgb(byte r, byte g, byte b)
            {
                this.r = r;
                this.g = g;
                this.b = b;
            }

            public RazerRgb(Color c)
            {
                this.r = c.R;
                this.g = c.G;
                this.b = c.B;
            }
        }

        public struct RazerReport
        {
            public byte status;
            
            /* transaction_id_union */
            public byte transaction_id;

            public ushort remaining_packets; /* Big Endian */
            public byte protocol_type; /*0x0*/
            public byte data_size;
            public byte command_class;

            /* command_id_union */
            public byte command_id;

            public fixed byte arguments[80];
            public byte crc;/*xor'ed bytes of report*/
            public byte reserved; /*0x0*/
        };


        private const int RAZER_VENDOR_ID = 0x1532;
        private static readonly int[] RAZER_BLADE_DEVICE_IDS = { 0x0205, 0x0240, 0x023b, 0x023a, 0x0209, 0x020F, 0x0210, 0x0233, 0x0220 };

        #endregion

        #region logging
        private const string DEVICE_TAG = "deviceHelper";
        static void Log(string message, params object[] para)
        {
            Util.Log(Util.LOG_LEVEL.INFO, DEVICE_TAG, message, para);
        }

        static void Success(string message, params object[] para)
        {
            Util.Log(Util.LOG_LEVEL.SUCCESS, DEVICE_TAG, message, para);
        }
        static void Error(string message, params object[] para)
        {
            Util.Log(Util.LOG_LEVEL.ERROR, DEVICE_TAG, message, para);
        }
        #endregion

        public static IEnumerable<HidDevice> EnumerateRazerDevices()
        {
            List<HidDevice> devices = new List<HidDevice>();
            foreach (int dev in RAZER_BLADE_DEVICE_IDS)
            {
                devices.AddRange(DeviceList.Local.GetHidDevices(RAZER_VENDOR_ID, dev)
                    .Where(x => (x.GetMaxFeatureReportLength() == 91)));
            }

            return devices;
        }

        /* util functions for marshalling the report struct to byte arrays */
        private static byte[] ReportToBytes(RazerReport data)
        {
            int sz = Marshal.SizeOf(data);
            byte[] ret = new byte[sz];
            IntPtr ptr = Marshal.AllocHGlobal(sz);
            Marshal.StructureToPtr(data, ptr, false);
            Marshal.Copy(ptr, ret, 0, sz);
            Marshal.FreeHGlobal(ptr);
            return ret;
        }

        private static RazerReport BytesToReport(byte[] data, int offset)
        {
            RazerReport ret = new RazerReport();
            int sz = Marshal.SizeOf(ret);
            IntPtr ptr = Marshal.AllocHGlobal(sz);
            Marshal.Copy(data, offset, ptr, sz);
            ret = (RazerReport)Marshal.PtrToStructure(ptr, typeof(RazerReport));
            Marshal.FreeHGlobal(ptr);
            return ret;
        }

        /* sends a report */
        public static bool RazerSendControlMsg(HidDevice dev, RazerReport data, byte report_index)
        {

            byte[] buf = new byte[RAZER_USB_REPORT_LEN + 1];

            buf[0] = report_index;
            Array.Copy(ReportToBytes(data), 0, buf, 1, RAZER_USB_REPORT_LEN);
            try
            {
                HidStream stream = dev.Open();
                stream.SetFeature(buf, 0, 91);
                return true;
            }
            catch (Exception e)
            {
                Error("could not write control message: {0}", e.Message);
                return false;
            }
        }

        /* sends a report, then receives a response */
        public static RazerReport RazerGetUsbResponse(HidDevice dev, byte report_index, RazerReport data)
        {
            RazerSendControlMsg(dev, data, report_index);
            
            byte[] response = new byte[RAZER_USB_REPORT_LEN + 1];
            try
            {
                HidStream stream = dev.Open();
                stream.GetFeature(response, 0, 91);
                RazerReport ret = BytesToReport(response, 1);
                return ret;
            }
            catch (Exception e)
            {
                Error("failed to read feature report from device: {0}", e.Message);
                return new RazerReport();
            }
        }

        /* calculates crc - xor all bytes */
        public static byte RazerCalculateCrc(RazerReport data)
        {
            byte crc = 0;
            byte[] raw = ReportToBytes(data);
            for (int i = 2; i < 88; i++)
            {
                crc ^= raw[i];
            }
            return crc;
        }

        /* creates a report */
        public static RazerReport GetRazerReport(byte command_class, byte command_id, byte data_size)
        {
            RazerReport report = new RazerReport();
            report.status = 0x0;
            report.transaction_id = 0xFF;
            report.remaining_packets = 0x0;
            report.protocol_type = 0x0;
            report.command_class = command_class;
            report.command_id = command_id;
            report.data_size = data_size;
            return report;
        }

        /* clamp functions */
        public static byte clamp_u8(byte value, byte min, byte max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }
        public static ushort clamp_u16(ushort value, ushort min, ushort max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }
    }
}
