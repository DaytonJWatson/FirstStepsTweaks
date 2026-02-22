using System.Collections.Generic;

namespace FirstStepsTweaks.Discord
{
    public class DiscordBridgeConfig
    {
        // REQUIRED
        public string BotToken { get; set; } = "";
        public string ChannelId { get; set; } = "";
        public string WebhookUrl { get; set; } = "";

        // Optional behavior
        public int PollMs { get; set; } = 5000;                 // 5000–10000 recommended
        public string DiscordPrefix { get; set; } = "[Discord]"; // shown in-game
        public bool RelayGameToDiscord { get; set; } = true;
        public bool RelayDiscordToGame { get; set; } = true;
        public bool RelayJoinLeave = true;

        // Optional filters
        public bool IgnoreEmptyDiscordMessages { get; set; } = true;
        public List<string> IgnoreDiscordPrefixes { get; set; } = new List<string> { "!" }; // ignore bot commands
        public List<string> IgnoreGamePrefixes { get; set; } = new List<string> { "/" };    // ignore commands typed in-game
    }
}