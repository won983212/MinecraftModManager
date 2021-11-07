using System;

namespace MinecraftModManager
{
    internal static class Logger
    {
        public static void Info(object obj)
        {
            Console.WriteLine("[Info] " + obj.ToString());
        }

        public static void Debug(object obj)
        {
            Console.WriteLine("[Debug] " + obj.ToString());
        }

        public static void Error(object obj)
        {
            Console.WriteLine("[Error] " + obj.ToString());
        }
    }
}
