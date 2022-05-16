using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Reshbot.Modules.Events;

namespace Reshbot.ReshDiscordUtils {
    public static class ServiceCollectionExtension {
        public static void RegisterBotEvents(this IServiceCollection serviceCollection) {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IEvent)))) {
                serviceCollection.AddSingleton(type);
            }
        }
    }
}
