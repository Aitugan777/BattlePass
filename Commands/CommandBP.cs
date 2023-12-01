using Rocket.API;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AitukBattlePass.Commands
{
    public class CommandBP : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "bp";

        public string Help => "bp";

        public string Syntax => "bp";

        public List<string> Aliases => new List<string>() { "bp"};

        public List<string> Permissions => new List<string>() { "bp" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer uplayer = (UnturnedPlayer)caller;

            uplayer.OpenUI();
        }
    }
}
