using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AitukBattlePass.Models
{
    public class AitukPlayer
    {
        public CSteamID CSteamID { get; set; }

        public int LVL { get; set; }

        public int Points { get; set; }
    }
}
