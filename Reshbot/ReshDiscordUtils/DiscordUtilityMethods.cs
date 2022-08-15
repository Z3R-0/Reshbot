using Discord.WebSocket;
using Discord.Interactions;
using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Reshbot;
using Discord;

namespace Reshbot.ReshDiscordUtils {
    public class DiscordUtilityMethods : BotInteraction<SocketSlashCommand>{
        public async Task CountdownAsync(int counts) {
            for (int i = counts; i > 0; i--) {
                await Task.Delay(TimeSpan.FromSeconds(1));
                await RespondAsync($"{i}");
            }
        }

        /// <summary>
        /// Returns a user ID formatted with to allow mentioning that user
        /// </summary>
        /// <param name="message">Message to parse</param>
        public static async Task<SocketGuildUser> GetUserFromMessage(string message, InteractionContext context) {
            try {
                SocketGuildUser user;

                user = (SocketGuildUser)await context.Guild.GetUserAsync(ulong.Parse(Regex.Match(message, "([0-9])\\w+").Value));

                return user;
            } catch (Exception e) {
                Logger.Log(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Returns ID of the user mentioned in this message
        /// </summary>
        /// <param name="message">Message to parse</param>
        public static string GetUserIdFromMessage(string message) {
            return Regex.Match(message, "([0-9])\\w+").Value;
        }

        /// <summary>
        /// Returns a pre-formatted embed with a custom title, the color set to Teal and the Footer to include the creator's name (Spade) and the link to Spade's Shitty Scrapyard
        /// </summary>
        /// <returns>A pre-formatted embed with the color set to Teal and the Footer to include the creator's name (Spade) and the link to Spade's Shitty Scrapyard</returns>
        public static EmbedBuilder GetEmbedBuilder(string title) {
            EmbedBuilder embedBuilder = new EmbedBuilder();

            embedBuilder.WithTitle(title);
            embedBuilder.WithColor(Color.Teal);
            embedBuilder.Footer = new EmbedFooterBuilder().WithText("Reshbot - Created by Spade#7981\nhttps://discord.gg/cXyZZAdpaR");

            return embedBuilder;
        }
    }
}
