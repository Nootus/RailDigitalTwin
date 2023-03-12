#pragma warning disable CS8618

using System.Text.Json.Serialization;

namespace Rail.DigitalTwin.Core.Models
{
    public class TrainModel
    {
        [JsonIgnore]
        public SectionModel Section { get; set; }
        [JsonIgnore]
        public LocationSensorModel FrontSensor { get; set; }
        [JsonIgnore]
        public LocationSensorModel RearSensor { get; set; }
        public double FrontTravelled { get; set; } = 0;  // meters
        public double RearTravelled { get; set; } = 0;  // meters

        public string TrainID { get; set; }
        public int TrainNumber { get;set; }

        public string TrainName { get; set; }
        public int TrainLength { get; set; } // meters
        public double Speed { get; set; } // meters/sec
        public double RecommendedSpeed { get; set; } // meters/sec
        public double SimulatorSpeed { get; set; } // m/s
    }
}
#pragma warning restore CS8618
