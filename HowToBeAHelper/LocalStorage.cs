using System.IO;
using System.Windows.Forms;

namespace HowToBeAHelper
{
    public static class LocalStorage
    {
        private static readonly string DirPath =
            Path.Combine(Path.GetDirectoryName(Application.ExecutablePath) ?? string.Empty, "localstore");

        public static void Init()
        {
            Directory.CreateDirectory(DirPath);
        }

        public static void Write(string name, string data, string suffix = "data")
        {
            try
            {
                File.WriteAllText(Path.Combine(DirPath, name + "." + suffix), data);
            }
            catch
            {
                //TODO: Needs handling
            }
        }

        public static string Read(string name, string suffix = "data")
        {
            try
            {
                return File.ReadAllText(Path.Combine(DirPath, name + "." + suffix));
            }
            catch
            {
                //TODO: Needs handling
                return null;
            }
        }

        public static void Delete(string name, string suffix = "data")
        {
            try
            {
                File.Delete(Path.Combine(DirPath, name + "." + suffix));
            }
            catch
            {
                //TODO: Needs handling
            }
        }
    }
}
