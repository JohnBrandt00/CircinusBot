using CircinusBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Linq;
namespace CircinusBot
{



    class Program
    {
        private DiscordSocketClient _discordSocketClient;
        private IServiceProvider serviceProvider;
        private IConfiguration _config;


        public Program()
        {
            //_discordSocketClient = new DiscordSocketClient();
            //_discordSocketClient.Log += Log;
            //_discordSocketClient.Ready += () =>
            //{
            //    Console.WriteLine("Bot is connected!");
            //    return Task.CompletedTask;
            //};
            //_discordSocketClient.MessageUpdated += MessageUpdated;
            //_discordSocketClient.MessageReceived += MessageReceivedAsync;
            
            var _builder = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile(path: "config.json");
            _config = _builder.Build();



           
        }
        static void Main(string[] args)
        
             => new Program().MainAsync().GetAwaiter().GetResult();
        

        public async Task MainAsync()
        {

            using(var services = ConfigureSevices())
            {
                serviceProvider = services;
                var client = services.GetRequiredService<DiscordSocketClient>();
                _discordSocketClient = client;
                _discordSocketClient.Log += Log;
               // _discordSocketClient.MessageReceived += MessageReceivedAsync;
                //_discordSocketClient.MessageUpdated += MessageUpdated;
                _discordSocketClient.Ready += () => { Console.WriteLine("[Circinus] Bot is connected and ready!"); return Task.CompletedTask; };
               _discordSocketClient.Disconnected += onDisc;
              
               
                services.GetRequiredService<CommandService>().Log += Log;

                await _discordSocketClient.LoginAsync(TokenType.Bot, _config["Token"]);
                await _discordSocketClient.StartAsync();

                await services.GetRequiredService<CommandHandler>().InitializeAsync();

                await Task.Delay(-1);
            }

            


        }

       

        private Task Log(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.ToString());
            return Task.CompletedTask;
        }


        //I wonder if there's a better way to handle commands (spoiler: there is :))
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            //This ensures we don't loop things by responding to ourselves (as the bot)
            if (message.Author.Id == _discordSocketClient.CurrentUser.Id)
                return;

            if (message.Content == ".hello")
            {
                await message.Channel.SendMessageAsync("world!");
                
            }
        }
        private async Task onDisc(Exception e )
        {
            Console.WriteLine("Disconexted");
              _discordSocketClient.Guilds.ToList<SocketGuild>().ForEach(x => { serviceProvider.GetRequiredService<AudioService>().LeaveAudio(x as IGuild); });
           
           //await _discordSocketClient.StartAsync();
           
        }
    

        private ServiceProvider ConfigureSevices()
        {
            return new ServiceCollection()
                .AddSingleton(_config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<AudioService>()
                .BuildServiceProvider();
        }


    }
}
