using Azure.DigitalTwins.Core;
using Microsoft.VisualBasic;
using Rail.DigitalTwin.Core.Models;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using static System.Collections.Specialized.BitVector32;

namespace Rail.DigitalTwin.Core.Azure
{
    public static class Mapper
    {
        public static BasicDigitalTwin MapSection(SectionModel model)
        {
            var twin = new BasicDigitalTwin
            {
                Id = model.SectionID,
                Metadata = { ModelId = ModelIDs.SectionModeID },
                Contents =
                {
                    { "length", model.Length },
                    { "speed", Math.Round(model.Speed, 2) },
                    { "safeDistance", model.SafeDistance },
                    { "criticalDistance", model.CriticalDistance },
                    { "startLocation", new Dictionary<string,object>
                        {
                            { "latitude", model.StartLocation.Latitude },
                            { "longitude", Math.Round(model.StartLocation.Longitude, 4) }
                        }
                    },
                    { "endLocation", new Dictionary<string,object>
                        {
                            { "latitude", model.EndLocation.Latitude },
                            { "longitude", Math.Round(model.EndLocation.Longitude, 4) }
                        }
                    }
                }
            };

            return twin;
        }

        public static SectionModel MapSection(BasicDigitalTwin twin)
        {
            SectionModel model = new SectionModel()
            {
                SectionID = twin.Id,
                Length = Convert.ToInt32(twin.Contents["length"].ToString()),
                Speed = Convert.ToDouble(twin.Contents["speed"].ToString()),
                SafeDistance = Convert.ToInt32(twin.Contents["safeDistance"].ToString()),
                CriticalDistance = Convert.ToInt32(twin.Contents["criticalDistance"].ToString()),
                StartLocation = new Location(twin.Contents["startLocation"].ToString()!),
                EndLocation = new Location(twin.Contents["endLocation"].ToString()!)
            };

            return model;
        }

        public static BasicDigitalTwin MapTrain(TrainModel model)
        {
           var twin = new BasicDigitalTwin
            {
                Id = model.TrainID,
                Metadata = { ModelId = ModelIDs.TrainModelID },
                Contents =
                {
                    { "trainNumber", model.TrainNumber },
                    { "trainName", model.TrainName },
                    { "trainLength", model.TrainLength },
                    { "speed", Math.Round(model.Speed, 2) },
                }
            };

            return twin;
        }

        public static BasicDigitalTwinComponent MapLocationSensor(LocationSensorModel model)
        {
            var twin = new BasicDigitalTwinComponent
            {
                Contents =
                {
                    { "position", model.Position },
                    { "location", new Dictionary<string,object>
                        {
                            { "latitude", model.Location.Latitude },
                            { "longitude", Math.Round(model.Location.Longitude, 4) }
                        }
                    },
                    { "distanceTravelled", model.DistanceTravelled },
                    { "speed", Math.Round(model.Speed, 2) },
                    { "timeElapsed", model.TimeElapsed },
                }
            };

            return twin;
        }
    }
}
