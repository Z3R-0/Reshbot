using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Data.Sqlite;
using Reshbot.ReshDiscordUtils;
using Reshbot.SQLModels;
using System.Collections;

namespace Reshbot.Modules.Commands {
    public class DuelRequest {
        public ulong ChallengerId;
        public ulong ChallengedId;
        public DateTime RequestedAt;
        public IUserMessage OriginalMessage;
        public bool HasResponded;

        public DuelRequest(ulong challengerId, ulong challengedId, DateTime requestedAt, IUserMessage originalMessage) {
            ChallengerId = challengerId;
            ChallengedId = challengedId;
            RequestedAt = requestedAt;
            OriginalMessage = originalMessage;
        }
    }

    public class DuelCommand : BotInteraction<SocketSlashCommand> {
        public static List<ulong> DuelingUsers = new List<ulong>();
        public static Queue<DuelRequest> DuelRequests = new Queue<DuelRequest>();

        private DuelDataSystem _duelDataSystem = DuelDataSystem.instance;
        private const int TIMEOUT_MS = 30000;

        /// <summary>
        /// Send a message to the user, and if they click the yes button, duel the challenger.
        /// </summary>
        /// <param name="SocketGuildUser">The user you want to duel</param>
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
            
            if (DuelingUsers.Contains(user.Id)) {
                await RespondAsync("This user is already in a duel...", ephemeral: true);
                return;
            }

            #endregion

            var yes_btn = new ButtonBuilder {
                Label = "Yes",
                CustomId = "click_yes:" + Context.User.Id + "," + user.Id, // append the challenger's and challenged's ID so it can be retrieved in the handler
                Style = ButtonStyle.Success,
            };

            var no_btn = new ButtonBuilder {
                Label = "No",
                CustomId = "click_no:" + Context.User.Id + "," + user.Id, // append the challenger's and challenged's ID so it can be retrieved in the handler
                Style = ButtonStyle.Danger,
            };

            await RespondAsync($"{user.Mention}, do you accept the challenge?", components: new ComponentBuilder().WithButton(yes_btn).WithButton(no_btn).Build());

            DuelRequests.Enqueue(new DuelRequest(Context.User.Id, user.Id, DateTime.Now, await GetOriginalResponseAsync()));

            UpdateQueue();
        }

        private async Task UpdateQueue() {
            if (DuelRequests.Peek() == null)
                return;

            TimeSpan timeSpan = DuelRequests.Peek().RequestedAt.AddMilliseconds(TIMEOUT_MS) - DateTime.Now;

            await Task.Delay(timeSpan.Milliseconds + TIMEOUT_MS);

            if (!DuelRequests.Peek().HasResponded) {
                await DuelRequests.Peek().OriginalMessage.ModifyAsync(msg => {
                    msg.Components = null;
                    msg.Content = "This duel request has expired...";
                });
            }

            DuelRequests.Dequeue();
        }

        [SlashCommand("duel_leaderboard", "see the duel leaderboard")]
        public async Task LeaderboardAsync() {
            EmbedBuilder embedBuilder = DiscordUtilityMethods.GetEmbedBuilder("Reshbot Duel Leaderboard (ordered by # of wins)");

            Leaderboard leaderboard = _duelDataSystem.GetLeaderboard(Context.Guild.Id.ToString());
            if (leaderboard.Rows.Count != 0)
                embedBuilder.WithDescription(leaderboard.ToString());
            else
                embedBuilder.WithDescription("No duels recorded yet on this server");


            await RespondAsync(embed: embedBuilder.Build());
        }
    }
}
