using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Reshbot.ReshDiscordUtils;
using Reshbot.SQLModels;

namespace Reshbot.Modules.Commands {
    /* This class is a BotInteraction that handles commands related to duels. */
    public class DuelCommandHandlers : BotInteraction<SocketMessageComponent> {
        private DuelDataSystem _duelDataSystem = DuelDataSystem.instance;

        /// <summary>
        /// The function is called when the user clicks the "Yes" button in the duel challenge message.
        /// It then updates the duel challenge message to say "Duel initiated!", and then sends a
        /// followup message to the channel saying "User has taken on challenger's challenge, Press the
        /// Shoot button as soon as it becomes available, first one to press 'Shoot' wins". Then it
        /// creates a new button with the label "Shoot" and the custom ID
        /// "click_shoot:challengerId,userId" (so that the handler can retrieve the challenger's ID and
        /// the user's ID), and then sends a message to the channel with the button.
        /// </summary>
        /// <param name="challengerId">The ID of the user who challenged the current user.</param>
        [ComponentInteraction("click_yes:*,*")]
        public async Task HandleYesButton(ulong challengerId, ulong challengedId) {
            if (Context.User.Id != challengedId)
                return;

            SocketGuildUser challenger = Context.Guild.GetUser(challengerId);
            SocketGuildUser challenged = Context.Guild.GetUser(challengedId);

            if (challenger == null) {
                await RespondAsync("The provided user/id was invalid", ephemeral: true);
                return;
            }

            await Context.Interaction.UpdateAsync(m => {
                m.Content = "Duel initiated!";
                m.Components = null;
            });

            await FollowupAsync($"{Context.User.Username} has taken on {challenger.DisplayName}'s challenge, Press the Shoot button as soon as it becomes available, first one to press 'Shoot' wins");

            var shoot_btn = new ButtonBuilder {
                Label = "Shoot",
                CustomId = "click_shoot:" + $"{challengerId},{Context.User.Id}", // append the challenger's ID so it can be retrieved in the handler
                Style = ButtonStyle.Primary,
            };

            await Task.Delay(new Random().Next(2, 4) * 1000).ContinueWith(async x => {
                await ReplyAsync("Press the button!", components: new ComponentBuilder().WithButton(shoot_btn).Build());
            });
        }

        /// <summary>
        /// When the user clicks the "No" button, the message is updated to say "Duel denied!" and the
        /// user is called a coward
        /// </summary>
        /// <param name="challengerId">The ID of the user who challenged you.</param>
        [ComponentInteraction("click_no:*")]
        public async Task HandleNoButton(ulong challengerId) {
            SocketGuildUser challenger = Context.Guild.GetUser(challengerId);

            await Context.Interaction.UpdateAsync(m => {
                m.Content = "Duel denied!";
                m.Components = null;
            });

            await FollowupAsync($"{Context.User.Username} is a coward...");
        }

        /// <summary>
        /// When the user clicks the "Shoot" button, the bot will update the message to say that the
        /// user has won, and then insert a new duel into the database
        /// </summary>
        /// <param name="challengerId">The ID of the user who started the duel.</param>
        /// <param name="challengedid">The id of the person who was challenged</param>
        [ComponentInteraction("click_shoot:*,*")]
        public async Task HandleShootButton(ulong challengerId, ulong challengedid) {
            await Context.Interaction.UpdateAsync(m => {
                m.Content = $"{Context.User.Mention} has won!";
                m.Components = null;
            });

            _duelDataSystem.Insert(new Duel(challengerId.ToString(), challengedid.ToString(), Context.User.Id.ToString()), Context.Guild.Id.ToString());
        }
    }
}
