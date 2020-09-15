using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HowToBeAHelper
{
    /// <summary>
    /// A helper class which offers loading and writing of TOML files.
    /// </summary>
    public static class Toml
    {
        /// <summary>
        /// Writes the object to the TOML file on the given path.
        /// </summary>
        /// <param name="path">The destination path</param>
        /// <param name="content">The object which will be saved</param>
        public static void Write(string path, object content)
        {
            Nett.Toml.WriteFile(content, path);
        }

        /// <summary>
        /// Reads the TOML file at the given path and converts it to the type.
        /// </summary>
        /// <typeparam name="T">The output type</typeparam>
        /// <param name="path">The TOML file</param>
        public static T Read<T>(string path)
        {
            return Nett.Toml.ReadFile<T>(path);
        }
    }
}
