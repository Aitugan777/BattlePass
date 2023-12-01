using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AitukBattlePass.Models
{
    public class AitukRewardDB
    {
        public CSteamID CSteamID { get; set; }

        public List<string> PermissionGives { get; set; }

        //public string PermissionGived { get; set; }

        public int LVL { get; set; }
    }
}
