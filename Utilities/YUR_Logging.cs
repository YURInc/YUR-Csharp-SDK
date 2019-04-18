using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace YUR.SDK.Unity
{
    public static class YUR_Log 
    {
        internal static bool Debugging { get; set; }
        internal static bool Editor_Debugging { get; set; }
        internal static bool Error_Logging { get; set; }
        internal static bool Server_Logging { get; set; }
        internal static bool Log_To_File { get; set; }

        /// <summary>
        /// Standard Logging with [YUR Log] >> Tag
        /// Writes to both Unity Debugging and Console
        /// </summary>
        /// <param name="toLog">Information to Log out</param>
        public static void Log(string toLog)
        {

            if (!Debugging)
                return;

            string log = "[ YUR | Log ] >> " + toLog;
            if (Log_To_File)
            {
                System.IO.Directory.CreateDirectory(Utilities.YUR_Conversions.PathCombine(UnityEngine.Application.dataPath, "../UserData/"));
                System.IO.File.AppendAllText(Utilities.YUR_Conversions.PathCombine(UnityEngine.Application.dataPath, "../UserData/YUR_Log.txt"), log + Environment.NewLine);

            }
                
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(log);
            if (Editor_Debugging)
                Debug.Log(log);
            Console.ResetColor();

        }

        /// <summary>
        /// Server Logging with [YUR Server Log] >> Tag
        /// Writes to both Unity Debugging and Console
        /// </summary>
        /// <param name="toLog">Information to Log Out</param>
        public static void Server_Log(string toLog)
        {
            if (!Server_Logging)
                return;

            string log = "[ YUR | Log ] || Server " + Login.Status + " || >> " + toLog;
            if (Log_To_File)
            {
                System.IO.Directory.CreateDirectory(Utilities.YUR_Conversions.PathCombine(UnityEngine.Application.dataPath, "../UserData/"));
                System.IO.File.AppendAllText(Utilities.YUR_Conversions.PathCombine(UnityEngine.Application.dataPath, "../UserData/YUR_Log.txt"), log + Environment.NewLine);

            }
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(log);
            if (Editor_Debugging)
                Debug.Log(log);
            Console.ResetColor();
        }

        /// <summary>
        /// Error Reporting with [YUR ERROR] >> Tag
        /// Writes to both Unity Debugging and Console
        /// </summary>
        /// <param name="error">Error to Output</param>
        public static void Error(string error)
        {
            if (!Error_Logging)
                return;
            string log = "[ YUR | Log ] || ERROR || >> " + error;
            if (Log_To_File)
            {
                System.IO.Directory.CreateDirectory(Utilities.YUR_Conversions.PathCombine(UnityEngine.Application.dataPath, "../UserData/"));
                System.IO.File.AppendAllText(Utilities.YUR_Conversions.PathCombine(UnityEngine.Application.dataPath, "../UserData/YUR_Log.txt"), log + Environment.NewLine);

            }
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(log);
            if (Editor_Debugging)
                Debug.Log(log);
            Console.ResetColor();
        }
    }
}
