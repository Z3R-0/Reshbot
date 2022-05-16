using Discord.Interactions;
using Discord.WebSocket;

namespace Reshbot.ReshDiscordUtils {
    // This is just an shortcut for `InteractionModuleBase<SocketInteractionContext<T>>`
    public class BotInteraction<TInteraction> : InteractionModuleBase<SocketInteractionContext<TInteraction>> where TInteraction : SocketInteraction {

    }
}
