using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CircinusBot.Modules
{
   public class AudioModule : ModuleBase<ICommandContext>
    {
        private readonly AudioService audioService;

         AudioModule(AudioService service)
        {
            audioService = service;
        }

        [Command("join", RunMode=RunMode.Async)]
        public async Task Join()
        {
            await audioService.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayCmd([Remainder] string song)
        {
            await audioService.SendAudioAsync(Context.Guild, Context.Channel, song);
        }


    }
}
