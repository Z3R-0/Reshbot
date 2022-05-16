using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Reshbot.Modules.Events {
    public class InteractionCreated : IEvent {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactionService;
        private readonly IServiceProvider _serviceProvider;

        public InteractionCreated(DiscordSocketClient client, InteractionService interactionService, IServiceProvider serviceProvider) {
            _client = client;
            _interactionService = interactionService;
            _serviceProvider = serviceProvider;
        }

        public async Task HandleEventAsync(SocketInteraction socketInteraction) {
            var ctx = CreateGeneric(_client, socketInteraction);
            await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
        }

        private static IInteractionContext CreateGeneric(DiscordSocketClient client, SocketInteraction interaction) => interaction switch {
            SocketModal modal => new SocketInteractionContext<SocketModal>(client, modal),
            SocketUserCommand user => new SocketInteractionContext<SocketUserCommand>(client, user),
            SocketSlashCommand slash => new SocketInteractionContext<SocketSlashCommand>(client, slash),
            SocketMessageCommand message => new SocketInteractionContext<SocketMessageCommand>(client, message),
            SocketMessageComponent component => new SocketInteractionContext<SocketMessageComponent>(client, component),
            _ => throw new InvalidOperationException("This interaction type is unsupported! Please report this.")
        };
    }
}
