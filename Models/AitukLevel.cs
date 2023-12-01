using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AitukBattlePass.Models
{
    public class AitukLevel
    {
        public int LVL { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int NeedPoints { get; set; }

        public string URLImageAcive { get; set; }

        public string URLImageBlocked { get; set; }

        public List<AitukTask> Tasks { get; set; }

        public List<AitukReward> Rewards { get; set; }
    }
}
