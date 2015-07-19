using UnityEngine;

using System;
using System.IO;
using System.Collections.Generic;

namespace LunarBuilder
{
    static class CommandLine
    {
        private static readonly string kCustomArgsPrefix = "-customArgs?";
        
        public static IDictionary<string, string> Arguments
        {
            get
            {
                IDictionary<string, string> args = new Dictionary<string, string>();
                
                string argsToken = GetCustomArgToken();
                
                if (!string.IsNullOrEmpty(argsToken))
                {
                    string[] pairs = argsToken.Split('&');
                    foreach (string pair in pairs)
                    {
                        string[] tokens = pair.Split('=');
                        if (tokens.Length != 2)
                        {
                            throw new IOException("Unable to parse custom args: " + argsToken);
                        }
                        
                        string key = tokens[0];
                        string value = tokens[1];
                        args[key] = value;
                    }
                }
                
                return args;
            }
        }
        
        private static string GetCustomArgToken()
        {
            string[] args = Environment.GetCommandLineArgs();
            foreach (string arg in args)
            {
                if (arg.StartsWith(kCustomArgsPrefix))
                {
                    return arg.Substring(kCustomArgsPrefix.Length);
                }
            }
            
            return null;
        }
    }
}