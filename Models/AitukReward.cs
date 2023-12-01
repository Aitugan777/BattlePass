using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AitukBattlePass.Models
{
    public class AitukReward
    {
        public string HasPermission { get; set; }
        public uint Experience { get; set; }
        public ushort VehicleId { get; set; }
        public List<string> Commands { get; set; }
        public List<AitukItem> Items { get; set; }
    }

    public class AitukItem
    {
        public ushort Id { get; set; }
        public byte Amount { get; set; }
    }
}
