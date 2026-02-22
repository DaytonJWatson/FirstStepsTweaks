using System;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace FirstStepsTweaks.Services
{
    public class JoinService
    {
        private readonly ICoreServerAPI api;

        private const string FirstJoinKey = "fst_firstjoin";

        public JoinService(ICoreServerAPI api)
        {
            this.api = api;
        }

        public void OnPlayerJoin(IServerPlayer player)
        {
            byte[] data = player.GetModdata(FirstJoinKey);

            if (data == null)
            {
                api.BroadcastMessageToAllGroups(
                    $"Welcome {player.PlayerName} to the server, this is their first time joining!",
                    EnumChatType.AllGroups
                );

                player.SetModdata(FirstJoinKey, new byte[] { 1 });
            }
            else
            {
                api.BroadcastMessageToAllGroups(
                    $"Welcome back {player.PlayerName}!",
                    EnumChatType.AllGroups
                );
            }
        }
    }
}