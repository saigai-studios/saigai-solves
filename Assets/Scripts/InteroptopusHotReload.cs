#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Interoptopus.Utils
{
    // TODO: This needs more love and cleanup.
    // - Make work on any Unity platform (not just Windows)
    // - Copy based on file hash, not random number
    // - Run automatically when file changes
    // - Nicer way to specify paths ...?
    class HotReload
    {
        private const string DllName = @"saigai";
        private const string SourceDll = @"Rust/target/debug";
        private const string SourceInteropRoot = @"Rust/bindings/csharp";
        private static readonly string[] InteropFiles = {
            @"Interop.cs",
        };
        private const string DestinationAssetFolder = @"Assets/Scripts/";

        
        static void UpdateInteropFiles()
        {
            var random = new System.Random();
            var pluginFolder = Path.Combine(@"Assets/", "Plugins");

            var targetDllPrefix = $"{DllName}.{Math.Abs(random.Next())}";

            // Copy plugin
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                var targetDllFullPath = Path.Combine(pluginFolder, $"lib{targetDllPrefix}.dylib");
                Directory.CreateDirectory(pluginFolder);
                File.Copy(Path.Combine(SourceDll, $"lib{DllName}.dylib"), targetDllFullPath);
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                var targetDllFullPath = Path.Combine(pluginFolder, $"{targetDllPrefix}.dll");
                Directory.CreateDirectory(pluginFolder);
                File.Copy(Path.Combine(SourceDll, $"{DllName}.dll"), targetDllFullPath);
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                var targetDllFullPath = Path.Combine(pluginFolder, $"lib{targetDllPrefix}.so");
                Directory.CreateDirectory(pluginFolder);
                File.Copy(Path.Combine(SourceDll, $"lib{DllName}.so"), targetDllFullPath);
            }

            // Copy interop files
            foreach (var file in InteropFiles)
            {
                var sourceFile = Path.Combine(SourceInteropRoot, file); 
                var destFile = Path.Combine(DestinationAssetFolder, file);

                var text = File.ReadAllText(sourceFile);
                var newText = text.Replace(DllName, targetDllPrefix);
                if (File.Exists(destFile)) {
                    File.Delete(destFile);
                }
                File.WriteAllText(destFile, newText);
            }
            
            Debug.Log("Hot reloading successful.");
        }
        
        [InitializeOnLoadMethod]
        static void OnProjectLoadedInEditor()
        {
            // TODO: Check hash and copy automatically 
            // Debug.Log("Project loaded in Unity Editor 2");
        }
        
        [MenuItem("Interoptopus/Hot Reload")]
        static void Init()
        {
            UpdateInteropFiles();
        }
    }
}
#endif
