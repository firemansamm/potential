using System;
namespace potential
{
    static class Util
    {
        public enum LOG_LEVEL
        {
            INFO,
            WARN,
            ERROR,
            SUCCESS
        }
        public static void Log(LOG_LEVEL level, string tag, string message, params object[] para)
        {
            switch (level)
            {
                case LOG_LEVEL.WARN:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LOG_LEVEL.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LOG_LEVEL.SUCCESS:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LOG_LEVEL.INFO:
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            Console.WriteLine("{0} [{1}]: {2}", DateTime.Now.ToString(), tag, string.Format(message, para));
        }
    }
}
