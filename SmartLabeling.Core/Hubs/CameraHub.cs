﻿using Microsoft.AspNetCore.SignalR;
using SmartLabeling.Core.Interfaces;
using SmartLabeling.Core.Models;
using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SmartLabeling.Core.Hubs
{
    public class CameraHub : Hub
    {
        private readonly ICameraService _cameraService;
        private static bool _isStreaming;
        private static int _imageWidth;
        private static int _imageHeight;
        private readonly ApiSettings _settings;

        public CameraHub(ICameraService cameraService, ApiSettings settings)
        {
            _imageWidth = settings.ImageWidth;
            _imageHeight = settings.ImageHeight;
            _cameraService = cameraService;
            _settings = settings;
        }

        public async Task StartCameraStreaming()
        {
            //TODO replace console with logger
            Console.WriteLine("Camera streaming started.");
            _isStreaming = true;
            await Clients.All.SendAsync("cameraStreamingStarted", "started...");
        }

        public async Task StopCameraStreaming()
        {
            Console.WriteLine("Camera streaming stopped.");
            _isStreaming = false;
            await Clients.All.SendAsync("cameraStreamingStopped");
        }

        public ChannelReader<Capture> CameraCaptureLoop()
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

                    //Console.WriteLine($"image size={capture.Image.Length} captured at {capture.CreatedAt}");

                    await Task.Delay(_settings.CaptureDelay);
                }
            }
        }
    }
}
