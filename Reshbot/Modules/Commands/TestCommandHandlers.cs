using Discord.Interactions;
using Discord.WebSocket;
using Reshbot.ReshDiscordUtils;

namespace Reshbot.Modules.Commands {
    public class TestCommandHandlers : BotInteraction<SocketMessageComponent> {
        [ComponentInteraction("click_button")]
        public async Task HandleButton() {
            await Context.Interaction.UpdateAsync(m =>
            {
                m.Content = "I have been clicked!";
                m.Components = null;
            });
            await FollowupAsync($"{Context.User.Username} has clicked the button");
        }

        // if only one option then use `string`, if more options use `string[]`
        [ComponentInteraction("menu_selection")]
        public async Task HandleMenu(string[] chosenOptions) {
            await RespondAsync(chosenOptions.Aggregate("", (current, option) => current + $"{option} - "));
        }
    }
}
