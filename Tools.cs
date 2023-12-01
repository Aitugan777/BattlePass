
using AitukBattlePass.Models;
using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering;

namespace AitukBattlePass
{
    public static class Tools
    {
        public static void GiveReward(this UnturnedPlayer uplayer, AitukLevel aitukLevel)
        {
            foreach (AitukReward aitukReward in aitukLevel.Rewards)
            {
                if ((uplayer.HasPermission(aitukReward.HasPermission) || aitukReward.HasPermission.ToLower() == "default") && !uplayer.isGivedReward(aitukLevel, aitukReward))
                {

                    uplayer.GiveVehicle(aitukReward.VehicleId);

                    foreach (AitukItem aitukItem in aitukReward.Items)
                    {
                        uplayer.GiveItem(aitukItem.Id, aitukItem.Amount);
                    }

                    foreach (string cmds in aitukReward.Commands)
                    {
                        string cmd = cmds;
                        if (cmds.Contains("AITUKSTEAMID"))
                            cmd.Replace("AITUKSTEAMID", uplayer.CSteamID.ToString());

                        R.Commands.Execute(new ConsolePlayer(), cmd);
                    }

                    uplayer.GetRewardDB(aitukLevel.LVL).PermissionGives.Add(aitukReward.HasPermission);
                    uplayer.OpenUI();
                }
            }
            DBManager.Save(Plugin.AitukServer);
        }

        public static bool isGivedReward(this UnturnedPlayer uplayer, AitukLevel aitukLevel, AitukReward aitukReward)
        {
            AitukRewardDB aitukRewardDB = uplayer.GetRewardDB(aitukLevel.LVL);

            foreach (string prm in aitukRewardDB.PermissionGives)
            {
                if (prm == aitukReward.HasPermission)
                    return true;
            }
            return false;
        }

        public static bool isBlockedReward(this UnturnedPlayer uplayer, AitukLevel aitukLevel)
        {
            if (uplayer.GetRewardDB(aitukLevel.LVL).PermissionGives.Count == 0)
            {
                return false;
            }

            int i = 0;
            int i2 = 0;
            foreach (AitukReward aitukReward in aitukLevel.Rewards)
            {
                if (uplayer.HasPermission(aitukReward.HasPermission) || aitukReward.HasPermission.ToLower() == "default")
                {
                    i++;
                    foreach (string prm in uplayer.GetRewardDB(aitukLevel.LVL).PermissionGives)
                    {
                        if (aitukReward.HasPermission == prm)
                            i2++;
                    }
                }
            }
            if (i2 >= i)
                return true;

            return false;
        }

        public static AitukRewardDB GetRewardDB(this UnturnedPlayer uplayer, int lvl)
        {
            foreach (AitukRewardDB aitukRewardDB in Plugin.AitukServer.Rewards)
            {
                if (aitukRewardDB.CSteamID == uplayer.CSteamID && aitukRewardDB.LVL == lvl)
                    return aitukRewardDB;
            }

            AitukRewardDB aitukReward = new AitukRewardDB() { CSteamID = uplayer.CSteamID, LVL = lvl, PermissionGives = new List<string>() };
            Plugin.AitukServer.Rewards.Add(aitukReward);
            return aitukReward;
        }

        public static AitukPlayer APlayer(this UnturnedPlayer uplayer)
        {
            foreach (AitukPlayer players in Plugin.AitukServer.Players)
            {
                if (players.CSteamID == uplayer.CSteamID)
                    return players;
            }

            AitukPlayer aitukPlayer = new AitukPlayer()
            {
                CSteamID = uplayer.CSteamID,
                LVL = 0,
                Points = 0
            };
            Plugin.AitukServer.Players.Add(aitukPlayer);
            return aitukPlayer;
        }

        public static AitukLevel GetLevel(this UnturnedPlayer uplayer)
        {
            foreach (AitukLevel lvl in Plugin.Instance.Configuration.Instance.Levels)
            {
                if (lvl.LVL == uplayer.APlayer().LVL + 1)
                    return lvl;
            }
            return null;
        }

        public static AitukTaskDB GetTaskDB(this UnturnedPlayer uplayer, ETaskType taskType, int lvl)
        {
            foreach (AitukTaskDB aitukTaskDB in Plugin.AitukServer.TasksDB)
            {
                if (uplayer.CSteamID == aitukTaskDB.CSteamID)
                {
                    if (aitukTaskDB.TaskType == taskType && aitukTaskDB.LVL == lvl)
                        return aitukTaskDB;
                }
            }
            AitukTaskDB aitukTask = new AitukTaskDB()
            {
                CSteamID = uplayer.CSteamID,
                Points = 0,
                TaskType = taskType,
                LVL = lvl
            };
            Plugin.AitukServer.TasksDB.Add(aitukTask);
            return aitukTask;
        }

        public static void ClearTasksDB(this UnturnedPlayer uplayer)
        {
            try
            {
                bool finished = false;
                while (!finished)
                {
                    foreach (AitukTaskDB aitukTaskDB in Plugin.AitukServer.TasksDB)
                    {
                        if (uplayer.CSteamID == aitukTaskDB.CSteamID)
                        {
                            if (aitukTaskDB.LVL != uplayer.APlayer().LVL)
                            {
                                Plugin.AitukServer.TasksDB.Remove(aitukTaskDB);
                            }
                        }
                    }
                    finished = true;
                }

            }
            catch { }
        }

        public static AitukTask GetTask(this AitukLevel level, ETaskType taskType)
        {
            foreach (AitukTask aitukTask in level.Tasks)
            {
                if (aitukTask.TaskType == taskType)
                    return aitukTask;
            }
            return null;
        }

        public static void GivePoint(this UnturnedPlayer uplayer, ETaskType taskType, ushort targetId = 0)
        {
            AitukLevel aitukLevel = uplayer.GetLevel();

            if (aitukLevel == null)
                return;

            AitukTask aitukTask = aitukLevel.GetTask(taskType);

            if (aitukTask == null)
                return;

            if (aitukTask.EquipmentItemID != 0)
            {
                if (uplayer.Player.equipment != null)
                {
                    if (uplayer.Player.equipment.asset != null)
                    {
                        if (uplayer.Player.equipment.asset.id != aitukTask.EquipmentItemID)
                        {
                            return;
                        }
                    }
                    else
                        return;
                }
                else
                    return;
            }

            if (targetId != 0)
            {
                if (aitukTask.TargetItemID != 0)
                {
                    if (aitukTask.TargetItemID != targetId)
                        return;
                }
            }


            AitukTaskDB aitukTaskDB = uplayer.GetTaskDB(taskType, aitukLevel.LVL);
            if (aitukTaskDB == null)
            {
                aitukTaskDB = new AitukTaskDB()
                {
                    CSteamID = uplayer.CSteamID,
                    Points = 0,
                    TaskType = taskType,
                    LVL = aitukLevel.LVL
                };
                Plugin.AitukServer.TasksDB.Add(aitukTaskDB);
            }

            if (aitukTask.ActionCoefficient + aitukTaskDB.Points >= aitukTask.MaxCount)
                aitukTaskDB.Points = aitukTask.MaxCount;
            else
                aitukTaskDB.Points += aitukTask.ActionCoefficient;

            if (aitukLevel.NeedPoints <= uplayer.GetPoints(aitukLevel))
            {
                uplayer.APlayer().LVL++;
                if (uplayer.APlayer().LVL >= Plugin.Instance.Configuration.Instance.Levels[Plugin.Instance.Configuration.Instance.Levels.Count -1].LVL)
                {
                    uplayer.APlayer().LVL = 0;
                }
                UnturnedChat.Say(uplayer, Plugin.Instance.Translate("new_lvl", uplayer.APlayer().LVL));
                uplayer.ClearTasksDB();
            }
        }

        public static float GetPoints(this UnturnedPlayer uplayer, AitukLevel aitukLevel)
        {
            float f = 0f;
            foreach (AitukTaskDB aitukTaskDB in Plugin.AitukServer.TasksDB)
            {
                if (aitukTaskDB.CSteamID == uplayer.CSteamID && aitukTaskDB.LVL == aitukLevel.LVL)
                {
                    f += aitukTaskDB.Points;
                }
            }
            return f;
        }
    }
}
