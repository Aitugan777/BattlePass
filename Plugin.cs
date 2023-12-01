using AitukBattlePass.Models;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
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
    public class Plugin : RocketPlugin<Config>
    {
        public static Plugin Instance;
        public static AitukServer AitukServer;

        protected override void Load()
        {
            Instance = this;

            try
            {
                AitukServer = DBManager.GetDB();
            }
            catch
            {
                DBManager.ServializeServer();
            }
            AitukServer = DBManager.GetDB();

            if (base.Configuration.Instance.LoadWorkshop)
                WorkshopDownloadConfig.getOrLoad().File_IDs.Add(3082883493);

            UnturnedPlayerEvents.OnPlayerUpdateStat += OnUpdatedStat;
            UnturnedPlayerEvents.OnPlayerDeath += OnPlayerDeath;
            UnturnedEvents.OnPlayerDamaged += OnPlayerdamage;
            BarricadeManager.onDamageBarricadeRequested += OnBarricadedamage;
            StructureManager.onDamageStructureRequested += OnStructuredamage;
            VehicleManager.onDamageVehicleRequested += OnVehicledamge;
            DamageTool.damageZombieRequested += OnZombieDamage;
            UnturnedPlayerEvents.OnPlayerRevive += OnPlayerRevive;
            EffectManager.onEffectButtonClicked += OnBtnClicked;
            UnturnedPlayerEvents.OnPlayerChatted += OnPlayerChatted;
            UnturnedPlayerEvents.OnPlayerUpdateExperience += OnUpdatedExperience;
            UnturnedPlayerEvents.OnPlayerUpdatePosition += OnUpdatePosition;

            BarricadeManager.OnRepaired += OnRepairBarricade;
            StructureManager.OnRepaired += OnRepairStructure;
            VehicleManager.onRepairVehicleRequested += OnVehicleRepair;
        }

        public Vector3 lastPosition = new Vector3();

        private void OnUpdatePosition(UnturnedPlayer uplayer, Vector3 position)
        {
            if (Vector3.Distance(position, lastPosition) >= 1f)
            {
                lastPosition = position;

                if (uplayer.IsInVehicle && (uplayer.Player.movement.getVehicle().asset.engine == EEngine.PLANE || uplayer.Player.movement.getVehicle().asset.engine == EEngine.HELICOPTER))
                {
                    uplayer.GivePoint(ETaskType.TRAVEL_FLY);
                }
                else if (uplayer.IsInVehicle)
                {
                    uplayer.GivePoint(ETaskType.TRAVEL_VEHICLE);
                }
                else if (!uplayer.IsInVehicle)
                {
                    uplayer.GivePoint(ETaskType.TRAVEL_FOOT);
                }
            }
        }

        private void OnVehicleRepair(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalHealing, ref bool shouldAllow)
        {
            UnturnedPlayer uplayer = UnturnedPlayer.FromCSteamID(instigatorSteamID);
            if (uplayer != null)
                uplayer.GivePoint(ETaskType.REPAIR_VEHICLE, vehicle.id);
        }

        private void OnRepairStructure(CSteamID instigatorSteamID, Transform structureTransform, float totalHealing)
        {
            UnturnedPlayer uplayer = UnturnedPlayer.FromCSteamID(instigatorSteamID);
            if (uplayer != null)
                uplayer.GivePoint(ETaskType.REPAIR_STRUCTURE);
        }

        private void OnRepairBarricade(CSteamID instigatorSteamID, Transform barricadeTransform, float totalHealing)
        {
            UnturnedPlayer uplayer = UnturnedPlayer.FromCSteamID(instigatorSteamID);
            if (uplayer != null)
                uplayer.GivePoint(ETaskType.REPAIR_BARRICADE);
        }

        public void FixedUpdate()
        {
            Timer();
        }

        private DateTime lastCalled = DateTime.Now;

        public void Timer()
        {
            if ((DateTime.Now - this.lastCalled).TotalMinutes > 1.0)
            {
                this.lastCalled = DateTime.Now;
                foreach (SteamPlayer steamPlayer in Provider.clients)
                {
                    UnturnedPlayer uplayer = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                    uplayer.GivePoint(ETaskType.TIME_PLAYED);
                }
            }
        }

        private void OnUpdatedExperience(UnturnedPlayer uplayer, uint experience)
        {
            uplayer.GivePoint(ETaskType.FOUND_EXPERIENCE);
        }

        private void OnPlayerChatted(UnturnedPlayer uplayer, ref Color color, string message, EChatMode chatMode, ref bool cancel)
        {
            uplayer.GivePoint(ETaskType.CHAT_WRITE);
        }

        private void OnPlayerRevive(UnturnedPlayer uplayer, Vector3 position, byte angle)
        {
            uplayer.GivePoint(ETaskType.REVIVE);
        }

        private void OnBtnClicked(Player player, string buttonName)
        {
            UnturnedPlayer uplayer = UnturnedPlayer.FromPlayer(player);

            List<AitukLevel> levels = uplayer.GetLevelsPage();

            for (int i = 0; i < 5; i++)
            {
                if (levels.Count >= i + 1)
                {
                    AitukLevel aitukLevel = levels[i];

                    if (buttonName == "bi" + i)
                    {
                        if (uplayer.isUsedLevel(aitukLevel))
                            uplayer.OpenInfoUI();
                    }
                }
            }

            for (int i = 0; i < 5; i++)
            {
                if (levels.Count >= i + 1)
                {
                    AitukLevel aitukLevel = levels[i];

                    if (buttonName == "bt" + i)
                    {
                        if (uplayer.isFinishedLevel(aitukLevel) && !uplayer.isBlockedReward(aitukLevel))
                            uplayer.GiveReward(aitukLevel);
                    }
                }
            }

            try
            {
                for (int i = 0; i < 10; i++)
                {
                    if (buttonName == "bpart" + i)
                    {
                        if (!UIManager.indexOpenPage.ContainsKey(uplayer.CSteamID))
                        {
                            UIManager.indexOpenPage.Add(uplayer.CSteamID, 0);
                        }

                        if (i * 5 <= base.Configuration.Instance.Levels.Count)
                        {
                            UIManager.indexOpenPage[uplayer.CSteamID] = i;
                            uplayer.OpenUI();
                        }
                    }
                }
            }
            catch { }

            if (buttonName == "bcloseTasks")
            {
                EffectManager.askEffectClearByID(base.Configuration.Instance.TasksUI, uplayer.CSteamID);
                EffectManager.askEffectClearByID(base.Configuration.Instance.InfoUI, uplayer.CSteamID);
                uplayer.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
            }
            if (buttonName == "bcloseTaskInfo")
            {
                EffectManager.askEffectClearByID(base.Configuration.Instance.InfoUI, uplayer.CSteamID);
            }
            try
            {
                if (buttonName == "bnext")
                {
                    if (!UIManager.indexOpenPage.ContainsKey(uplayer.CSteamID))
                    {
                        UIManager.indexOpenPage.Add(uplayer.CSteamID, 0);
                    }
                    int maxCount = 5;
                    if (UIManager.indexOpenPage[uplayer.CSteamID] > 0)
                        maxCount = (UIManager.indexOpenPage[uplayer.CSteamID] + 1) * 5;

                    if (base.Configuration.Instance.Levels.Count <= maxCount)
                    {
                        UnturnedChat.Say(uplayer, Plugin.Instance.Translate("max_count_mess", null));
                        return;
                    }
                    UIManager.indexOpenPage[uplayer.CSteamID]++;
                    uplayer.OpenUI();

                }
                else if (buttonName == "bback")
                {
                    if (!UIManager.indexOpenPage.ContainsKey(uplayer.CSteamID))
                    {
                        UIManager.indexOpenPage.Add(uplayer.CSteamID, 0);
                    }

                    if (UIManager.indexOpenPage[uplayer.CSteamID] == 0)
                    {
                        UnturnedChat.Say(uplayer, Plugin.Instance.Translate("min_count_mess", null));
                        return;
                    }
                    UIManager.indexOpenPage[uplayer.CSteamID]--;
                    uplayer.OpenUI();
                }
            }
            catch { }
        }

        private void OnVehicledamge(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalDamage, ref bool canRepair, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            try
            {
                UnturnedPlayer uplayer = UnturnedPlayer.FromCSteamID(instigatorSteamID);

                uplayer.GivePoint(ETaskType.DAMAGE_VEHICLE, vehicle.id);
            }
            catch { }
        }

        private void OnStructuredamage(CSteamID instigatorSteamID, Transform structureTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            try
            {
                UnturnedPlayer uplayer = UnturnedPlayer.FromCSteamID(instigatorSteamID);



                byte x;
                byte y;

                StructureRegion structureRegion;
                if (!StructureManager.tryGetRegion(structureTransform, out x, out y, out structureRegion))
                {
                    return;
                }

                StructureDrop structureDrop = structureRegion.FindStructureByRootTransform(structureTransform);

                if (pendingTotalDamage >= structureDrop.GetServersideData().structure.health)
                {
                    uplayer.GivePoint(ETaskType.DESTROY_STRUCTURE, structureDrop.GetServersideData().structure.id);
                }

                uplayer.GivePoint(ETaskType.DAMAGE_STRUCTURE, structureDrop.GetServersideData().structure.id);
            }
            catch { }
        }

        private void OnZombieDamage(ref DamageZombieParameters parameters, ref bool shouldAllow)
        {
            try
            {
                UnturnedPlayer uplayer = UnturnedPlayer.FromPlayer(parameters.instigator as Player);
                uplayer.GivePoint(ETaskType.DAMAGE_ZOMBIE);

                if (Mathf.Min(65535, Mathf.FloorToInt(parameters.damage * parameters.times)) >= parameters.zombie.GetHealth())
                {
                    switch (parameters.zombie.speciality)
                    {
                        case EZombieSpeciality.CRAWLER:
                            uplayer.GivePoint(ETaskType.KILL_ZOMBIE_CRAWLER);
                            break;
                        case EZombieSpeciality.SPRINTER:
                            uplayer.GivePoint(ETaskType.KILL_ZOMBIE_SPRINTER);
                            break;
                        case EZombieSpeciality.FLANKER_FRIENDLY:
                            uplayer.GivePoint(ETaskType.KILL_ZOMBIE_FLANKER_FRIENDLY);
                            break;
                        case EZombieSpeciality.FLANKER_STALK:
                            uplayer.GivePoint(ETaskType.KILL_ZOMBIE_FLANKER_STALK);
                            break;
                        case EZombieSpeciality.BURNER:
                            uplayer.GivePoint(ETaskType.KILL_ZOMBIE_BURNER);
                            break;
                        case EZombieSpeciality.ACID:
                            uplayer.GivePoint(ETaskType.KILL_ZOMBIE_ACID);
                            break;
                        case EZombieSpeciality.BOSS_ELECTRIC:
                            uplayer.GivePoint(ETaskType.KILL_ZOMBIE_BOSS_ELECTRIC);
                            break;
                        case EZombieSpeciality.BOSS_WIND:
                            uplayer.GivePoint(ETaskType.KILL_ZOMBIE_BOSS_WIND);
                            break;
                        case EZombieSpeciality.BOSS_FIRE:
                            uplayer.GivePoint(ETaskType.KILL_ZOMBIE_BOSS_FIRE);
                            break;
                        case EZombieSpeciality.BOSS_ALL:
                            uplayer.GivePoint(ETaskType.KILL_ZOMBIE_BOSS_ALL);
                            break;
                        case EZombieSpeciality.BOSS_MAGMA:
                            uplayer.GivePoint(ETaskType.KILL_ZOMBIE_BOSS_MAGMA);
                            break;
                        case EZombieSpeciality.SPIRIT:
                            uplayer.GivePoint(ETaskType.KILL_ZOMBIE_SPIRIT);
                            break;
                        case EZombieSpeciality.BOSS_SPIRIT:
                            uplayer.GivePoint(ETaskType.KILL_ZOMBIE_BOSS_SPIRIT);
                            break;
                        case EZombieSpeciality.BOSS_NUCLEAR:
                            uplayer.GivePoint(ETaskType.KILL_ZOMBIE_BOSS_NUCLEAR);
                            break;
                        case EZombieSpeciality.DL_RED_VOLATILE:
                            uplayer.GivePoint(ETaskType.KILL_ZOMBIE_DL_RED_VOLATILE);
                            break;
                        case EZombieSpeciality.DL_BLUE_VOLATILE:
                            uplayer.GivePoint(ETaskType.KILL_ZOMBIE_DL_BLUE_VOLATILE);
                            break;
                        case EZombieSpeciality.BOSS_ELVER_STOMPER:
                            uplayer.GivePoint(ETaskType.KILL_ZOMBIE_BOSS_ELVER_STOMPER);
                            break;
                        case EZombieSpeciality.BOSS_KUWAIT:
                            uplayer.GivePoint(ETaskType.KILL_ZOMBIE_BOSS_KUWAIT);
                            break;
                    }
                }
            }
            catch { }
        }

        private void OnBarricadedamage(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            try
            {
                UnturnedPlayer uplayer = UnturnedPlayer.FromCSteamID(instigatorSteamID);



                byte x;
                byte y;
                ushort plant;
                BarricadeRegion bRegion;
                if (!BarricadeManager.tryGetRegion(barricadeTransform, out x, out y, out plant, out bRegion))
                {
                    return;
                }

                BarricadeDrop bDrop = bRegion.FindBarricadeByRootTransform(barricadeTransform);

                if (pendingTotalDamage >= bDrop.GetServersideData().barricade.health)
                {
                    uplayer.GivePoint(ETaskType.DESTROY_BARRICADE, bDrop.GetServersideData().barricade.id);
                }
                uplayer.GivePoint(ETaskType.DAMAGE_BARRICADE, bDrop.GetServersideData().barricade.id);
            }
            catch { }
        }

        private void OnPlayerdamage(UnturnedPlayer uplayer, ref EDeathCause cause, ref ELimb limb, ref UnturnedPlayer killer, ref UnityEngine.Vector3 direction, ref float damage, ref float times, ref bool canDamage)
        {
            try
            {
                killer.GivePoint(ETaskType.DAMAGE_PLAYER);
            }
            catch { }
        }

        private void OnPlayerDeath(UnturnedPlayer died, EDeathCause cause, ELimb limb, Steamworks.CSteamID murderer)
        {
            try
            {
                if (cause == EDeathCause.INFECTION)
                {
                    died.GivePoint(ETaskType.DIE_VIRUS);
                }
                if (cause == EDeathCause.ZOMBIE)
                {
                    died.GivePoint(ETaskType.DIE_ZOMBIE);
                }
                if (cause == EDeathCause.ANIMAL)
                {
                    died.GivePoint(ETaskType.DIE_ANIMAL);
                }
                if (cause == EDeathCause.BREATH)
                {
                    died.GivePoint(ETaskType.DIE_OXYGEN);
                }
                if (cause == EDeathCause.PUNCH)
                {
                    died.GivePoint(ETaskType.KILL_PLAYER_PUNCH);
                }
                if (cause == EDeathCause.SPIT)
                {
                    died.GivePoint(ETaskType.DIE_ACID);
                }
                if (cause == EDeathCause.MELEE || cause == EDeathCause.LANDMINE || cause == EDeathCause.MISSILE || cause == EDeathCause.GRENADE)
                {
                    died.GivePoint(ETaskType.DIE_BOOM);
                }
                UnturnedPlayer killer = UnturnedPlayer.FromCSteamID(murderer);
                if (killer != null)
                {
                    if (limb == ELimb.SKULL && cause != EDeathCause.SUICIDE)
                    {
                        killer.GivePoint(ETaskType.KILL_PLAYER_HEAD);
                    }
                    if (cause == EDeathCause.ROADKILL)
                    {
                        killer.GivePoint(ETaskType.KILL_PLAYER_ROADKILL);
                    }
                    killer.GivePoint(ETaskType.KILL_PLAYER);
                    died.GivePoint(ETaskType.DIE_PLAYER);
                }
            }
            catch { }
        }

        private void OnUpdatedStat(UnturnedPlayer uplayer, EPlayerStat stat)
        {
            if (stat == EPlayerStat.KILLS_ZOMBIES_NORMAL)
            {
                uplayer.GivePoint(ETaskType.KILL_ZOMBIE);
            }
            if (stat == EPlayerStat.KILLS_ZOMBIES_MEGA)
            {
                uplayer.GivePoint(ETaskType.KILL_ZOMBIE_MEGA);
            }
            if (stat == EPlayerStat.FOUND_FISHES)
            {
                uplayer.GivePoint(ETaskType.FOUND_FISH);
            }
            if (stat == EPlayerStat.FOUND_PLANTS)
            {
                uplayer.GivePoint(ETaskType.FOUND_PLANTS);
            }
            if (stat == EPlayerStat.KILLS_ANIMALS)
            {
                uplayer.GivePoint(ETaskType.KILL_ANIMAL);
            }
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "max_count_mess", "Больше нет страниц" },
            { "min_count_mess", "Больше нет страниц" },
            { "new_lvl", "Вы достигли уровня {0}, заберите приз за предыдущий (/bp)" }
        };

        protected override void Unload()
        {
            DBManager.Save(AitukServer);
        }
    }
}
