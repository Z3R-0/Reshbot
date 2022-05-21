using Discord;
using Discord.WebSocket;
using Reshbot.Config;
using Reshbot.Modules.Events;
using Reshbot.ReshDiscordUtils;

namespace Reshbot {
    public class Bot {
        private readonly DiscordSocketClient _client;

        private readonly Ready _ready;
        private readonly MessageReceived _messageReceived;
        private readonly InteractionCreated _interactionCreated;

        public static DateTime BotStarted;

        public Bot(DiscordSocketClient client, Ready ready, MessageReceived messageReceived, InteractionCreated interactionCreated) {
            _client = client;
            _ready = ready;
            _messageReceived = messageReceived;
            _interactionCreated = interactionCreated;
        }

        public async Task RunAsync() {
            await _client.LoginAsync(TokenType.Bot, config.token);
            await _client.SetGameAsync("vibing");
            await _client.StartAsync();


            _client.Log += Logger.Log;

            _client.Ready += _ready.HandleEventAsync;
            _client.InteractionCreated += _interactionCreated.HandleEventAsync;
            _client.MessageReceived += _messageReceived.HandleEventAsync;

            await Task.Delay(-1); // waits indefinitely
        }
    }
}
