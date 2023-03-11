#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Rail.DigitalTwin.Core.Models
{
    public class LocationSensorModel
    {
        public LocationSensorModel(string sensorId, SensorPosition position, Location location)
        {
            SensorId = sensorId;
            Position = position;
            CurrentLocation = location;
            DistanceTravelled = 0;
            Speed = 0;
            TimeElapsed = 0;
        }

        public LocationSensorModel(LocationSensorModel sensor)
        {
            SensorId = sensor.SensorId;
            Position = sensor.Position;
            CurrentLocation = sensor.CurrentLocation;
            DistanceTravelled = 0;
            Speed = 0;
            TimeElapsed = 0;
        }

        public string SensorId { get; set; }
        public SensorPosition Position { get; set; }
        public Location CurrentLocation { get; set; }
        public double DistanceTravelled { get; set; } // meters
        public double Speed { get; set; }
        public int TimeElapsed { get; set; }
    }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
