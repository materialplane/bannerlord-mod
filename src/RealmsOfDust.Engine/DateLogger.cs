using System;
using System.IO;
using TaleWorlds.CampaignSystem;

namespace RealmsOfDust.Engine {
    public static class DateLogger {
        public static void LogNow(string ctx) {
            try {
                var ct = CampaignTime.Now;
                File.AppendAllText(@"C:\ProgramData\Mount and Blade II Bannerlord\logs\realms_of_dust.log",
                    $"[{ctx}] Year={ct.GetYear}, Season={ct.GetSeasonOfYear}, DayOfSeason={ct.GetDayOfSeason}, ToDays={ct.ToDays}\n");
            } catch (Exception ex) {
                File.AppendAllText(@"C:\ProgramData\Mount and Blade II Bannerlord\logs\realms_of_dust.log", $"[{ctx} ERR] {ex.Message}\n");
            }
        }
    }
}
