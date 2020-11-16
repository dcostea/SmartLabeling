namespace SmartLabeling.Core.Models
{
    public class SensorsSettings
    {
        public int InfraredDistance { get; set; }

        public int ProximityTriggerPin { get; set; }

        public int ProximityEchoPin { get; set; }

        public int ProximityMaxDistance { get; set; }

        public int ProximityDistance { get; set; }

    }
}
