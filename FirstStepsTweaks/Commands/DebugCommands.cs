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
            IServerPlayer player = args.Caller.Player as IServerPlayer;
            if (player == null) return TextCommandResult.Error("Player only command");

            Block block = api.World.GetBlock(new AssetLocation("firststepstweaks:gravestone"));
            if (block == null) return TextCommandResult.Error("Gravestone block not found");

            BlockPos pos = player.Entity.Pos.AsBlockPos.Copy();
            pos.Y--; // place at feet

            api.World.BlockAccessor.SetBlock(block.BlockId, pos);
            api.World.BlockAccessor.MarkBlockDirty(pos);

            return TextCommandResult.Success("Ran debug command");
        }

        public static void DebugBlock(IServerPlayer player, BlockSelection blockSel)
        {
            if (player == null) return;

            BlockPos pos = blockSel.Position;

            player.SendMessage(GlobalConstants.GeneralChatGroup, $"Used block at {pos}", EnumChatType.Notification);
        }
    }
}