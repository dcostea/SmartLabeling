using Microsoft.AspNetCore.SignalR;
using SmartLabeling.Core.Interfaces;
using SmartLabeling.Core.Models;
using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SmartLabeling.Core.Hubs
{
    public class SensorsHub : Hub
    {
        private readonly ISensorsService _sensorsService;
        private static bool _isStreaming;
        private static int _imageWidth;
        private static int _imageHeight;
        private readonly ApiSettings _settings;

        public SensorsHub(ISensorsService sensorsService, ApiSettings settings)
        {
            _imageWidth = settings.ImageWidth;
            _imageHeight = settings.ImageHeight;
            _sensorsService = sensorsService;
            _settings = settings;
        }

        public async Task StartSensorsStreaming()
        {
            //TODO replace console with logger
            Console.WriteLine("Sensors streaming started.");
            _isStreaming = true;
            await Clients.All.SendAsync("sensorsStreamingStarted", "started...");
        }

        public async Task StopSensorsStreaming()
        {
            Console.WriteLine("Sensors streaming stopped.");
            _isStreaming = false;
            await Clients.All.SendAsync("sensorsStreamingStopped");
        }

        public ChannelReader<Reading> SensorsCaptureLoop()
        {
            var channel = Channel.CreateUnbounded<Reading>();
            _ = WriteToChannel(channel.Writer);
            return channel.Reader;

            async Task WriteToChannel(ChannelWriter<Reading> writer)
            {
                while (_isStreaming)
                {
                    var luminosity = await _sensorsService.ReadLuminosity();
                    await Task.Delay(_settings.ReadingDelay);

                    var temperature = await _sensorsService.ReadTemperature();
                    await Task.Delay(_settings.ReadingDelay);

                    var infrared = await _sensorsService.ReadInfrared();
                    await Task.Delay(_settings.ReadingDelay);

                    var reading = new Reading
                    {
                        Luminosity = luminosity,
                        Temperature = temperature,
                        Infrared = infrared,
                        CreatedAt = DateTime.Now.ToString("yyyyMMddhhmmssff")
                    };

                    await writer.WriteAsync(reading);

                    //Console.WriteLine($"image size={capture.Image.Length} captured at {capture.CreatedAt}");

                    await Task.Delay(_settings.CaptureDelay);
                }
            }
        }
    }
}
