using FirstStepsTweaks.Services;
using System;
using System.Linq;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace FirstStepsTweaks.Commands
{
    public static class GravestoneCommands
    {
        private const string AdminPrivilege = "firststepstweaks.graveadmin";

        private static GravestoneService gravestoneService;

        public static void Register(ICoreServerAPI api, GravestoneService service)
        {
            gravestoneService = service;

            api.ChatCommands
                .Create("graveadmin")
                .WithDescription("Admin tools for gravestone management")
                .RequiresPlayer()
                .RequiresPrivilege(AdminPrivilege)
                .BeginSubCommand("list")
                    .WithDescription("List active gravestones")
                    .HandleWith(args => List(api, args))
                .EndSubCommand()
                .BeginSubCommand("giveblock")
                    .WithDescription("Give gravestone block item(s) to a player")
                    .WithArgs(
                        api.ChatCommands.Parsers.Word("player"),
                        api.ChatCommands.Parsers.OptionalWord("quantity")
                    )
                    .HandleWith(args => GiveBlock(api, args))
                .EndSubCommand()
                .BeginSubCommand("dupeitems")
                    .WithDescription("Duplicate stored gravestone items to a player without removing the gravestone")
                    .WithArgs(
                        api.ChatCommands.Parsers.Word("graveId"),
                        api.ChatCommands.Parsers.Word("player")
                    )
                    .HandleWith(args => DuplicateItems(api, args))
                .EndSubCommand()
                .BeginSubCommand("restore")
                    .WithDescription("Restore gravestone items to a player and remove the gravestone")
                    .WithArgs(
                        api.ChatCommands.Parsers.Word("graveId"),
                        api.ChatCommands.Parsers.Word("player")
                    )
                    .HandleWith(args => Restore(api, args))
                .EndSubCommand()
                .BeginSubCommand("remove")
                    .WithDescription("Remove a gravestone by id without restoring items")
                    .WithArgs(api.ChatCommands.Parsers.Word("graveId"))
                    .HandleWith(args => Remove(api, args))
                .EndSubCommand();
        }

        private static TextCommandResult List(ICoreServerAPI api, TextCommandCallingArgs args)
        {
            IServerPlayer caller = args.Caller.Player as IServerPlayer;
            if (caller == null || gravestoneService == null)
            {
                return TextCommandResult.Success();
            }

            var graves = gravestoneService.GetActiveGraves();
            if (graves.Count == 0)
            {
                SendBoth(caller, "No active gravestones found.");
                return TextCommandResult.Success();
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Active gravestones ({graves.Count}):");

            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            foreach (GraveData grave in graves.OrderBy(grave => grave.CreatedUnixMs))
            {
                if (grave == null)
                {
                    continue;
                }

                long ageMinutes = Math.Max(0, (now - grave.CreatedUnixMs) / 60000L);
                string claimState = gravestoneService.IsPubliclyClaimable(grave) ? "public" : "owner-only";

                sb.AppendLine($"- {grave.GraveId} | owner={grave.OwnerName} | pos={grave.Dimension}:{grave.X},{grave.Y},{grave.Z} | age={ageMinutes}m | {claimState}");
            }

            caller.SendMessage(GlobalConstants.InfoLogChatGroup, sb.ToString().TrimEnd(), EnumChatType.Notification);
            return TextCommandResult.Success();
        }

        private static TextCommandResult GiveBlock(ICoreServerAPI api, TextCommandCallingArgs args)
        {
            IServerPlayer caller = args.Caller.Player as IServerPlayer;
            string targetName = args[0] as string;
            string quantityRaw = args[1] as string;

            IServerPlayer target = ResolveOnlinePlayer(api, targetName);
            if (target == null)
            {
                SendBoth(caller, "Target player is not online.");
                return TextCommandResult.Success();
            }

            int quantity = 1;
            if (!string.IsNullOrWhiteSpace(quantityRaw) && (!int.TryParse(quantityRaw, out quantity) || quantity <= 0))
            {
                SendBoth(caller, "Quantity must be a positive whole number.");
                return TextCommandResult.Success();
            }

            ItemService.GiveCollectible(api, target, gravestoneService.GraveBlockCode, quantity);
            SendBoth(caller, $"Gave {quantity} gravestone block item(s) to {target.PlayerName}.");

            if (!string.Equals(caller.PlayerUID, target.PlayerUID, StringComparison.OrdinalIgnoreCase))
            {
                SendBoth(target, $"{caller.PlayerName} gave you {quantity} gravestone block item(s).");
            }

            return TextCommandResult.Success();
        }

        private static TextCommandResult DuplicateItems(ICoreServerAPI api, TextCommandCallingArgs args)
        {
            IServerPlayer caller = args.Caller.Player as IServerPlayer;
            string graveId = args[0] as string;
            string targetName = args[1] as string;

            IServerPlayer target = ResolveOnlinePlayer(api, targetName);
            if (target == null)
            {
                SendBoth(caller, "Target player is not online.");
                return TextCommandResult.Success();
            }

            bool success = gravestoneService.TryDuplicateGraveItemsToPlayer(graveId, target, out string message);
            SendBoth(caller, message);

            if (success && !string.Equals(caller.PlayerUID, target.PlayerUID, StringComparison.OrdinalIgnoreCase))
            {
                SendBoth(target, $"{caller.PlayerName} duplicated gravestone items to your inventory.");
            }

            return TextCommandResult.Success();
        }

        private static TextCommandResult Restore(ICoreServerAPI api, TextCommandCallingArgs args)
        {
            IServerPlayer caller = args.Caller.Player as IServerPlayer;
            string graveId = args[0] as string;
            string targetName = args[1] as string;

            IServerPlayer target = ResolveOnlinePlayer(api, targetName);
            if (target == null)
            {
                SendBoth(caller, "Target player is not online.");
                return TextCommandResult.Success();
            }

            bool success = gravestoneService.TryAdminRestoreGraveToPlayer(graveId, target, out string message);
            SendBoth(caller, message);

            if (success && !string.Equals(caller.PlayerUID, target.PlayerUID, StringComparison.OrdinalIgnoreCase))
            {
                SendBoth(target, $"{caller.PlayerName} restored gravestone items to you.");
            }

            return TextCommandResult.Success();
        }

        private static TextCommandResult Remove(ICoreServerAPI api, TextCommandCallingArgs args)
        {
            IServerPlayer caller = args.Caller.Player as IServerPlayer;
            string graveId = args[0] as string;

            bool success = gravestoneService.TryRemoveGrave(graveId, out string message);
            SendBoth(caller, message);

            return TextCommandResult.Success();
        }

        private static IServerPlayer ResolveOnlinePlayer(ICoreServerAPI api, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return null;
            }

            foreach (IServerPlayer player in api.World.AllOnlinePlayers)
            {
                if (player.PlayerName.Equals(query, StringComparison.OrdinalIgnoreCase)
                    || player.PlayerUID.Equals(query, StringComparison.OrdinalIgnoreCase))
                {
                    return player;
                }
            }

            return null;
        }

        private static void SendBoth(IServerPlayer player, string message)
        {
            if (player == null || string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            player.SendMessage(GlobalConstants.InfoLogChatGroup, message, EnumChatType.CommandSuccess);
            player.SendMessage(GlobalConstants.GeneralChatGroup, message, EnumChatType.Notification);
        }
    }
}
