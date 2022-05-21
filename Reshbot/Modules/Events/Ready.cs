using Discord.Interactions;
using Discord.WebSocket;
using Reshbot.Config;
using Reshbot.ReshDiscordUtils;

namespace Reshbot.Modules.Events {
    public class Ready : IEvent {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactionService;

        public Ready(DiscordSocketClient client, InteractionService interactionService) { 
            _client = client;
            _interactionService = interactionService;
        }

        public async Task HandleEventAsync() {
            Logger.Log($"{_client.CurrentUser.Username} was ready in {(DateTime.Now - Bot.BotStarted).Milliseconds}ms");

            _interactionService.Log += Logger.Log;

            RegisterCommand();

            await Task.CompletedTask;
        }

        private async void RegisterCommand() {
#if DEBUG
            await _interactionService.RegisterCommandsToGuildAsync(ulong.Parse(config.DebugGuildID));
#else
            await _interactionService.RegisterCommandsGloballyAsync();
#endif
        }
    }
}
