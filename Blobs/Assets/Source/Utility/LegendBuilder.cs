#if UNITY_EDITOR
using UnityEditor;
using System.Linq;

namespace Legend
{
    public class LegendBuilder
    {
        private static string[] EnabledLevels()
        {
            return (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();
        }

        public static void DevelopmentBuild()
        {
            EditorUserBuildSettings.development = false;
            EditorUserBuildSettings.allowDebugging = true;
            //EditorUserBuildSettings.connectProfiler = true;

            UnityEditor.EditorUserBuildSettings.SetPlatformSettings("Standalone", "CopyPDBFiles", "true");
            EditorUserBuildSettings.SetPlatformSettings(BuildTarget.StandaloneOSX.ToString(), "CopyPDBFiles", "true");
            EditorUserBuildSettings.SetPlatformSettings(BuildTarget.StandaloneLinuxUniversal.ToString(), "CopyPDBFiles", "true");
            EditorUserBuildSettings.SetPlatformSettings(BuildTarget.StandaloneWindows.ToString(), "CopyPDBFiles", "true");

            var options = new BuildPlayerOptions()
            {
                scenes = EnabledLevels(),
                locationPathName = "OSXBuild\\Never Split the Party.exe",
                target = BuildTarget.StandaloneOSX,                
                //options = BuildOptions.Development | BuildOptions.AllowDebugging
            };
            BuildPipeline.BuildPlayer(options);

            options = new BuildPlayerOptions()
            {
                scenes = EnabledLevels(),
                locationPathName = "LinuxBuild\\Never Split the Party.exe",
                target = BuildTarget.StandaloneLinuxUniversal,
                //options = BuildOptions.Development | BuildOptions.AllowDebugging
            };
            BuildPipeline.BuildPlayer(options);

            options = new BuildPlayerOptions() {
                scenes = EnabledLevels(),
                locationPathName = "WinBuild\\Never Split the Party.exe",
                target = BuildTarget.StandaloneWindows,
                //options = BuildOptions.Development | BuildOptions.AllowDebugging
            };
            BuildPipeline.BuildPlayer(options);
        }
    }
}
#endif