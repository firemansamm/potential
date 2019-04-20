using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static potential.RazerDeviceHelper;

namespace potential
{
    unsafe static class RazerChroma
    {
        public static RazerReport RazerChromaStandardGetDeviceMode()
        {
            return GetRazerReport(0x00, 0x84, 0x02);
        }

        public static RazerReport RazerChromaStandardGetSerial()
        {
            return GetRazerReport(0x00, 0x82, 0x16);
        }

        public static RazerReport RazerChromaStandardGetFirmwareVersion()
        {
            return GetRazerReport(0x00, 0x81, 0x02);
        }


        public static RazerReport RazerChromaStandardSetLedState(LED_STORAGE storage, LEDS led_id, LED_STATE led_state)
        {
            RazerReport report = GetRazerReport(0x03, 0x00, 0x03);
            report.arguments[0] = (byte) storage;
            report.arguments[1] = (byte) led_id;
            report.arguments[2] = clamp_u8((byte) led_state, 0x00, 0x01);
            return report;
        }

        public static RazerReport RazerChromaStandardSetLedBlinking(LED_STORAGE storage, LEDS led_id)
        {
            RazerReport report = GetRazerReport(0x03, 0x04, 0x04);
            report.arguments[0] = (byte) storage;
            report.arguments[1] = (byte) led_id;
            report.arguments[2] = 0x05;
            report.arguments[3] = 0x05;
            return report;
        }

        public static RazerReport RazerChromaStandardGetLedState(LED_STORAGE storage, LEDS led_id)
        {
            RazerReport report = GetRazerReport(0x03, 0x80, 0x03);
            report.arguments[0] = (byte) storage;
            report.arguments[1] = (byte) led_id;
            return report;
        }

        public static RazerReport RazerChromaStandardSetLedRgb(LED_STORAGE storage, LEDS led_id, RazerRgb rgb)
        {
            RazerReport report = GetRazerReport(0x03, 0x01, 0x05);
            report.arguments[0] = (byte) storage;
            report.arguments[1] = (byte) led_id;
            report.arguments[2] = rgb.r;
            report.arguments[3] = rgb.g;
            report.arguments[4] = rgb.b;
            return report;
        }

        public static RazerReport RazerChromaStandardGetLedRgb(LED_STORAGE storage, LEDS led_id)
        {
            RazerReport report = GetRazerReport(0x03, 0x81, 0x05);
            report.arguments[0] = (byte) storage;
            report.arguments[1] = (byte) led_id;
            return report;
        }

        public static RazerReport RazerChromaStandardSetLedEffect(LED_STORAGE storage, LEDS led_id, LED_EFFECT effect)
        {
            RazerReport report = GetRazerReport(0x03, 0x02, 0x03);
            report.arguments[0] = (byte) storage;
            report.arguments[1] = (byte) led_id;
            report.arguments[2] = clamp_u8((byte) effect, 0x00, 0x05);
            return report;
        }

        public static RazerReport RazerChromaStandardGetLedEffect(LED_STORAGE storage, LEDS led_id)
        {
            RazerReport report = GetRazerReport(0x03, 0x82, 0x03);
            report.arguments[0] = (byte)storage;
            report.arguments[1] = (byte)led_id;
            return report;
        }

        public static RazerReport RazerChromaStandardSetLedBrightness(LED_STORAGE storage, LEDS led_id, byte brightness)
        {
            RazerReport report = GetRazerReport(0x03, 0x03, 0x03);
            report.arguments[0] = (byte) storage;
            report.arguments[1] = (byte) led_id;
            report.arguments[2] = brightness;
            return report;
        }

        public static RazerReport RazerChromaStandardGetLedBrightness(LED_STORAGE storage, LEDS led_id)
        {
            RazerReport report = GetRazerReport(0x03, 0x83, 0x03);
            report.arguments[0] = (byte)storage;
            report.arguments[1] = (byte)led_id;
            return report;
        }

        public static RazerReport RazerChromaStandardMatrixEffectNone(LED_STORAGE storage, LEDS led_id)
        {
            RazerReport report = GetRazerReport(0x03, 0x0A, 0x01);
            report.arguments[0] = 0x00;
            return report;
        }

        public enum WAVE_DIRECTION : byte
        {
            LEFT = 0x01,
            RIGHT = 0x02
        }
        public static RazerReport RazerChromaStandardMatrixEffectWave(LED_STORAGE storage, LEDS led_id, WAVE_DIRECTION wave_direction)
        {
            RazerReport report = GetRazerReport(0x03, 0x0A, 0x02);
            report.arguments[0] = 0x01; // Effect ID
            report.arguments[1] = clamp_u8((byte)wave_direction, 0x01, 0x02);
            return report;
        }

        public static RazerReport RazerChromaStandardMatrixEffectSpectrum(LED_STORAGE storage, LEDS led_id)
        {
            RazerReport report = GetRazerReport(0x03, 0x0A, 0x01);
            report.arguments[0] = 0x04; // Effect ID
            return report;
        }

        public static RazerReport RazerChromaStandardMatrixEffectReactive(LED_STORAGE storage, LEDS led_id, byte speed, RazerRgb rgb1)
        {
            RazerReport report = GetRazerReport(0x03, 0x0A, 0x05);
            report.arguments[0] = 0x02; // Effect ID
            report.arguments[1] = clamp_u8(speed, 0x01, 0x04); // Time
            report.arguments[2] = rgb1.r; /*rgb color definition*/
            report.arguments[3] = rgb1.g;
            report.arguments[4] = rgb1.b;

            return report;
        }

        public static RazerReport RazerChromaStandardMatrixEffectStatic(LED_STORAGE storage, LEDS led_id, RazerRgb rgb1)
        {
            RazerReport report = GetRazerReport(0x03, 0x0A, 0x04);
            report.arguments[0] = 0x06; // Effect ID
            report.arguments[1] = rgb1.r; /*rgb color definition*/
            report.arguments[2] = rgb1.g;
            report.arguments[3] = rgb1.b;

            return report;
        }

        public static RazerReport RazerChromaStandardMatrixEffectStarlightSingle(LED_STORAGE storage, LEDS led_id, byte speed, RazerRgb rgb1)
        {
            RazerReport report = GetRazerReport(0x03, 0x0A, 0x01);

            speed = clamp_u8(speed, 0x01, 0x03); // For now only seen

            report.arguments[0] = 0x19; // Effect ID
            report.arguments[1] = 0x01; // Type one color
            report.arguments[2] = speed; // Speed

            report.arguments[3] = rgb1.r; // Red 1
            report.arguments[4] = rgb1.g; // Green 1
            report.arguments[5] = rgb1.b; // Blue 1

            // For now havent seen any chroma using this, seen the extended version
            report.arguments[6] = 0x00; // Red 2
            report.arguments[7] = 0x00; // Green 2
            report.arguments[8] = 0x00; // Blue 2

            return report;
        }

        public static RazerReport RazerChromaStandardMatrixEffectStarlightDual(LED_STORAGE storage, LEDS led_id, byte speed, RazerRgb rgb1, RazerRgb rgb2)
        {
            RazerReport report = GetRazerReport(0x03, 0x0A, 0x01);

            speed = clamp_u8(speed, 0x01, 0x03); // For now only seen

            report.arguments[0] = 0x19; // Effect ID
            report.arguments[1] = 0x02; // Type two color
            report.arguments[2] = speed; // Speed

            report.arguments[3] = rgb1.r; // Red 1
            report.arguments[4] = rgb1.g; // Green 1
            report.arguments[5] = rgb1.b; // Blue 1

            // For now havent seen any chroma using this, seen the extended version
            report.arguments[6] = rgb2.r; // Red 2
            report.arguments[7] = rgb2.g; // Green 2
            report.arguments[8] = rgb2.b; // Blue 2

            return report;
        }

        public static RazerReport RazerChromaStandardMatrixEffectStarlightRandom(LED_STORAGE storage, LEDS led_id, byte speed)
        {
            RazerReport report = GetRazerReport(0x03, 0x0A, 0x01);

            speed = clamp_u8(speed, 0x01, 0x03); // For now only seen

            report.arguments[0] = 0x19; // Effect ID
            report.arguments[1] = 0x03; // Type random color
            report.arguments[2] = speed; // Speed

            return report;
        }

        public static RazerReport RazerChromaStandardMatrixEffectBreathingRandom(LED_STORAGE storage, LEDS led_id)
        {
            RazerReport report = GetRazerReport(0x03, 0x0A, 0x08);
            report.arguments[0] = 0x03; // Effect ID
            report.arguments[1] = 0x03; // Breathing type

            return report;
        }

        public static RazerReport RazerChromaStandardMatrixEffectBreathingSingle(LED_STORAGE storage, LEDS led_id, RazerRgb rgb1)
        {
            RazerReport report = GetRazerReport(0x03, 0x0A, 0x08);
            report.arguments[0] = 0x03; // Effect ID
            report.arguments[1] = 0x01; // Breathing type
            report.arguments[2] = rgb1.r;
            report.arguments[3] = rgb1.g;
            report.arguments[4] = rgb1.b;

            return report;
        }

        public static RazerReport RazerChromaStandardMatrixEffectBreathingDual(LED_STORAGE storage, LEDS led_id, RazerRgb rgb1, RazerRgb rgb2)
        {
            RazerReport report = GetRazerReport(0x03, 0x0A, 0x08);
            report.arguments[0] = 0x03; // Effect ID
            report.arguments[1] = 0x02; // Breathing type
            report.arguments[2] = rgb1.r;
            report.arguments[3] = rgb1.g;
            report.arguments[4] = rgb1.b;
            report.arguments[5] = rgb2.r;
            report.arguments[6] = rgb2.g;
            report.arguments[7] = rgb2.b;

            return report;
        }

        public static RazerReport RazerChromaStandardMatrixEffectCustomFrame(LED_STORAGE storage)
        {
            RazerReport report = GetRazerReport(0x03, 0x0A, 0x02);
            report.arguments[0] = 0x05; // Effect ID
            report.arguments[1] = (byte)storage; // Data frame ID
                                                 // report.arguments[1] = 0x01; // Data frame ID

            return report;
        }

        public static RazerReport RazerChromaStandardMatrixSetCustomFrame(byte row_index, byte start_col, byte stop_col, byte[] rgb_data)
        {
            int row_length = (((stop_col + 1) - start_col) * 3);
            RazerReport report = GetRazerReport(0x03, 0x0B, 0x46); // In theory should be able to leave data size at max as we have start/stop
            int index = 4 + (start_col * 3);
            report.arguments[0] = 0xFF; // Frame ID
            report.arguments[1] = row_index;
            report.arguments[2] = start_col;
            report.arguments[3] = stop_col;
            for (int i = 0; i < row_length; i++) report.arguments[index + i] = rgb_data[i];

            return report;
        }

        public static RazerReport RazerChromaMiscFnKeyToggle(byte state)
        {
            RazerReport report = GetRazerReport(0x02, 0x06, 0x02);
            report.arguments[0] = 0x00; // ?? Variable storage maybe
            report.arguments[1] = clamp_u8(state, 0x00, 0x01); // State
            return report;
        }

        public static RazerReport RazerChromaMiscSetBladeBrightness(byte brightness)
        {
            RazerReport report = GetRazerReport(0x0E, 0x04, 0x02);
            report.arguments[0] = 0x01;
            report.arguments[1] = brightness;
            return report;
        }

        public static RazerReport RazerChromaMiscGetBladeBrightness()
        {
            RazerReport report = GetRazerReport(0x0E, 0x84, 0x02);
            report.arguments[0] = 0x01;
            return report;
        }

    }
}