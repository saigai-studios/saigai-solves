using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CLIBuild
{
    public static class Builder
    {
        public static void BuildProjectParse()
        {
            var args = System.Environment.GetCommandLineArgs();
            bool parsePath = false;
            bool parseTarget = false;

            string path = "Builds/win64";
            string exec = ".exe";
            string trgstr = "win64";
            BuildTarget trg = BuildTarget.StandaloneWindows64;

            foreach (string arg in args)
            {
                if(parsePath)
                {
                    path = arg;
                }
                else if (parseTarget)
                {
                    trgstr = arg;
                }
                
                if (arg == "-customBuildPath")
                {
                    parsePath = true;
                }
                else
                {
                    parsePath = false;
                }

                if (arg == "-buildTarget")
                {
                    parseTarget = true;
                }
                else
                {
                    parseTarget = false;
                }
            }

            switch (trgstr)
            {
                case "win64":
                    trg = BuildTarget.StandaloneWindows64;
                    exec = ".exe";
                    break;

                case "osxuniversal":
                    trg = BuildTarget.StandaloneOSX;
                    exec = ".app";
                    break;

                case "linux64":
                    trg = BuildTarget.StandaloneLinux64;
                    exec = "";
                    break;

                default:
                    Debug.Log("Error: Invalid build target (must be \"win64\", \"osxuniversal\", or \"linux64\")");
                    break;
            }

            BuildProject(path+"/SaigaiSolves"+exec, trg);
        }

        public static void BuildProject(string path, BuildTarget buildTarget)
        {
            // string[] all_scenes =
            // {
            //     "Assets/Scenes/StartScreen.unity",
            //     "Assets/Scenes/Credits.unity",
            //     "Assets/Scenes/E Level - Minigame 1.unity",
            //     "Assets/Scenes/E Level - Minigame 2.unity",
            //     "Assets/Scenes/E Level - Minigame 3.unity",
            //     "Assets/Scenes/Gallery.unity",
            //     "Assets/Scenes/Language Selection.unity",
            //     "Assets/Scenes/SceneSelect.unity",
            //     "Assets/Scenes/View3DCard.unity"
            // };

            var all_scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(s => s.path).ToArray();


            var options = new BuildPlayerOptions
            {
                scenes = all_scenes,
                target = buildTarget,
                locationPathName = path,
            };

            BuildPipeline.BuildPlayer(options);
        }
    }
}
