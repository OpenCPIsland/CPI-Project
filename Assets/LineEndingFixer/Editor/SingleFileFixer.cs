using UnityEngine;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kamgam.LEF
{
    public class SingleFileFixer
    {
        public const char LineFeed = '\n';
        public const char CarriageReturn = '\r';
        public const string CarriageReturnLineFeed = "\r\n";
        public static Regex NewLineRegEx = new Regex("(\r\n|\r|\n)");

        public static void Log(string message, LogLevel logLevel = LogLevel.Log)
        {
            LineEndingFixer.LogMessage(message, logLevel);
        }

        public static bool ChangeLinesEndings(string path, LineEndingBehaviour lineEndingBehaviour, System.Text.Encoding defaultEncoding)
        {
            return FixLineEndings(path, lineEndingBehaviour, defaultEncoding, force: true);
        }

        public static bool FixLineEndings(string path, LineEndingBehaviour lineEndingBehaviour, System.Text.Encoding defaultEncoding, bool force = false)
        {
            try
            {
                // Check if the file contains mixed line breaks.
                string fullPath = GetFullAssetPath(path);
                var text = File.ReadAllText(fullPath, defaultEncoding);

                int nCount, rCount, rnCount;
                bool hasMixedLinesEndings;
                AnalyzeLineEndings(text, out nCount, out rCount, out rnCount, out hasMixedLinesEndings);

                // skip if no mixed line endings are found.
                if (!hasMixedLinesEndings && !force)
                {
                    Log(string.Format("'<b>{0}</b>' has no mixed line endings. Skipping.", path), LogLevel.Message);
                    return true;
                }

                // change and save
                string newLineEnding;
                string newText = changeLineEndings(lineEndingBehaviour, text, nCount, rCount, rnCount, out newLineEnding);
                File.WriteAllText(path, newText);

                // Log
                string newNewLineAsText = newLineEnding.Replace("\r", "\\r").Replace("\n", "\\n");
                Log(
                    string.Format("<color=grey>Changed line endings in </color>'<b>{0}</b>' to <color=yellow>{1}</color>. <color=grey>It contained {2} \\r, {3} \\n and {4} \\r\\n lines.</color>", path, newNewLineAsText, rCount, nCount, rnCount)
                    , LogLevel.Message
                    );

                return true;
            }
            catch (System.Exception e)
            {
                Log("Error: Could access not file! " + e.Message + " Path: '" + path + "'", LogLevel.Error);

                bool isAccessDenied = e.Message.Contains("ccess") && e.Message.Contains("denied");
                if (isAccessDenied)
                {
                    string msg = "Maybe your IDE is locking the file?. Please try to close it or make it unlock the file.";
                    Log(msg, LogLevel.Error);
                }

                return false;
            }
        }

        public static void AnalyzeLineEndings(string text, out int nCount, out int rCount, out int rnCount, out bool hasMixedLinesEndings)
        {
            nCount = text.Count(f => f == LineFeed);
            rCount = text.Count(f => f == CarriageReturn);
            rnCount = 0;
            for (int i = 0; i < text.Length - 1; i++)
            {
                if (text[i] == CarriageReturn && text[i + 1] == LineFeed)
                {
                    rnCount++;
                }
            }

            hasMixedLinesEndings = nCount > 0 && rCount > 0 && (rCount != nCount || rCount != rnCount || nCount != rnCount);

            rCount -= rnCount;
            nCount -= rnCount;
        }

        private static string changeLineEndings(LineEndingBehaviour lineEndingBehaviour, string text, int nCount, int rCount, int rnCount, out string newLineEnding)
        {
            newLineEnding = null;
            switch (lineEndingBehaviour)
            {
                case LineEndingBehaviour.Majority:
                    int max = Mathf.Max(nCount, rCount, rnCount);
                    if (nCount == max)
                        newLineEnding = "\n";
                    else if (rCount == max)
                        newLineEnding = "\n";
                    else
                        newLineEnding = "\r\n";
                    break;

                case LineEndingBehaviour.OSDefault:
                    newLineEnding = System.Environment.NewLine;
                    break;

                case LineEndingBehaviour.Windows:
                    newLineEnding = "\r\n";
                    break;

                case LineEndingBehaviour.Unix:
                    newLineEnding = "\n";
                    break;

                default:
                    newLineEnding = System.Environment.NewLine;
                    break;
            }

            if (!string.IsNullOrEmpty(newLineEnding))
            {
                return replaceLineEndings(text, newLineEnding);
            }
            else
                return text;
        }

        public static string replaceLineEndings(string text, string newLineEnding)
        {
            return NewLineRegEx.Replace(text, newLineEnding);
        }

        public static string GetFullAssetPath(string path)
        {
            string projectDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../")).Replace("\\", "/");
            return projectDir + path;
        }
    }
}
