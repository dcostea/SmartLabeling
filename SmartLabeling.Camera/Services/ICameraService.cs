using System.Threading.Tasks;

namespace SmartLabeling.Camera.Services
{
    public interface ICameraService
    {
        public Task<byte[]> GetImage(int width, int height);
    }
}