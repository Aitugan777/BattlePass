using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AitukBattlePass.Models
{
    public class AitukServer
    {
        [XmlArray("Players"), XmlArrayItem("Player")]
        public List<AitukPlayer> Players = new List<AitukPlayer>();

        [XmlArray("Tasks"), XmlArrayItem("Task")]
        public List<AitukTaskDB> TasksDB = new List<AitukTaskDB>();

        [XmlArray("Rewards"), XmlArrayItem("Reward")]
        public List<AitukRewardDB> Rewards = new List<AitukRewardDB>();
    }
}
