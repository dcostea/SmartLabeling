using Microsoft.AspNetCore.SignalR;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SmartLabeling.API.Hubs
{
    public class SensorHub : Hub
    {
        public async Task StartStreaming()
        {
            await Clients.All.SendAsync("streamingStarted", "started...");
        }

        public async Task StopStreaming()
        {
            await Clients.All.SendAsync("streamingStopped");
        }

        public ChannelReader<string> SensorsTick()
        {
            var channel = Channel.CreateUnbounded<string>();
            _ = WriteToChannel(channel.Writer);
            return channel.Reader;

            static async Task WriteToChannel(ChannelWriter<string> writer)
            {
                await writer.WriteAsync("writing");
            }
        }
    }
}
