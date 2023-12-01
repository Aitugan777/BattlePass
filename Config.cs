using AitukBattlePass.Models;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AitukBattlePass
{
    public class Config : IRocketPluginConfiguration, IDefaultable
    {
        public void LoadDefaults()
        {
            this.Levels = new List<AitukLevel>()
            {
                new AitukLevel()
                {
                    Name = "Зомбиленд",
                    Description = "Убей много зомби",
                    LVL = 1,
                    NeedPoints = 2,
                    URLImageAcive = "https://cdn.icon-icons.com/icons2/1394/PNG/512/zombie-pvz01_96778.png",
                    URLImageBlocked = "https://cdn.icon-icons.com/icons2/330/PNG/256/Zombie-icon_35212.png",
                    Tasks = new List<AitukTask>()
                    {
                        new AitukTask()
                        {
                            Name = "Убить зомби",
                            TaskType = ETaskType.TIME_PLAYED,
                            ActionCoefficient = 1,
                            MaxCount = 100,
                            EquipmentItemID = 16,
                            TargetItemID = 363,
                        }
                    },
                    Rewards = new List<AitukReward>()
                    {
                        new AitukReward()
                        {
                            Commands = new List<string>() { "p add AITUKSTEAMID vip", "p add AITUKSTEAMID admin" },
                            Experience = 100,
                            HasPermission = "default",
                            VehicleId = 53,
                            Items = new List<AitukItem>()
                            {
                                new AitukItem() {Id = 16, Amount = 2},
                                new AitukItem() {Id = 4, Amount = 1}
                            }
                        }
                    }
                },
                new AitukLevel()
                {
                    Name = "Зомбиленд",
                    Description = "Убей много зомби",
                    LVL = 2,
                    NeedPoints = 2,
                    URLImageAcive = "https://cdn.icon-icons.com/icons2/1394/PNG/512/zombie-pvz01_96778.png",
                    URLImageBlocked = "https://cdn.icon-icons.com/icons2/330/PNG/256/Zombie-icon_35212.png",
                    Tasks = new List<AitukTask>()
                    {
                        new AitukTask()
                        {
                            Name = "Убить зомби",
                            TaskType = ETaskType.TIME_PLAYED,
                            ActionCoefficient = 1,
                            MaxCount = 100,
                            EquipmentItemID = 16,
                            TargetItemID = 363,
                        }
                    },
                    Rewards = new List<AitukReward>()
                    {
                        new AitukReward()
                        {
                            Commands = new List<string>() { "p add AITUKSTEAMID vip", "p add AITUKSTEAMID admin" },
                            Experience = 100,
                            HasPermission = "default",
                            VehicleId = 53,
                            Items = new List<AitukItem>()
                            {
                                new AitukItem() {Id = 16, Amount = 2},
                                new AitukItem() {Id = 4, Amount = 1}
                            }
                        }
                    }
                }
            };
        }

        public bool LoadWorkshop = true;

        public short UIKey = 9400;

        public ushort TasksUI = 9400;
        public ushort InfoUI = 9401;

        public List<AitukLevel> Levels;
    }
}
