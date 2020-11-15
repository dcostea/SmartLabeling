using Microsoft.AspNetCore.SignalR;
using SmartLabeling.Camera.Models;
using SmartLabeling.Camera.Services;
using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SmartLabeling.Camera.Hubs
{
    public class CameraHub : Hub
    {
        private readonly ICameraService _cameraService;
        private static bool _isStreaming;
        private static int _imageWidth;
        private static int _imageHeight;
        private readonly CameraSettings _settings;

        public CameraHub(ICameraService cameraService, CameraSettings cameraSettings)
        {
            _imageWidth = cameraSettings.ImageWidth;
            _imageHeight = cameraSettings.ImageHeight;
            _cameraService = cameraService;
            _settings = cameraSettings;
        }

        public async Task StartStreaming()
        {
            //TODO replace console with logger
            Console.WriteLine("Streaming started.");
            _isStreaming = true;
            await Clients.All.SendAsync("streamingStarted", "started...");
        }

        public async Task StopStreaming()
        {
            Console.WriteLine("Streaming stopped.");
            _isStreaming = false;
            await Clients.All.SendAsync("streamingStopped");
        }

        public ChannelReader<Capture> SensorsTick()
        {
            var channel = Channel.CreateUnbounded<Capture>();
            _ = WriteToChannel(channel.Writer);
            return channel.Reader;

            async Task WriteToChannel(ChannelWriter<Capture> writer)
            {
                while (_isStreaming)
                {
                    var image = await _cameraService.GetImage(_imageWidth, _imageHeight);

                    var capture = new Capture
                    {
                        Image = image,
                        CreatedAt = DateTime.Now.ToString("yyyyMMddhhmmssff")
                    };

                    await writer.WriteAsync(capture);

                    Console.WriteLine($"image size={capture.Image.Length} captured at {capture.CreatedAt}");

                    await Task.Delay(_settings.CaptureDelay);
                }
            }
        }
    }
}
