using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace CircinusBot.Services
{
    class CommandHandler
    {
        private readonly IConfiguration configuration;
        private readonly CommandService commandService;
        private readonly DiscordSocketClient discordSocketClient;
        private readonly IServiceProvider serviceProvider;

        public CommandHandler(IServiceProvider _serviceProvider)
        {
            //this.serviceProvider = serviceProvider;
            Console.WriteLine("command handler");
            configuration = _serviceProvider.GetRequiredService<IConfiguration>();
            commandService = _serviceProvider.GetRequiredService<CommandService>();
            discordSocketClient = _serviceProvider.GetRequiredService<DiscordSocketClient>();
            
            serviceProvider = _serviceProvider;

            commandService.CommandExecuted += CommandService_CommandExecuted;
            discordSocketClient.MessageReceived += DiscordSocketClient_MessageReceived;
            
        }

        public async Task InitializeAsync()
        {
            await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);
        }

        private async Task DiscordSocketClient_MessageReceived(SocketMessage msg)
        {
            if (!(msg is SocketUserMessage message))
            {
                return;
            }

            if (message.Source != MessageSource.User)
            {
                return;
            }

            // sets the argument position away from the prefix we set
            var argPos = 0;

            // get prefix from the configuration file
            char prefix = Char.Parse(configuration["cmdPrefix"]);

            // determine if the message has a valid prefix, and adjust argPos based on prefix
            if (!(message.HasMentionPrefix(discordSocketClient.CurrentUser, ref argPos) || message.HasCharPrefix(prefix, ref argPos)))
            {
                return;
            }

            var context = new SocketCommandContext(discordSocketClient, message);

            // execute command if one is found that matches
            await commandService.ExecuteAsync(context, argPos, serviceProvider);

        }

        private async Task CommandService_CommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
          if(!command.IsSpecified)
            {
                
                Console.WriteLine("[Circinus] Command failed [] <-> []");
                return;
            }
          if(result.IsSuccess)
            {
                Console.WriteLine("[Circinus] command [] executed for -> []");
                return;
            }

            await context.Channel.SendMessageAsync("Something went horribly wrong!");

        }
    }
}
