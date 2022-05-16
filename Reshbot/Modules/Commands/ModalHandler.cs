using Discord.Interactions;
using Discord.WebSocket;
using Reshbot.ReshDiscordUtils;

namespace Reshbot.Modules.Commands {
    public class ModalHandler : BotInteraction<SocketModal> {
        [ModalInteraction("modal_example")]
        public async Task HandleModal(ModalExample modal) {
            await RespondAsync($"{modal.Name} - {modal.Description}");
        }
    }
}
