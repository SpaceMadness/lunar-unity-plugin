using System;
using UnityEngine;

namespace LunarPluginInternal
{
    static class StackTrace
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

