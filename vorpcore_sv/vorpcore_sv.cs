using System;
using CitizenFX.Core;

namespace vorpcore_sv
{
    public class vorpcore_sv : BaseScript
    {
        public static bool IsLinux
        {
            get
            {
                var p = (int)Environment.OSVersion.Platform;
                return p == 4 || p == 6 || p == 128;
            }
        }

        public vorpcore_sv()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(@"" + "\n" +
                              @" /$$    /$$  /$$$$$$  /$$$$$$$  /$$$$$$$   /$$$$$$    /$$ /$$   /$$$$$$$  /$$$$$$$$" +
                              "\n" +
                              @"| $$   | $$ /$$__  $$| $$__  $$| $$__  $$ /$$__  $$  / $$/ $$  | $$__  $$| $$_____/" +
                              "\n" +
                              @"| $$   | $$| $$  \ $$| $$  \ $$| $$  \ $$| $$  \__/ /$$$$$$$$$$| $$  \ $$| $$      " +
                              "\n" +
                              @"|  $$ / $$/| $$  | $$| $$$$$$$/| $$$$$$$/| $$      |   $$  $$_/| $$$$$$$/| $$$$$   " +
                              "\n" +
                              @" \  $$ $$/ | $$  | $$| $$__  $$| $$____/ | $$       /$$$$$$$$$$| $$__  $$| $$__/   " +
                              "\n" +
                              @"  \  $$$/  | $$  | $$| $$  \ $$| $$      | $$    $$|_  $$  $$_/| $$  \ $$| $$      " +
                              "\n" +
                              @"   \  $/   |  $$$$$$/| $$  | $$| $$      |  $$$$$$/  | $$| $$  | $$  | $$| $$$$$$$$" +
                              "\n" +
                              @"    \_/     \______/ |__/  |__/|__/       \______/   |__/|__/  |__/  |__/|________/" +
                              "\n" +
                              "");

            if (IsLinux)
            {
                Console.WriteLine("\nVORP CORE Running on Linux, thanks for using VorpCore");
            }

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
