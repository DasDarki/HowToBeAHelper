using System;
using System.Collections.Generic;
using System.Text;

namespace HowToBeAHelper
{
    /// <summary>
    /// This class contains extension methods for more functionality with the default C# libraries.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns the string name of the enum.
        /// </summary>
        /// <param name="enum">The given enum</param>
        /// <returns>The name of the given enum</returns>
        public static string GetName(this Enum @enum)
        {
            return Enum.GetName(@enum.GetType(), @enum);
        }
    }
}
