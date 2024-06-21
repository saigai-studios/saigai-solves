using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Interoptopus.Utils
{
    class HotReload
    {
        // Name of the Rust library
        private const string DllName = @"saigai";

        // Location relative to the Unity project root where the DLL is created
        private const string SourceDll = @"Rust/target/debug";

        // Location relative to the Unity project root where the C# sripts are created
        private const string SourceInteropRoot = @"Assets/Scripts/";

        // List of files to copy from the generated Rust bindings to the Unity project
        private static readonly string[] InteropFiles = {
            @"Interop.cs",
        };

        // Location relative to the Unity project root where the C# sripts are copied to
        private const string DestinationAssetFolder = @"Assets/Scripts/";

        // Create the hash of the DLL contents to uniquely identify it.
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

        /// Remove stale DLLs and make sure the latest DLL is copied into the /plugins folder.
        static bool SyncDll(string[] dllsFound, string pluginFolder, string sourceDllFullPath, string targetDllFullPath) {
            bool is_latest = false;
            // Remove all stale dlls
            foreach (var file in dllsFound) {
                // Identify if the DLL is identical to the provided one to copy
                if (file == targetDllFullPath) {
                    Debug.Log("No changes to Rust DLL.");
                    is_latest = true;
                } else {
                    try {
                        File.Delete(file);
                        // delete the .meta as well
                        File.Delete(file + ".meta");
                    } catch (Exception) {
                        Debug.Log("Failed to delete file: " + file);
                    }
                }
            }
            // Copy in the new dll (only if currently does not exist)
            if (is_latest == false) {
                Directory.CreateDirectory(pluginFolder);
                File.Copy(sourceDllFullPath, targetDllFullPath);
            }
            return is_latest;
        }
        
        /// Update the C# bindings files and the DLL to be synchronized with latest changes.
        static void UpdateInteropFiles()
        {
            var pluginFolder = Path.Combine(@"Assets/", "Plugins");
            var targetDllPrefix = ""; 
            string sourceDllFullPath = "";

            bool is_latest = false;

            string[] dllsFound = new string[] {}; 
 
            // Copy plugin
            var targetDllFullPath = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                
                sourceDllFullPath = Path.Combine(SourceDll, $"lib{DllName}.dylib");
                targetDllPrefix = $"{DllName}.{HashFile(sourceDllFullPath)}";
                targetDllFullPath = Path.Combine(pluginFolder, $"lib{targetDllPrefix}.dylib");
                
                dllsFound = System.IO.Directory.GetFiles(pluginFolder, "*saigai.*.dylib");
                // Synchronize the DLL
                is_latest = SyncDll(dllsFound, pluginFolder, sourceDllFullPath, targetDllFullPath);

            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                sourceDllFullPath = Path.Combine(SourceDll, $"{DllName}.dll");
                targetDllPrefix = $"{DllName}.{HashFile(sourceDllFullPath)}";   
                targetDllFullPath = Path.Combine(pluginFolder, $"{targetDllPrefix}.dll");

                dllsFound = System.IO.Directory.GetFiles(pluginFolder, "*saigai.*.dll");
                // Synchronize the DLL
                is_latest = SyncDll(dllsFound, pluginFolder, sourceDllFullPath, targetDllFullPath);

            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                sourceDllFullPath = Path.Combine(SourceDll, $"lib{DllName}.so");
                targetDllPrefix = $"{DllName}.{HashFile(sourceDllFullPath)}";   
                targetDllFullPath = Path.Combine(pluginFolder, $"lib{targetDllPrefix}.so");
                
                dllsFound = System.IO.Directory.GetFiles(pluginFolder, "*saigai.*.so");
                // Synchronize the DLL
                is_latest = SyncDll(dllsFound, pluginFolder, sourceDllFullPath, targetDllFullPath);
            }

            // Copy interop files to destination in Unity.
            foreach (var file in InteropFiles)
            {
                var sourceFile = Path.Combine(SourceInteropRoot, file); 
                var destFile = Path.Combine(DestinationAssetFolder, file);

                var text = File.ReadAllText(sourceFile);
                var newText = text.Replace(DllName, targetDllPrefix);
                if (File.Exists(destFile) == true) {
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

        }
        
        [MenuItem("Interoptopus/Hot Reload")]
        static void Init()
        {
            UpdateInteropFiles();
        }
#endif

    }
}
