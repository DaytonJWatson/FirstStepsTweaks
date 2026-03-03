using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace FirstStepsTweaks.Commands
{
    public class DebugCommands
    {
        private ICoreServerAPI api;

        public DebugCommands(ICoreServerAPI api)
        {
            this.api = api;
        }

        public static void Register(ICoreServerAPI api)
        {
            api.ChatCommands
                .Create("fsdebug")
                .WithDescription("Debug command for First Steps dev")
                .RequiresPlayer()
                .RequiresPrivilege(Privilege.controlserver)
                .HandleWith(args => fsDebug(api, args));
        }
        private static TextCommandResult fsDebug(ICoreServerAPI api, TextCommandCallingArgs args)
        {
            return TextCommandResult.Success("Ran debug command");
        }
    }
}