using System;
using System.IO;
using System.Net;

namespace HowToBeAHelper.Net
{
    internal static class Requests
    {
        internal static string Get(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream ?? throw new InvalidOperationException()))
                {
                    return reader.ReadToEnd();
                }
            }
            catch
            {
                return "";
            }
        }
    }
}
