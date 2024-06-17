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


        static string HashFile(string path) {
            byte[] raw_hash;
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = File.OpenRead(path)) {
                    raw_hash = md5.ComputeHash(stream);
                }
            }
            string hash = BitConverter.ToString(raw_hash).Replace("-", "").ToLowerInvariant();
            return hash.Substring(0, 8);
        }
        
        static void UpdateInteropFiles()
        {
            var random = new System.Random();

            var pluginFolder = Path.Combine(@"Assets/", "Plugins");
            var targetDllPrefix = ""; 
            string sourceDllFullPath = "";

            string[] dlls = new string[] {}; 
 
            // Copy plugin
            var targetDllFullPath = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                
                sourceDllFullPath = Path.Combine(SourceDll, $"lib{DllName}.dylib");
                targetDllPrefix = $"{DllName}.{HashFile(sourceDllFullPath)}";
                targetDllFullPath = Path.Combine(pluginFolder, $"lib{targetDllPrefix}.dylib");
                
                dlls = System.IO.Directory.GetFiles(@"Assets/Plugins", "*saigai.*.dylib");
                // remove all dlls
                foreach (var file in dlls) {
                    // exit and keep the dll if it is identical to the planned one to copy
                    if (file == targetDllFullPath) {
                        Debug.Log("No changes to Rust library");
                        return;
                    }
                    File.Delete(file);
                    // delete the .meta as well
                    File.Delete(file + ".meta");
                }

                Directory.CreateDirectory(pluginFolder);
                File.Copy(sourceDllFullPath, targetDllFullPath);

            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                sourceDllFullPath = Path.Combine(SourceDll, $"{DllName}.dll");
                targetDllPrefix = $"{DllName}.{HashFile(sourceDllFullPath)}";   
                targetDllFullPath = Path.Combine(pluginFolder, $"{targetDllPrefix}.dll");

                dlls = System.IO.Directory.GetFiles(@"Assets/Plugins", "*saigai.*.dll");
                // remove all dlls
                foreach (var file in dlls) {
                    // exit and keep the dll if it is identical to the planned one to copy
                    if (file == targetDllFullPath) {
                        Debug.Log("No changes to Rust library");
                        return;
                    }
                    File.Delete(file);
                    // delete the .meta as well
                    File.Delete(file + ".meta");
                }

                Directory.CreateDirectory(pluginFolder);
                File.Copy(sourceDllFullPath, targetDllFullPath);

            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                sourceDllFullPath = Path.Combine(SourceDll, $"lib{DllName}.so");
                targetDllPrefix = $"{DllName}.{HashFile(sourceDllFullPath)}";   
                targetDllFullPath = Path.Combine(pluginFolder, $"lib{targetDllPrefix}.so");
                
                dlls = System.IO.Directory.GetFiles(@"Assets/Plugins", "*saigai.*.so");
                // remove all dlls
                foreach (var file in dlls) {
                    // exit and keep the dll if it is identical to the planned one to copy
                    if (file == targetDllFullPath) {
                        Debug.Log("No changes to Rust library");
                        return;
                    }
                    File.Delete(file);
                    // delete the .meta as well
                    File.Delete(file + ".meta");
                }

                Directory.CreateDirectory(pluginFolder);
                File.Copy(sourceDllFullPath, targetDllFullPath);
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

#if UNITY_EDITOR
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
#endif
    }
}
