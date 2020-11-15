using System.Threading.Tasks;
using Unosquare.RaspberryIO;

namespace SmartLabeling.Camera.Services
{
    public class CameraService : ICameraService
    {
        public Task<byte[]> GetImage(int width, int height)
        {
            return Pi.Camera.CaptureImageJpegAsync(width, height);
        }
    }
}
