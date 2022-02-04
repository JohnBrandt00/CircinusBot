using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CircinusBot.Modules
{
   public class BasicCommand : ModuleBase
    {
        [Command("hello")]
        public async Task HelloCommand()
        {
            var chan = Context.Channel.Name;
            var user = Context.User;
            
            await ReplyAsync($"Hello {user.Username}  we're in the {chan} channel!");
            //await ReplyAsync("Hello");
            await ReplyAsync("https://www.youtube.com/watch?v=5eZyspecXJE");
            

            
            //EmbedBuilder embed = new EmbedBuilder();
           // embed.WithTitle("Bro you smell like beef");
          //  embed.WithThumbnailUrl("https://lh3.googleusercontent.com/YGJ77qN9KiwctZgfqV8Bf3hNo0rZvcFaPKDTkvtS6kVbtwyCS80Pm6dpXzJCCLZE1Q");
         //   embed.AddField("Space: ", "yes");
        //    embed.AddField("are u ugly?", "yes bb");



           // await ReplyAsync(message: null, false ,embed.Build() );
        }
    }
}
