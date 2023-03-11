#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Rail.DigitalTwin.Core.Models
{
    public class SectionModel
    {
        public int Length { get; set; }  // meters
        public double Speed { get; set; }     // meters / sec
        public int SafeDistance { get; set; } // meters
        public int CriticalDistance { get; set; } // meters

        public Location StartLocation { get; set; }
        public Location EndLocation { get; set;}
    }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
