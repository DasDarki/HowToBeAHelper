using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Ionic.Zip;

namespace HowToBeAHelper.Updater
{
    class Program
    {
        private const int SwHide = 0;
        private const string UpdateZip = "update.zip";
        private const string Executable = "HowToBeAHelper.exe";

        static void Main(string[] args)
        {
            try
            {
                ShowWindow(GetConsoleWindow(), SwHide);
                string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
                if (string.IsNullOrEmpty(currentPath)) return;
                string exe = Path.Combine(currentPath, Executable);
                if (!File.Exists(exe)) return;
                string zip = Path.Combine(currentPath, UpdateZip);
                if (!File.Exists(zip)) return;
                Thread.Sleep(2000);
                using (ZipFile zipFile = ZipFile.Read(zip))
                {
                    zipFile.ExtractAll(currentPath, ExtractExistingFileAction.OverwriteSilently);
                }

                Process.Start(exe);
            }
            catch
            {
                //Ignore silently
            }
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
