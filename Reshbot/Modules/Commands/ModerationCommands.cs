using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Reshbot.ReshDiscordUtils;

namespace Reshbot.Modules.Commands {
    public class ModerationCommands : BotInteraction<SocketSlashCommand> {

        [SlashCommand("info", "Returns basic info about a user")]
        public async Task InfoAsync(SocketGuildUser user = null) {
            if (user == null) user = (SocketGuildUser?)Context.User;

            EmbedBuilder embedBuilder = DiscordUtilityMethods.GetEmbedBuilder($"{user.Username}#{user.Discriminator}");

            embedBuilder.WithDescription($"ID: {user.Id}\nCreated at: {TimestampTag.FromDateTimeOffset(user.CreatedAt)}\nJoined At: {TimestampTag.FromDateTimeOffset((DateTimeOffset)user.JoinedAt)}\nAvatar: ")
                        .WithImageUrl(user.GetGuildAvatarUrl() ?? user.GetAvatarUrl(size: 4096));

            await RespondAsync(embed: embedBuilder.Build());
        }

        [SlashCommand("help", "Describes some of the commands of this bot")]
        public async Task HelpAsync() {
            EmbedBuilder embedBuilder = DiscordUtilityMethods.GetEmbedBuilder("Reshbot help");

            embedBuilder.WithDescription("Hello there, I am Reshbot and this is what I can currently do:\n\n" +
                             "**info**: *Returns basic information about yourself or another user.* (WIP)\n" +
                             "**duel**: *Starts a duel against the mentioned user.*\n" +
                             "**giveaway**: *Coming Soon.*");

            await ReplyAsync(embed: embedBuilder.Build());
        }
    }
}
