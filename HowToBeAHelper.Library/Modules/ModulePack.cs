using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SevenZip;

namespace HowToBeAHelper.Modules
{
    /// <summary>
    /// The module pack is for packing modules and offering a simple sharing of them.
    /// </summary>
    [Serializable]
    public class ModulePack
    {
        /// <summary>
        /// The content of the pack.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The optional ruleset of the pack.
        /// </summary>
        public string Ruleset { get; set; }

        /// <summary>
        /// Serializes the pack into a byte array.
        /// </summary>
        /// <param name="pack">The pack to be compiled</param>
        /// <returns>The byte array</returns>
        public static byte[] Serialize(ModulePack pack)
        {
            using MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, pack);
            byte[] array = stream.ToArray();
            return SevenZipHelper.Compress(array);
        }


        /// <summary>
        /// Deserializes the given byte array into a pack object.
        /// </summary>
        /// <param name="bytes">The input bytes</param>
        /// <returns>The module pack or null</returns>
        public static ModulePack Deserialize(byte[] bytes)
        {
            using MemoryStream stream = new MemoryStream(SevenZipHelper.Decompress(bytes)) {Position = 0};
            BinaryFormatter formatter = new BinaryFormatter();
            object obj = formatter.Deserialize(stream);
            return obj as ModulePack;
        }
    }
}
