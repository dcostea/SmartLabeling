using Microsoft.AspNetCore.SignalR;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SmartLabeling.Core.Hubs
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
    }
}
