using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AitukBattlePass.Models
{
    public class AitukTaskDB
    {
        public CSteamID CSteamID { get; set; }

        public int LVL { get; set; }

        public ETaskType TaskType { get; set; }
        
        public float Points { get; set; }
    }
}
