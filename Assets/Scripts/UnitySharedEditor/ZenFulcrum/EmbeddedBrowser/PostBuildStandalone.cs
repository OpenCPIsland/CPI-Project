using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Runtime.InteropServices;

namespace ZenFulcrum.EmbeddedBrowser
{

    class PostBuildStandalone
    {

        // Platform-specific files for Windows and Linux
        private static readonly List<string> windowsDlls = new List<string>{
            "d3dcompiler_43.dll",
            "d3dcompiler_47.dll",  // Updated to use a more modern compiler version
            "libEGL.dll",
            "libGLESv2.dll",
            "zf_cef.dll",
        };

        private static readonly List<string> linuxLibs = new List<string>{
            "libgcrypt.so.11",  // Example library for Linux
        };

        [PostProcessBuild(10)]
        public static void PostprocessBuild(BuildTarget target, string buildFile)
        {
            if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
            {
                PostprocessWindowsBuild(target, buildFile);
            }
            else if (target == BuildTarget.StandaloneLinux64)
            {
                PostprocessLinuxBuild(target, buildFile);
            }
            else if (target == BuildTarget.StandaloneOSX)
            {
                PostprocessMacBuild(target, buildFile);
            }
        }

        public static void PostprocessWindowsBuild(BuildTarget target, string buildFile)
        {
            Debug.Log("Post processing Windows build: " + buildFile);

            var buildName = Regex.Match(buildFile, @"\/([^\/]+)\.exe$").Groups[1].Value;
            var buildPath = Directory.GetParent(buildFile).FullName;
            var dataPath = Path.Combine(buildPath, buildName + "_Data");

            // Copy required Windows DLLs to root build folder
            foreach (var file in windowsDlls)
            {
                MoveFile(Path.Combine(dataPath, "Plugins", file), Path.Combine(buildPath, file));
            }

            // Commented out CEF resource copying since it’s no longer needed
            //CopyCEFResources(buildPath, dataPath, "w" + (target == BuildTarget.StandaloneWindows64 ? "64" : "32"));
        }

        public static void PostprocessLinuxBuild(BuildTarget target, string buildFile)
        {
            Debug.Log("Post processing Linux build: " + buildFile);

            var buildName = Regex.Match(buildFile, @"\/([^\/]+?)(\.x86(_64)?)?$").Groups[1].Value;
            var buildPath = Directory.GetParent(buildFile).FullName;
            var dataPath = Path.Combine(buildPath, buildName + "_Data");

            // Copy Linux-specific libraries
            foreach (var lib in linuxLibs)
            {
                MoveFile(Path.Combine(ZFFolder, "Plugins", lib), Path.Combine(dataPath, "Plugins", lib));
            }

            // Symlink binary files
            var byBinFiles = new List<string> {
                "natives_blob.bin",
                "snapshot_blob.bin",
                "icudtl.dat"
            };

            foreach (var file in byBinFiles)
            {
                var src = Path.Combine(ZFFolder, "Plugins", file);
                var dest = Path.Combine(dataPath, "Plugins", file);
                MoveFile(src, dest);
            }

            // Commented out CEF resource copying since it’s no longer needed
            //CopyCEFResources(buildPath, dataPath, "l64");
        }

        public static void PostprocessMacBuild(BuildTarget target, string buildFile)
        {
            Debug.Log("Post processing Mac build: " + buildFile);

            var buildPath = buildFile; // Path to the .app file
            var platformPluginsSrc = Path.Combine(ZFFolder, "Plugins", "m64");

            // Commented out the CEF-related file copy
            /*
            CopyDirectory(Path.Combine(platformPluginsSrc, "BrowserLib.app/Contents/Frameworks/Chromium Embedded Framework.framework"),
                Path.Combine(buildPath, "Contents/Frameworks/Chromium Embedded Framework.framework"));
            CopyDirectory(Path.Combine(platformPluginsSrc, "BrowserLib.app/Contents/Frameworks/ZFGameBrowser.app"),
                Path.Combine(buildPath, "Contents/Frameworks/ZFGameBrowser.app"));

            MoveFile(Path.Combine(platformPluginsSrc, "libZFEmbedWeb.dylib"), Path.Combine(buildPath, "Contents/Plugins/libZFEmbedWeb.dylib"));
            */

            // Commented out the browser asset writing
            //WriteBrowserAssets(Path.Combine(buildPath, "Contents", StandaloneWebResources.DefaultPath));
        }

        // Commented out CopyCEFResources method, in case you need it in the future
        /*
        private static void CopyCEFResources(string buildPath, string dataPath, string platformFolder)
        {
            var platformPluginsSrc = Path.Combine(ZFFolder, "Plugins", platformFolder);

            // Copy CEF blobs
            MoveFile(Path.Combine(platformPluginsSrc, "natives_blob.bin"), Path.Combine(dataPath, "Plugins", "natives_blob.bin"));
            MoveFile(Path.Combine(platformPluginsSrc, "snapshot_blob.bin"), Path.Combine(dataPath, "Plugins", "snapshot_blob.bin"));

            // Copy icudtl.dat
            MoveFile(Path.Combine(platformPluginsSrc, "icudtl.dat"), Path.Combine(buildPath, "icudtl.dat"));

            // Copy resources from CEFResources
            var resSrcDir = Path.Combine(ZFFolder, "Plugins", "CEFResources");
            foreach (var file in Directory.GetFiles(resSrcDir))
            {
                if (!file.EndsWith(".meta"))
                {
                    MoveFile(file, Path.Combine(dataPath, "Plugins", Path.GetFileName(file)));
                }
            }

            // Copy locales
            var localesSrcDir = Path.Combine(resSrcDir, "locales");
            var localesDestDir = Path.Combine(dataPath, "Plugins", "locales");
            Directory.CreateDirectory(localesDestDir);
            foreach (var file in Directory.GetFiles(localesSrcDir))
            {
                if (!file.EndsWith(".meta"))
                {
                    MoveFile(file, Path.Combine(localesDestDir, Path.GetFileName(file)));
                }
            }

            WriteBrowserAssets(Path.Combine(dataPath, StandaloneWebResources.DefaultPath));
        }
        */

        private static void MoveFile(string src, string dest)
        {
            if (File.Exists(dest))
            {
                File.Delete(dest);
            }
            if (File.Exists(src))
            {
                File.Move(src, dest);
            }
        }

        private static void CopyDirectory(string src, string dest)
        {
            foreach (var dir in Directory.GetDirectories(src, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dir.Replace(src, dest));
            }

            foreach (var file in Directory.GetFiles(src, "*", SearchOption.AllDirectories))
            {
                if (!file.EndsWith(".meta"))
                {
                    File.Copy(file, file.Replace(src, dest), true);
                }
            }
        }

        private static string ZFFolder
        {
            get
            {
                var path = new System.Diagnostics.StackTrace(true).GetFrame(0).GetFileName();
                path = Directory.GetParent(path).Parent.Parent.FullName;
                return path;
            }
        }

        // Commented out WriteBrowserAssets method, in case you need it in the future
        /*
        private static void WriteBrowserAssets(string path)
        {
            var htmlDir = Application.dataPath + "/../BrowserAssets";
            var allData = new Dictionary<string, byte[]>();

            if (Directory.Exists(htmlDir))
            {
                foreach (var file in Directory.GetFiles(htmlDir, "*", SearchOption.AllDirectories))
                {
                    var localPath = file.Substring(htmlDir.Length).Replace("\\", "/");
                    allData[localPath] = File.ReadAllBytes(file);
                }
            }

            var wr = new StandaloneWebResources(path);
            wr.WriteData(allData);
        }
        */
    }
}
