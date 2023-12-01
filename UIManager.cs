using AitukBattlePass.Models;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AitukBattlePass
{
    public static class UIManager
    {
        public static Dictionary<CSteamID, int> indexOpenPage = new Dictionary<CSteamID, int>();

        public static short UIKey = Plugin.Instance.Configuration.Instance.UIKey;
        public static ushort TasksUI = Plugin.Instance.Configuration.Instance.TasksUI;
        public static ushort InfoUI = Plugin.Instance.Configuration.Instance.InfoUI;

        public static List<AitukLevel> GetLevelsPage(this UnturnedPlayer uplayer)
        {
            if (!indexOpenPage.ContainsKey(uplayer.CSteamID))
            {
                indexOpenPage.Add(uplayer.CSteamID, 0);
            }

            int indexPage = indexOpenPage[uplayer.CSteamID];

            if (indexPage * 5 >= Plugin.Instance.Configuration.Instance.Levels.Count)
            {
                return new List<AitukLevel>();
            }

            int count = 5;
            if (Plugin.Instance.Configuration.Instance.Levels.Count <= count + (indexPage * 5))
            {
                count = Plugin.Instance.Configuration.Instance.Levels.Count - (indexPage * 5);
            }

            return Plugin.Instance.Configuration.Instance.Levels.GetRange(indexPage * 5, count);
        }

        public static void OpenInfoUI(this UnturnedPlayer uplayer)
        {
            EffectManager.sendUIEffect(InfoUI, (short)InfoUI, uplayer.CSteamID, true);

            uplayer.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);

            AitukLevel aitukLevel = uplayer.GetLevel();

            EffectManager.sendUIEffectText((short)InfoUI, uplayer.CSteamID, true, "tn", aitukLevel.Name);
            EffectManager.sendUIEffectText((short)InfoUI, uplayer.CSteamID, true, "td", aitukLevel.Description);
            EffectManager.sendUIEffectImageURL((short)InfoUI, uplayer.CSteamID, true, "iicon", aitukLevel.URLImageAcive);

            string str1 = "";
            string str2 = "";
            foreach (AitukTask aitukTask in aitukLevel.Tasks)
            {
                str1 += aitukTask.Name + "\n-----------------------------------";
                str2 += Math.Round(uplayer.GetTaskDB(aitukTask.TaskType, aitukLevel.LVL).Points, 2) + "/" + aitukTask.MaxCount + "\n-----------------";
            }

            EffectManager.sendUIEffectText((short)InfoUI, uplayer.CSteamID, true, "tt1", str1);
            EffectManager.sendUIEffectText((short)InfoUI, uplayer.CSteamID, true, "tt2", str2);

            EffectManager.sendUIEffectText((short)InfoUI, uplayer.CSteamID, true, "ptp", uplayer.GetPercentLevel(aitukLevel) + "%");
            EffectManager.sendUIEffectText((short)InfoUI, uplayer.CSteamID, true, "pt", uplayer.GetPercentString(aitukLevel));
        }

        public static void OpenUI(this UnturnedPlayer uplayer)
        {
            EffectManager.sendUIEffect(TasksUI, UIKey, uplayer.CSteamID, true);

            uplayer.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);
            //uplayer.Player.enablePluginWidgetFlag(EPluginWidgetFlags.sh);

            uplayer.UpdateUI();
        }

        public static void UpdateUI(this UnturnedPlayer uplayer)
        {
            List<AitukLevel> levels = uplayer.GetLevelsPage();

            for (int i = 0; i < 5; i++)
            {
                if (levels.Count >= i + 1)
                {
                    if (uplayer.isActiveLevel(levels[i]) || uplayer.isUsedLevel(levels[i]))
                    {
                        if (uplayer.isFinishedLevel(levels[i]))
                        {
                            EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "ptt" + i, 100 + "%");
                            EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "pt" + i, GetProgressString(100));
                            EffectManager.sendUIEffectImageURL(UIKey, uplayer.CSteamID, true, "bi" + i, "");
                            EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "bit" + i, "");

                            if (uplayer.isBlockedReward(levels[i]))
                            {
                                EffectManager.sendUIEffectImageURL(UIKey, uplayer.CSteamID, true, "bt" + i, "");
                                EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "btt" + i, "");
                            }
                        }
                        else
                        {
                            EffectManager.sendUIEffectImageURL(UIKey, uplayer.CSteamID, true, "bt" + i, "");
                            EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "btt" + i, "");
                        }

                        if (uplayer.isUsedLevel(levels[i]))
                        {
                            EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "ptt" + i, uplayer.GetPercentLevel(levels[i]) + "%");
                            EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "pt" + i, uplayer.GetPercentString(levels[i]));
                        }
                        EffectManager.sendUIEffectImageURL(UIKey, uplayer.CSteamID, true, "ti" + i, levels[i].URLImageAcive);
                    }
                    else
                    {
                        EffectManager.sendUIEffectImageURL(UIKey, uplayer.CSteamID, true, "ti" + i, levels[i].URLImageBlocked);
                        EffectManager.sendUIEffectImageURL(UIKey, uplayer.CSteamID, true, "bt" + i, "");
                        EffectManager.sendUIEffectImageURL(UIKey, uplayer.CSteamID, true, "bi" + i, "");
                        EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "btt" + i, "");
                        EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "bit" + i, "");
                        EffectManager.sendUIEffectImageURL(UIKey, uplayer.CSteamID, true, "progressimg" + i, "");
                        EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "pt" + i, "");
                        EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "ptt" + i, "");
                    }
                    EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "nt" + i, levels[i].Name);
                    EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "lt" + i, levels[i].LVL.ToString());
                }
                else
                {
                    EffectManager.sendUIEffectImageURL(UIKey, uplayer.CSteamID, true, "task" + i, "");
                    EffectManager.sendUIEffectImageURL(UIKey, uplayer.CSteamID, true, "ti" + i, "");
                    EffectManager.sendUIEffectImageURL(UIKey, uplayer.CSteamID, true, "bt" + i, "");
                    EffectManager.sendUIEffectImageURL(UIKey, uplayer.CSteamID, true, "bi" + i, "");
                    EffectManager.sendUIEffectImageURL(UIKey, uplayer.CSteamID, true, "progressimg" + i, "");
                    EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "btt" + i, "");
                    EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "bit" + i, "");
                    EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "ti" + i, "");
                    EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "pt" + i, "");
                    EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "ptt" + i, "");
                    EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "nt" + i, "");
                    EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "lt" + i, "");
                }
            }

            int listcount = 0;
            int count = Plugin.Instance.Configuration.Instance.Levels.Count;

            while (count > 0)
            {
                count -= 5;
                listcount++;
            }
            if (listcount == 0)
                listcount = 1;


            EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "tlistcount", indexOpenPage[uplayer.CSteamID] + 1 + "/" + listcount);
            EffectManager.sendUIEffectText(UIKey, uplayer.CSteamID, true, "allprogresstxt", uplayer.GetPercentStringAllLevels());
        }

        public static bool isFinishedLevel(this UnturnedPlayer uplayer, AitukLevel aitukLevel)
        {
            return aitukLevel.LVL <= uplayer.APlayer().LVL;
        }

        public static bool isActiveLevel(this UnturnedPlayer uplayer, AitukLevel aitukLevel)
        {
            return uplayer.APlayer().LVL >= aitukLevel.LVL;
        }
        
        public static bool isUsedLevel(this UnturnedPlayer uplayer, AitukLevel aitukLevel)
        {
            return uplayer.APlayer().LVL + 1 == aitukLevel.LVL;
        }

        public static int GetPercentLevel(this UnturnedPlayer uplayer, AitukLevel aitukLevel)
        {
            int percent = (int)Math.Round(uplayer.GetPoints(aitukLevel) / aitukLevel.NeedPoints * 100);

            if (uplayer.GetPoints(aitukLevel) >= aitukLevel.NeedPoints)
                percent = 100;
            
            return percent;
        }

        public static string GetPercentString(this UnturnedPlayer uplayer, AitukLevel aitukLevel)
        {
            return GetProgressString((byte)uplayer.GetPercentLevel(aitukLevel));
        }

        public static string GetPercentStringAllLevels(this UnturnedPlayer uplayer)
        {
            int percent = (int)Math.Round((double)uplayer.APlayer().LVL / Plugin.Instance.Configuration.Instance.Levels.Count * 100);
            return GetProgressString((byte)percent);
        }

        private static string GetProgressString(byte b)
        {
            if (b == 0)
                return " ";
            //b = (byte)Math.Round((double)(b / 2));
            string str = "";
            for (byte i = 0; i < b; i++)
            {
                str += " ";
            }
            return str;
        }
    }
}
