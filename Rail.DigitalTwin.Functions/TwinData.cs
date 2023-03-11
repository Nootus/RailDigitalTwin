using Rail.DigitalTwin.Core.Models;

namespace Rail.DigitalTwin.Functions
{
    public class TwinData
    {
        private static readonly TwinData instance = new TwinData();
        public static TwinData Instance { get { return instance; } }

        public List<TrainModel> TrainTwins { get; } = new List<TrainModel>();
        public Dictionary<string, LocationSensorModel> SensorTwins { get; } = new Dictionary<string, LocationSensorModel>();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public SectionModel SectionTwin { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
