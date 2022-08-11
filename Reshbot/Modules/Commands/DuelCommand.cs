using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Data.Sqlite;
using Reshbot.ReshDiscordUtils;
using Reshbot.SQLModels;

namespace Reshbot.Modules.Commands {
    public class DuelCommand : BotInteraction<SocketSlashCommand> {
        private DuelDataSystem _duelDataSystem = DuelDataSystem.instance;

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

        [SlashCommand("duel_leaderboard", "see the duel leaderboard")]
        public async Task LeaderboardAsync() {
            EmbedBuilder embedBuilder = DiscordUtilityMethods.GetEmbedBuilder("Reshbot Duel Leaderboard (ordered by # of wins)");

            SqliteDataReader data_reader = _duelDataSystem.GetDuelsWithQuery("SELECT VictorId, COUNT(VictorId) FROM Duels " +
                            "GROUP BY VictorId " +
                            "ORDER BY COUNT(VictorId) DESC" +
                            "LIMIT 15;");

            string result = "";

            while (data_reader.Read()) {
                result += $"\n{Context.Guild.GetUser(ulong.Parse(data_reader.GetString(0)))} --- {data_reader.GetInt32(1)}";
            }

            embedBuilder.WithDescription(result);

            await RespondAsync(embed: embedBuilder.Build());
        }
    }
}
