using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Paps.DevelopmentTools.Editor
{
    public static class TerminalTools
    {
        [MenuItem("Assets/Paps/Open Terminal On Root", priority = -1000)]
        public static void OpenTerminalOnRoot()
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "cd " + Application.dataPath;
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}