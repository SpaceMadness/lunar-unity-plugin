using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace LunarBuilder
{
    public static partial class Builder
    {
        private static readonly string kArgumentAssetList = "assets";
        private static readonly string kArgumentOutputFile = "output";

        private static void ExportUnityPackage()
        {
            IDictionary<string, string> args = CommandLine.Arguments;

            string outputFile = GetCommandLineArg(args, kArgumentOutputFile);
            string[] assetList = GetCommandLineArray(args, kArgumentAssetList);

            DirectoryInfo outputDirectory = Directory.GetParent(outputFile);
            outputDirectory.Create();
            if (!outputDirectory.Exists)
            {
                throw new IOException("Can't create output directory: " + outputDirectory.FullName);
            }

            string projectDir = Directory.GetParent(Application.dataPath).FullName;

            Debug.Log("Checkings assets...");
            foreach (string asset in assetList)
            {
                string assetPath = Path.Combine(projectDir, asset);
                if (!File.Exists(assetPath) && !Directory.Exists(assetPath))
                {
                    throw new IOException("Asset does not exist: " + asset);
                }
            }

            Debug.Log("Exporting assets...");
            AssetDatabase.ExportPackage(assetList, outputFile);

            Debug.Log("Package written: " + outputFile);
        }

        private static string GetCommandLineArg(IDictionary<string, string> args, string key)
        {
            string value;
            if (!args.TryGetValue(key, out value))
            {
                throw new IOException("Missing command line argument: '" + key + "'");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new IOException("Command line argument is empty: '" + key + "'");
            }

            return value;
        }

        private static string[] GetCommandLineArray(IDictionary<string, string> args, string key, char delim = ',')
        {
            string value = GetCommandLineArg(args, key);
            return value.Split(delim);
        }
    }
}