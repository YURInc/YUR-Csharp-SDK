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
        /// <param name="ObjectReference">Object to which the message applies</param>
        public static void Log(string toLog, UnityEngine.Object ObjectReference = null)
        {

            if (!Debugging)
                return;

            string log = "[ YUR | Log ] >> " + toLog + "\n" + (ObjectReference != null ? "Name: " + ObjectReference.ToString() : "No object provided as reference");
            if (Log_To_File && Application.platform != RuntimePlatform.Android)
            {
                System.IO.Directory.CreateDirectory(Utilities.YUR_Conversions.PathCombine(UnityEngine.Application.dataPath, "../UserData/"));
                System.IO.File.AppendAllText(Utilities.YUR_Conversions.PathCombine(UnityEngine.Application.dataPath, "../UserData/YUR_Log.txt"), log + Environment.NewLine);
            }
                
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(log);
            if (Editor_Debugging)
            {
                if (ObjectReference != null)
                    Debug.Log(log, ObjectReference);
                else
                    Debug.Log(log);
            }
            Console.ResetColor();

        }

        /// <summary>
        /// Server Logging with [YUR Server Log] >> Tag
        /// Writes to both Unity Debugging and Console
        /// </summary>
        /// <param name="toLog">Information to Log Out</param>
        /// /// <param name="ObjectReference">Object to which the message applies</param>
        public static void Server_Log(string toLog, UnityEngine.Object ObjectReference = null)
        {
            if (!Server_Logging)
                return;

            string log = "<b>[ YUR | Log ] || Server " + Login.Status + " || >></b> " + toLog + "\n" + (ObjectReference != null ? "Name: " + ObjectReference.ToString() : "No object provided as reference");
            if (Log_To_File && Application.platform != RuntimePlatform.Android)
            {
                System.IO.Directory.CreateDirectory(Utilities.YUR_Conversions.PathCombine(UnityEngine.Application.dataPath, "../UserData/"));
                System.IO.File.AppendAllText(Utilities.YUR_Conversions.PathCombine(UnityEngine.Application.dataPath, "../UserData/YUR_Log.txt"), log + Environment.NewLine);
            }
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(log);
            if (Editor_Debugging)
            {
                if (ObjectReference != null)
                    Debug.Log(log, ObjectReference);
                else
                    Debug.Log(log);
            }
            Console.ResetColor();
        }


        public static void Warning(string toLog, UnityEngine.Object ObjectReference = null)
        {

            if (!Debugging)
                return;

            string log = "<b>[ YUR | WARNING ] >> </b>" + toLog + "\n" + (ObjectReference != null ? "Name: " + ObjectReference.ToString() : "No object provided as reference");
            if (Log_To_File && Application.platform != RuntimePlatform.Android)
            {
                System.IO.Directory.CreateDirectory(Utilities.YUR_Conversions.PathCombine(UnityEngine.Application.dataPath, "../UserData/"));
                System.IO.File.AppendAllText(Utilities.YUR_Conversions.PathCombine(UnityEngine.Application.dataPath, "../UserData/YUR_Log.txt"), log + Environment.NewLine);
            }

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(log);
            if (Editor_Debugging)
            {
                if (ObjectReference != null)
                    Debug.LogWarning(log, ObjectReference);
                else
                    Debug.LogWarning(log);
            }
                
            Console.ResetColor();

        }
        /// <summary>
        /// Error Reporting for YUR
        /// Writes to both Unity Debugging and Console
        /// </summary>
        /// <param name="error">Error to Output</param>
        /// /// <param name="ObjectReference">Object to which the message applies</param>
        public static void Error(string error, UnityEngine.Object ObjectReference = null)
        {
            if (!Error_Logging)
                return;
            string log = "<b>------------------ YUR ERROR ------------------</b>\n"  + error + "\n" + (ObjectReference != null ? "Name: " + ObjectReference.ToString() : "No object provided as reference");
            if (Log_To_File && Application.platform != RuntimePlatform.Android)
            {
                System.IO.Directory.CreateDirectory(Utilities.YUR_Conversions.PathCombine(UnityEngine.Application.dataPath, "../UserData/"));
                System.IO.File.AppendAllText(Utilities.YUR_Conversions.PathCombine(UnityEngine.Application.dataPath, "../UserData/YUR_Log.txt"), log + Environment.NewLine);
            }
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(log);
            if (Editor_Debugging)
            {
                if (ObjectReference != null)
                    Debug.LogError(log, ObjectReference);
                else
                    Debug.LogError(log);
            }
            Console.ResetColor();
        }
    }
}
