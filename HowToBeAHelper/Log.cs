using System;
using System.IO;
using System.Windows.Forms;

namespace HowToBeAHelper
{
    internal static class Log
    {
        private static readonly string FilePath =
            Path.Combine(Path.GetDirectoryName(Application.ExecutablePath) ?? string.Empty, "log.txt");

        public static void Append(string message)
        {
            try
            {
                File.AppendAllLines(FilePath, new[] { $"[{DateTime.Now:G}] " + message });
            }
            catch
            {
                //ignore
            }
        }
    }
}
