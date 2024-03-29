﻿using Discord;
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
            if (Context.User.Id != challengedId) {
                await RespondAsync("You are not the intended user for interacting with these buttons", ephemeral: true);
                return;
            }

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

            // Update duel trackers
            DuelCommand.DuelingUsers.Add(challengerId);
            DuelCommand.DuelingUsers.Add(challengedId);
            DuelCommand.DuelRequests.First(duel => duel.ChallengedId == challengedId).HasResponded = true;

            var shoot_btn = new ButtonBuilder {
                Label = "Shoot",
                CustomId = "click_shoot:" + $"{challengerId},{Context.User.Id}", // append the challenger's and challenged's IDs so they can be retrieved in the handler
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
        [ComponentInteraction("click_no:*,*")]
        public async Task HandleNoButton(ulong challengerId, ulong challengedId) {
            if (Context.User.Id != challengedId) {
                await RespondAsync("You are not the intended user for interacting with these buttons", ephemeral: true);
                return;
            }

            SocketGuildUser challenger = Context.Guild.GetUser(challengerId);

            DuelCommand.DuelRequests.First(duel => duel.ChallengedId == challengedId).HasResponded = true;

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
        /// <param name="challengedId">The id of the person who was challenged</param>
        [ComponentInteraction("click_shoot:*,*")]
        public async Task HandleShootButton(ulong challengerId, ulong challengedId) {
            if (Context.User.Id != challengerId && Context.User.Id != challengedId) {
                await RespondAsync("You are not the intended user for interacting with these buttons", ephemeral: true);
                return;
            }

            await Context.Interaction.UpdateAsync(m => {
                m.Content = $"{Context.User.Mention} has won!";
                m.Components = null;
            });

            DuelCommand.DuelingUsers.Remove(challengedId);
            DuelCommand.DuelingUsers.Remove(challengerId);

            IUserMessage msg = await GetOriginalResponseAsync();

            TimeSpan responseTime = DateTime.UtcNow - msg.Timestamp;

            _duelDataSystem.Insert(new Duel(challengerId.ToString(), challengedId.ToString(), Context.User.Id.ToString(), DateTime.UtcNow, responseTime.Milliseconds), Context.Guild.Id.ToString());
        }
    }
}
