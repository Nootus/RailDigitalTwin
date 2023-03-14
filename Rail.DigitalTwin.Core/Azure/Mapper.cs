using Azure.DigitalTwins.Core;
using Rail.DigitalTwin.Core.Models;

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
                            { "longitude", model.StartLocation.Longitude }
                        }
                    },
                    { "endLocation", new Dictionary<string,object>
                        {
                            { "latitude", model.EndLocation.Latitude },
                            { "longitude", model.EndLocation.Longitude }
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
            // creating sensor twins
            var frontSensorTwin = Mapper.MapLocationSensor(model.FrontSensor);
            var rearSensorTwin = Mapper.MapLocationSensor(model.RearSensor);

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
                    { "recommendedSpeed", Math.Round(model.RecommendedSpeed, 2) },
                    { "simulatorSpeed", Math.Round(model.SimulatorSpeed, 2) },
                    { "frontTravelled", Math.Round(model.FrontTravelled, 2) },
                    { "rearTravelled", Math.Round(model.RearTravelled, 2) },
                    { "frontSensor", frontSensorTwin },
                    { "rearSensor", rearSensorTwin }
                }
            };

            return twin;
        }

        public static TrainModel MapTrain(BasicDigitalTwin twin)
        {
            TrainModel model = new TrainModel()
            {
                TrainID = twin.Id,
                TrainNumber = Convert.ToInt32(twin.Contents["trainNumber"].ToString()),
                TrainName = twin.Contents["trainName"].ToString()!,
                TrainLength = Convert.ToInt32(twin.Contents["trainLength"].ToString()),
                Speed = Convert.ToDouble(twin.Contents["speed"].ToString()),
                RecommendedSpeed = Convert.ToDouble(twin.Contents["recommendedSpeed"].ToString()),
                SimulatorSpeed = Convert.ToDouble(twin.Contents["simulatorSpeed"].ToString()),
                FrontTravelled = Convert.ToDouble(twin.Contents["frontTravelled"].ToString()),
                RearTravelled = Convert.ToDouble(twin.Contents["rearTravelled"].ToString()),
            };

            return model;
        }

        public static BasicDigitalTwinComponent MapLocationSensor(LocationSensorModel model)
        {
            var twin = new BasicDigitalTwinComponent
            {
                Contents =
                {
                    { "sensorID", model.SensorID },
                    { "trainID", model.TrainID },
                    { "position", model.Position },
                    { "location", new Dictionary<string,object>
                        {
                            { "latitude", model.Location.Latitude },
                            { "longitude", model.Location.Longitude }
                        }
                    },
                    { "distanceTravelled", Math.Round(model.DistanceTravelled, 2) },
                    { "speed", Math.Round(model.Speed, 2) },
                    { "timeElapsed", model.TimeElapsed },
                }
            };

            return twin;
        }
        public static LocationSensorModel MapLocationSensor(BasicDigitalTwinComponent twin)
        {
            LocationSensorModel model = new LocationSensorModel()
            {
                SensorID = twin.Contents["sensorID"].ToString()!,
                TrainID = twin.Contents["trainID"].ToString()!,
                Position = (SensorPosition) Convert.ToInt32(twin.Contents["position"].ToString()),
                Location = new Location(twin.Contents["location"].ToString()!),
                DistanceTravelled = Convert.ToDouble(twin.Contents["distanceTravelled"].ToString()),
                Speed = Convert.ToDouble(twin.Contents["speed"].ToString()),
                TimeElapsed = Convert.ToInt32(twin.Contents["timeElapsed"].ToString()),
            };

            return model;
        }
    }
}
