using SmartLabeling.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace SmartLabeling.API.Services
{
    public class FakeSensorsService : ISensorsService
    {
        private readonly Random _random;

        public FakeSensorsService()
        {
            _random = new Random();
        }

        public Task<double> ReadDistance()
        {
            return Task.Run(() => _random.NextDouble() * 400); // between 0 and 400
        }

        public Task<double> ReadInfrared()
        {
            return Task.Run(() => _random.NextDouble() * 100); // between 0 and 100
        }

        public Task<double> ReadLuminosity()
        {
            return Task.Run(() => _random.NextDouble() * 100); // between 0 and 100
        }

        public Task<double> ReadTemperature()
        {
            return Task.Run(() => _random.NextDouble() * 130 + 20); // between 20 and 150 celsius degrees
        }
    }
}
