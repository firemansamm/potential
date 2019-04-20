using System;
using HidSharp;
using static potential.RazerDeviceHelper;
using static potential.RazerChroma;

namespace potential
{
    static unsafe class RazerKeyboard
    {
        #region logging
        private const string DEVICE_TAG = "keyboardDriver";
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

        static RazerReport RazerGetReport(HidDevice device, RazerReport request_report)
        {
            return RazerGetUsbResponse(device, 0x02, request_report);
        }

        static RazerReport RazerSendPayload(HidDevice device, RazerReport request_report)
        {
            request_report.crc = RazerCalculateCrc(request_report);
            RazerReport response = RazerGetReport(device, request_report);

            if (response.remaining_packets != request_report.remaining_packets ||
                response.command_class != request_report.command_class ||
                response.command_id != request_report.command_id)
            {
                Error("response doesn't match request");
            } else if (response.status == (byte)REPORT_RESPONSE.RAZER_CMD_FAILURE)
            {
                Error("command failed");
            }
            else if (response.status == (byte)REPORT_RESPONSE.RAZER_CMD_NOT_SUPPORTED)
            {
                Error("command not supported");
            }
            else if (response.status == (byte)REPORT_RESPONSE.RAZER_CMD_TIMEOUT)
            {
                Error("command timed out");
            }

            return response;
        }

        public static string RazerAttrReadGetFirmwareVersion(HidDevice device)
        {
            RazerReport report = RazerChromaStandardGetFirmwareVersion();
            RazerReport response = RazerSendPayload(device, report);
            return String.Format("v{0}.{1}", response.arguments[0], response.arguments[1]);
        }

        public static string RazerAttrReadModeGame(HidDevice usb_dev)
        {
            RazerReport report = RazerChromaStandardGetLedState(LED_STORAGE.VARSTORE, LEDS.GAME_LED);
            RazerReport response = RazerSendPayload(usb_dev, report);
            return response.arguments[2].ToString();
        }

        /* TODO: what do these actually return? */
        public static int RazerAttrWriteModeMacro(HidDevice device, string buf, int count)
        {
            byte en = Convert.ToByte(buf, 10);
            RazerReport report = RazerChromaStandardSetLedState(LED_STORAGE.VARSTORE, LEDS.MACRO_LED, (en == 1) ? LED_STATE.ON : LED_STATE.OFF);
            RazerSendPayload(device, report);
            return count;
        }

        /* TODO: something is broken here... */
        public static int RazerAttrWriteModeMacroEffect(HidDevice device, string buf, int count)
        {
            byte en = Convert.ToByte(buf, 10);
            RazerReport report = RazerChromaStandardSetLedEffect(LED_STORAGE.VARSTORE, LEDS.MACRO_LED, (LED_EFFECT)en);
            RazerSendPayload(device, report);
            return count;
        }

        public static string RazerAttrReadModeMacroEffect(HidDevice device)
        {
            RazerReport report = RazerChromaStandardGetLedEffect(LED_STORAGE.VARSTORE, LEDS.MACRO_LED);
            RazerReport response = RazerSendPayload(device, report);
            return response.arguments[2].ToString();
        }

        public static int RazerAttrWriteModePulsate(HidDevice device, string buf, int count)
        {
            RazerReport report = RazerChromaStandardSetLedEffect(LED_STORAGE.VARSTORE, LEDS.BACKLIGHT_LED, LED_EFFECT.LED_PULSATING);
            RazerSendPayload(device, report);
            return count;
        }

        public static string RazerAttrReadModePulsate(HidDevice device)
        {
            RazerReport report = RazerChromaStandardGetLedEffect(LED_STORAGE.VARSTORE, LEDS.LOGO_LED);
            RazerReport response = RazerSendPayload(device, report);
            return response.arguments[2].ToString();
        }

        public static string RazerAttrReadTartarusProfileLedRed(HidDevice device)
        {
            RazerReport report = RazerChromaStandardGetLedState(LED_STORAGE.VARSTORE, LEDS.RED_PROFILE_LED);
            RazerReport response = RazerSendPayload(device, report);
            return response.arguments[2].ToString();
        }

        public static string RazerAttrReadTartarusProfileLedGreen(HidDevice device)
        {
            RazerReport report = RazerChromaStandardGetLedState(LED_STORAGE.VARSTORE, LEDS.GREEN_PROFILE_LED);
            RazerReport response = RazerSendPayload(device, report);
            return response.arguments[2].ToString();
        }

        public static string RazerAttrReadTartarusProfileLedBlue(HidDevice device)
        {
            RazerReport report = RazerChromaStandardGetLedState(LED_STORAGE.VARSTORE, LEDS.BLUE_PROFILE_LED);
            RazerReport response = RazerSendPayload(device, report);
            return response.arguments[2].ToString();
        }

        public static void RazerAttrWriteModeNone(HidDevice device)
        {
            RazerReport report = RazerChromaStandardMatrixEffectNone(LED_STORAGE.VARSTORE, LEDS.BACKLIGHT_LED);
            RazerSendPayload(device, report);
        }

        public static void RazerAttrWriteModeWave(HidDevice device, WAVE_DIRECTION direction)
        {
            RazerReport report =
                RazerChromaStandardMatrixEffectWave(LED_STORAGE.VARSTORE, LEDS.BACKLIGHT_LED, direction);
            RazerSendPayload(device, report);
        }

        public static void RazerAttrWriteModeSpectrum(HidDevice device)
        {
            RazerReport report = RazerChromaStandardMatrixEffectSpectrum(LED_STORAGE.VARSTORE, LEDS.BACKLIGHT_LED);
            RazerSendPayload(device, report);
        }

        public static void RazerAttrWriteModeReactive(HidDevice device, byte speed, RazerRgb rgb)
        {
            RazerReport report =
                RazerChromaStandardMatrixEffectReactive(LED_STORAGE.VARSTORE, LEDS.BACKLIGHT_LED, speed, rgb);
            RazerSendPayload(device, report);
        }

        public static void RazerAttrWriteModeStatic(HidDevice device, RazerRgb rgb)
        {
            RazerReport report = RazerChromaStandardMatrixEffectStatic(LED_STORAGE.VARSTORE, LEDS.BACKLIGHT_LED, rgb);
            RazerSendPayload(device, report);
        }

        public static void RazerAttrWriteModeStarlight(HidDevice device)
        {
            RazerReport report =
                RazerChromaStandardMatrixEffectStarlightRandom(LED_STORAGE.VARSTORE, LEDS.BACKLIGHT_LED, 0x01);
            RazerSendPayload(device, report);
        }
        public static void RazerAttrWriteModeStarlight(HidDevice device, RazerRgb rgb1)
        {
            RazerReport report =
                RazerChromaStandardMatrixEffectStarlightSingle(LED_STORAGE.VARSTORE, LEDS.BACKLIGHT_LED, 0x01, rgb1);
            RazerSendPayload(device, report);
        }

        public static void RazerAttrWriteModeStarlight(HidDevice device, RazerRgb rgb1, RazerRgb rgb2)
        {
            RazerReport report =
                RazerChromaStandardMatrixEffectStarlightDual(LED_STORAGE.VARSTORE, LEDS.BACKLIGHT_LED, 0x01, rgb1, rgb2);
            RazerSendPayload(device, report);
        }

        public static void RazerAttrWriteModeBreath(HidDevice device, RazerRgb? rgb1, RazerRgb? rgb2)
        {
            RazerReport report;
            if (rgb1 == null)
            {
                /* random mode */
                report = RazerChromaStandardMatrixEffectBreathingRandom(LED_STORAGE.VARSTORE, LEDS.BACKLIGHT_LED);
            }
            else if (rgb2 == null)
            {
                report = RazerChromaStandardMatrixEffectBreathingSingle(LED_STORAGE.VARSTORE, LEDS.BACKLIGHT_LED, rgb1.Value);
            }
            else
            {
                report = RazerChromaStandardMatrixEffectBreathingDual(LED_STORAGE.VARSTORE, LEDS.BACKLIGHT_LED, rgb1.Value, rgb2.Value);
            }

            RazerSendPayload(device, report);
        }

        public static void RazerAttrWriteSetLogo(HidDevice device, byte state)
        {
            RazerReport report =
                RazerChromaStandardSetLedEffect(LED_STORAGE.VARSTORE, LEDS.LOGO_LED, (LED_EFFECT) state);
            RazerSendPayload(device, report);
        }

        public static void RazerAttrWriteModeCustom(HidDevice device)
        {
            RazerReport report = RazerChromaStandardMatrixEffectCustomFrame(LED_STORAGE.VARSTORE);
            RazerSendPayload(device, report);
        }

        /* TODO: uhh this doesn't seem right */
        public static void RazerAttrWriteSetFnToggle(HidDevice device, byte state)
        {
            RazerReport report = RazerChromaMiscFnKeyToggle(state);
            RazerSendPayload(device, report);
        }

        public static void RazerAttrWriteSetBrightness(HidDevice device, byte brightness)
        {
            RazerReport report = RazerChromaMiscSetBladeBrightness(brightness);
            RazerSendPayload(device, report);
        }

        public static string RazerAttrReadSetBrightness(HidDevice device)
        {
            RazerReport report = RazerChromaMiscGetBladeBrightness();
            RazerReport response = RazerSendPayload(device, report);
            return response.arguments[1].ToString();
        }
        
    }
}
