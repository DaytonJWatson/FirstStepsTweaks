using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace FirstStepsTweaks.Commands
{
    public class DiscordCommands
    {
        public static void Register(ICoreServerAPI api)
        {
            api.ChatCommands
                .Create("discord")
                .WithDescription("Displays the Discord invite")
                .RequiresPlayer()
                .RequiresPrivilege(Privilege.chat)
                .HandleWith(args => Discord(api, args));
        }

        private static TextCommandResult Discord(ICoreServerAPI api, TextCommandCallingArgs args)
        {
            IServerPlayer player = (IServerPlayer)args.Caller.Player;

            player.SendMessage(
                GlobalConstants.GeneralChatGroup,
                "Discord: discord.gg/8SqKaERD6m",
                EnumChatType.AllGroups
                );

            player.SendMessage(
                GlobalConstants.InfoLogChatGroup,
                "Discord: discord.gg/8SqKaERD6m",
                EnumChatType.AllGroups);

            return TextCommandResult.Success();
        }
    }
}
