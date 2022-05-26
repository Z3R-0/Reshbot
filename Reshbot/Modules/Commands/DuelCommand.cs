using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Reshbot.ReshDiscordUtils;

namespace Reshbot.Modules.Commands {
    public class DuelCommand : BotInteraction<SocketSlashCommand> {
        [SlashCommand("duel", "duel command")]
        public async Task DuelAsync(SocketGuildUser user) {

            #region Knockout Criteria
            if (user.IsBot) {
                await RespondAsync("You can't duel bots, we are all-powerful B)", ephemeral: true);
                return;
            }
            if (Context.User.Id == user.Id) {
                await RespondAsync("You cannot duel your own demons though Discord...", ephemeral: true);
                return;
            }
            #endregion

            var yes_btn = new ButtonBuilder {
                Label = "Yes",
                CustomId = "click_yes:" + Context.User.Id, // append the challenger's ID so it can be retrieved in the handler
                Style = ButtonStyle.Success,
            };

            var no_btn = new ButtonBuilder {
                Label = "No",
                CustomId = "click_no:" + Context.User.Id, // append the challenger's ID so it can be retrieved in the handler
                Style = ButtonStyle.Danger,
            };

            await RespondAsync($"{user.Mention}, do you accept the challenge?", components: new ComponentBuilder().WithButton(yes_btn).WithButton(no_btn).Build());
        }
    }
}
