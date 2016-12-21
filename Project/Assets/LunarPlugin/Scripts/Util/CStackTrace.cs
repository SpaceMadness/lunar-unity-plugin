//
//  CStackTrace.cs
//
//  Lunar Plugin for Unity: a command line solution for your game.
//  https://github.com/SpaceMadness/lunar-unity-plugin
//
//  Copyright 2016 Alex Lementuev, SpaceMadness.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

﻿using System;
using UnityEngine;

namespace LunarPluginInternal
{
    static class CStackTrace
    {
        #if TRIM_STACK_TRACE
        private static readonly string kMarkerToken = " (at ";
        #endif

        public static string ExtractStackTrace(int numFrames = 0)
        {
            return ExtractStackTrace(StackTraceUtility.ExtractStackTrace(), numFrames);
        }

        public static string ExtractStackTrace(string stackTrace, int numFrames = 0)
        {
            if (stackTrace != null)
            {
                int startPos = 0;
                for (int frameIndex = 0; frameIndex < numFrames && startPos < stackTrace.Length; ++frameIndex)
                {
                    int pos = stackTrace.IndexOf('\n', startPos);
                    if (pos == -1)
                    {
                        return "";
                    }

                    startPos = pos + 1;
                }

                #if TRIM_STACK_TRACE
                if (startPos < stackTrace.Length)
                {
                    int markerPos;
                    int endPos = startPos;
                    int endOfLine;

                    while (endPos < stackTrace.Length)
                    {
                        markerPos = stackTrace.IndexOf(kMarkerToken, endPos);
                        if (markerPos == -1 || (endOfLine = StringUtils.EndOfLineIndex(stackTrace, endPos + 1)) < markerPos)
                        {
                            break;
                        }

                        endPos = endOfLine;
                    }

                    return stackTrace.Substring(startPos, endPos - startPos);
                }

                return "";
                #else  // TRIM_STACK_TRACE
                return stackTrace.Substring(startPos);
                #endif // TRIM_STACK_TRACE
            }

            return null;
        }
    }
}

