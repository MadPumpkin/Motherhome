using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

#if WINDOWS
using Newtonsoft.Json;
using System.Timers;
#endif

namespace Legend
{
    static class WebServiceClient
    {
        const string serviceAddress = "http://play.legendstudio.com/Service";
        const int reportLimit = 20;
        static int reportCount = 0;

        internal static void Report(string message, string stack, LogType type)
        {
            reportCount++;
            if (reportCount > reportLimit)
                return;

            //Debug.Log("Report Crash");
            var errorMessage = new StringBuilder();

            errorMessage.AppendLine(Level.GameName + " " + Utility.Version + (Level.Modded ? " Modded" : ""));

            errorMessage.AppendLine();
            errorMessage.AppendLine(message);
            errorMessage.AppendLine(stack);
            errorMessage.AppendLine(type.ToString());

            //if (exception.InnerException != null) {
            //    errorMessage.Append("\n\n ***INNER EXCEPTION*** \n");
            //    errorMessage.Append(exception.InnerException.ToString());
            //}

            errorMessage.AppendLine();

            errorMessage.AppendFormat("{0}: {1}\n", "CheatsEnabled", Level.CheatsEnabled);
            errorMessage.AppendFormat("{0}: {1}\n", "Current Level", Level.Instance.LevelNumber);

            errorMessage.AppendLine();

            errorMessage.AppendLine("PLAYERS");
            errorMessage.Append(Player.Instance.ToString());
            errorMessage.AppendLine();

            errorMessage.AppendLine("CONSOLE LOG");
            foreach (var text in Utility.ConsoleLog)
            {
                errorMessage.AppendLine(text);
            }
            errorMessage.AppendLine();

            errorMessage.AppendLine("SYSTEM INFO");
            errorMessage.AppendFormat
            (
                "{0} {1} {2} {3}\n{4}, {5}, {6}, {7}x {8}\n{9}x{10} {11}dpi FullScreen {12}, {13}, {14} vmem: {15} Max Texture: {16}\nScene {17}, Unity Version {18}",
                SystemInfo.deviceModel,
                SystemInfo.deviceName,
                SystemInfo.deviceType,
                SystemInfo.deviceUniqueIdentifier,

                SystemInfo.operatingSystem,
                "Language",//I2.Loc.LocalizationManager.CurrentLanguage, // language
                SystemInfo.systemMemorySize,
                SystemInfo.processorCount,
                SystemInfo.processorType,

                Screen.currentResolution.width,
                Screen.currentResolution.height,
                Screen.dpi,
                Screen.fullScreen,
                SystemInfo.graphicsDeviceName,
                SystemInfo.graphicsDeviceVendor,
                SystemInfo.graphicsMemorySize,
                SystemInfo.maxTextureSize,

                SceneManager.GetActiveScene().name,
                Application.unityVersion
            );

            errorMessage.AppendFormat("\n\n{0}: {1}", "Account Link", Level.AccountLink);

            try
            {
                using (var client = new WebClient())
                {
                    var arguments = new NameValueCollection();
                    //if (loginResult != null)
                    //    arguments.Add("SessionId", loginResult.SessionId.ToString());
                    arguments.Add("report", errorMessage.ToString());
                    var result = Encoding.ASCII.GetString(client.UploadValues(serviceAddress + "/ReportCrash", arguments));
                    Debug.Log(result);
                }
            }
            catch (WebException e)
            {
                Debug.Log("Report Crash: " + e.ToString());
            }
        }

        internal static void RunComplete(string reportText)
        {
            if (Level.Modded || Level.Mods.Count > 0)
                return;

            try
            {
                using (var client = new WebClient())
                {
                    var arguments = new NameValueCollection();
                    //if (loginResult != null)
                    //    arguments.Add("SessionId", loginResult.SessionId.ToString());
                    arguments.Add("report", reportText);
                    arguments.Add("accountLink", Level.AccountLink);
                    var result = Encoding.ASCII.GetString(client.UploadValues(serviceAddress + "/LucaRun", arguments));
                    Debug.Log(result);
                }
            }
            catch (WebException e)
            {
                Debug.Log("Report Crash: " + e.ToString());
            }
        }
    }
}
