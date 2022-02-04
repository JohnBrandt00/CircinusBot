using Discord;
using Discord.Audio;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CircinusBot
{
    class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> connectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        public async Task JoinAudio(IGuild guild, IVoiceChannel target)
        {
            IAudioClient client;

            if (connectedChannels.TryGetValue(guild.Id, out client))
            {
                
                Console.WriteLine("Already in the channel");
                return;
            }
            if (target.Guild.Id != guild.Id)
                return;

            var audioclient = await target.ConnectAsync();

            if(connectedChannels.TryAdd(guild.Id,audioclient))
            {
                 Console.WriteLine("connected to voice");
            }


        }

        public async Task LeaveAudio(IGuild guild)
        {
            IAudioClient client;
            Console.WriteLine(guild.Id);
            if(connectedChannels.TryRemove(guild.Id,out client))
            {
                await client.StopAsync();
                
            }

        }


        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
        {
            // Your task: Get a full path to the file if the value of 'path' is only a filename.
            path = @"C:\Users\notjo\source\repos\bcs\CircinusBot\CircinusBot\newest.wav";
            Console.WriteLine(path);
            
            if (!File.Exists(path))
            {
                await channel.SendMessageAsync("File does not exist.");
                return;
            }
            IAudioClient client;
            if (connectedChannels.TryGetValue(guild.Id, out client))
            {
                //await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");
                using (var ffmpeg = CreateProcess(path))
                using (var stream = client.CreatePCMStream(AudioApplication.Music))
                {
                    try { await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream); }
                    finally { await stream.FlushAsync(); }
                }
            }
        }

        private Process CreateProcess(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }


    }
}
