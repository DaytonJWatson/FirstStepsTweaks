using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using FirstStepsTweaks.Services;   // Make sure ItemService is here

namespace FirstStepsTweaks.Commands
{
    public static class KitCommands
    {
        private const string StarterKey = "fst_starterclaimed";
        private const string WinterKey = "fst_winterclaimed";

        public static void Register(ICoreServerAPI api)
        {
            api.ChatCommands
                .Create("starterkit")
                .WithDescription("Gives starter items")
                .RequiresPlayer()
                .RequiresPrivilege(Privilege.chat)
                .HandleWith(args => StarterKit(api, args));

            api.ChatCommands
                .Create("winterkit")
                .WithDescription("Gives winter starter kit")
                .RequiresPlayer()
                .RequiresPrivilege(Privilege.chat)
                .HandleWith(args => WinterKit(api, args));
        }

        private static TextCommandResult StarterKit(ICoreServerAPI api, TextCommandCallingArgs args)
        {
            IServerPlayer player = (IServerPlayer)args.Caller.Player;

            if (player.GetModdata(StarterKey) != null)
            {
                player.SendMessage(
                    GlobalConstants.InfoLogChatGroup,
                    "You have already claimed your starter kit.",
                    EnumChatType.CommandError
                );
                return TextCommandResult.Success();
            }

            // Give items
            ItemService.GiveCollectible(api, player, "game:flint", 6);
            ItemService.GiveCollectible(api, player, "game:stick", 6);
            ItemService.GiveCollectible(api, player, "game:drygrass", 1);
            ItemService.GiveCollectible(api, player, "game:firewood", 4);
            ItemService.GiveCollectible(api, player, "game:torch-basic-lit-up", 4);
            ItemService.GiveCollectible(api, player, "game:bread-rye-perfect", 8);

            player.SetModdata(StarterKey, new byte[] { 1 });

            player.SendMessage(
                GlobalConstants.InfoLogChatGroup,
                "You have received your starter kit!",
                EnumChatType.CommandSuccess
            );

            return TextCommandResult.Success();
        }

        private static TextCommandResult WinterKit(ICoreServerAPI api, TextCommandCallingArgs args)
        {
            IServerPlayer player = (IServerPlayer)args.Caller.Player;

            if (player.GetModdata(WinterKey) != null)
            {
                player.SendMessage(
                    GlobalConstants.InfoLogChatGroup,
                    "You have already claimed your winter kit.",
                    EnumChatType.CommandError
                );
                return TextCommandResult.Success();
            }

            ItemService.GiveCollectible(api, player, "game:clothes-upperbodyover-fur-coat", 1);
            ItemService.GiveCollectible(api, player, "game:clothes-foot-knee-high-fur-boots", 1);
            ItemService.GiveCollectible(api, player, "game:clothes-hand-fur-gloves", 1);
            ItemService.GiveCollectible(api, player, "game:redmeat-cooked", 12);

            player.SetModdata(WinterKey, new byte[] { 1 });

            player.SendMessage(
                GlobalConstants.InfoLogChatGroup,
                "You have received your winter kit!",
                EnumChatType.CommandSuccess
            );

            return TextCommandResult.Success();
        }
    }
}