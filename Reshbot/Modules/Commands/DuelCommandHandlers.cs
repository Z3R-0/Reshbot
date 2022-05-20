using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Reshbot.ReshDiscordUtils;

namespace Reshbot.Modules.Commands {
    public class DuelCommandHandlers : BotInteraction<SocketMessageComponent> {

        [ComponentInteraction("click_yes:*")]
        public async Task HandleYesButton(ulong challengerId) {
            SocketGuildUser challenger = Context.Guild.GetUser(challengerId);

            await Context.Interaction.UpdateAsync(m => {
                m.Content = "Duel initiated!";
                m.Components = null;
            });

            await FollowupAsync($"{Context.User.Username} has taken on {challenger.DisplayName}'s challenge, Press the Shoot button as soon as it becomes available, first one to press 'Shoot' wins");

            var shoot_btn = new ButtonBuilder {
                Label = "Shoot",
                CustomId = "click_shoot:" + challengerId, // append the challenger's ID so it can be retrieved in the handler
                Style = ButtonStyle.Primary,
            };

            await Task.Delay(new Random().Next(2, 4) * 1000).ContinueWith(async x => {
                await ReplyAsync("Press the button!", components: new ComponentBuilder().WithButton(shoot_btn).Build());
            });
        }

        [ComponentInteraction("click_no:*")]
        public async Task HandleNoButton(ulong challengerId) {
            SocketGuildUser challenger = Context.Guild.GetUser(challengerId);

            await Context.Interaction.UpdateAsync(m => {
                m.Content = "Duel denied!";
                m.Components = null;
            });

            await FollowupAsync($"{Context.User.Username} is a coward...");
        }

        [ComponentInteraction("click_shoot:*")]
        public async Task HandleShootButton(ulong challengerId) {
            await Context.Interaction.UpdateAsync(m => {
                m.Content = $"{Context.Guild.GetUser(challengerId).Mention} has won!";
                m.Components = null;
            });
        }
    }
}
