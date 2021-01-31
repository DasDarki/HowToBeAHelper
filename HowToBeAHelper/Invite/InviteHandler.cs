using System;
using System.Buffers.Text;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using WebSocketSharp;

namespace HowToBeAHelper.Invite
{
    internal static class InviteHandler
    {
        private static readonly HttpListener Listener = new HttpListener();
        private static readonly string Uri = "http://localhost:24842/";

        internal static void Start()
        {
            Listener.Prefixes.Add(Uri);
            new Thread(async () =>
                {
                    Listener.Start();
                    while (true)
                    {
                        HttpListenerContext ctx = await Listener.GetContextAsync();
                        HttpListenerRequest req = ctx.Request;
                        HttpListenerResponse resp = ctx.Response;

                        if (req.HttpMethod == "GET")
                        {
                            if (req.QueryString.Contains("payload"))
                            {
                                string invite =
                                    Encoding.UTF8.GetString(Convert.FromBase64String(req.QueryString["payload"]));
                                MainForm.Instance.AfterSessionJoin(invite);
                            }
                        }

                        byte[] data = Encoding.UTF8.GetBytes("done");
                        resp.ContentType = "application/json";
                        resp.ContentEncoding = Encoding.UTF8;
                        resp.ContentLength64 = data.LongLength;
                        await resp.OutputStream.WriteAsync(data, 0, data.Length);
                        resp.Close();
                    }
                })
                {IsBackground = true}.Start();
        }

        internal static bool CheckAndPush(object invite)
        {
            try
            {
                using (WebClient client = new CustomWebClient())
                {
                    if (invite != null)
                        client.QueryString.Add("payload",
                            Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(invite))));
                    client.DownloadString(Uri);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        internal class CustomWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest req = base.GetWebRequest(address);
                req.Timeout = 1000;
                return req;
            }
        }
    }
}
