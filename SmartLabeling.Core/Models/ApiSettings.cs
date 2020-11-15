namespace SmartLabeling.Core.Models
{
    public class ApiSettings 
    {
        public string CameraUrl { get; set; }

        public string SensorsUrl { get; set; }
        
        public string FakeUrl { get; set; }

        public string CameraHub { get; set; }

        public string SensorsHub { get; set; }

        public string FakeHub { get; set; }

        public int ImageWidth { get; set; }

        public int ImageHeight { get; set; }

        public int CaptureDelay { get; set; }
    }
}
