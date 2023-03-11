using Azure.DigitalTwins.Core;
using Rail.DigitalTwin.Core.Models;
using Rail.DigitalTwin.Core.Utilities;

namespace Rail.DigitalTwin.Functions
{
    public static class DigitalTwinFunctions
    {
        private static readonly TwinData _twinData;
        private static readonly EventQueue<LocationSensorModel> _sensorQueue;

        private static readonly object _lockObj;


        // Azure objects
        private static DigitalTwinsClient? _client;

        static DigitalTwinFunctions()
        {
            _twinData = TwinData.Instance;
            _sensorQueue = EventQueue<LocationSensorModel>.Instance;
            _sensorQueue.Enqueued += ProcessLocationSensor;
            _lockObj = new object();
        }

        public static SectionModel CreateSectionTwin()
        {
            var sectionTwin = new SectionModel()
            {
                Length = 10 * 1000, // meters
                Speed = 150 * (5.0 / 18.0), // meters/sec
                SafeDistance = 200, // meters
                CriticalDistance = 100, // meters
                StartLocation = new Location(17, 78), // Hyderabad location
            };

            sectionTwin.EndLocation = DistanceCalculator.GetPoint2(sectionTwin.StartLocation, sectionTwin.Length);

            lock (_lockObj)
            {
                _twinData.SectionTwin = sectionTwin;
            }
            return sectionTwin;
        }

        public static void CreateTrainTwin(TrainModel trainTwin)
        {
            if(_twinData.SectionTwin == null)
                CreateSectionTwin();

            lock (_lockObj)
            {
                if (_twinData.TrainTwins.Count > 0)
                    trainTwin.TrainID = _twinData.TrainTwins.Last().TrainID + 1;
                else
                    trainTwin.TrainID = 1;
                trainTwin.Section = _twinData.SectionTwin!;
                Location frontLocation = _twinData.SectionTwin!.StartLocation;
                Location rearLocation = DistanceCalculator.GetPoint2(frontLocation, -trainTwin.TrainLength);

                trainTwin.FrontSensor = new LocationSensorModel(trainTwin.TrainID + "-front", SensorPosition.Front, frontLocation);
                trainTwin.RearSensor = new LocationSensorModel(trainTwin.TrainID + "-rear", SensorPosition.Rear, rearLocation);
                _twinData.SensorTwins.Add(trainTwin.FrontSensor.SensorId, trainTwin.FrontSensor);
                _twinData.SensorTwins.Add(trainTwin.RearSensor.SensorId, trainTwin.RearSensor);
                _twinData.TrainTwins.Add(trainTwin);
            }
        }

        public static void ClearTrainTwins()
        {
            lock ( _lockObj)
            {
                _twinData.TrainTwins.Clear();
                _twinData.SensorTwins.Clear();
            }
        }

        public static List<TrainModel> GetTrains()
        {
            return _twinData.TrainTwins;
        }

        public static SectionModel GetSection()
        {
            if(_twinData.SectionTwin == null)
                CreateSectionTwin();

            return _twinData.SectionTwin!;
        }

        public static void SetTrainSpeed(TrainSpeedModel speedModel)
        {
            TrainModel trainTwin = _twinData.TrainTwins.Find(t => t.TrainID == speedModel.TrainID)!;

            lock( _lockObj)
            {
                trainTwin.SimulatorSpeed = speedModel.Speed;
            }
        }

        public static void ProcessLocationSensor(LocationSensorModel sensor)
        {
            TrainModel? trainTwin = null;
            // getting train for the sensor
            foreach (TrainModel train in _twinData.TrainTwins)
            {
                if(train.FrontSensor.SensorId == sensor.SensorId || train.RearSensor.SensorId == sensor.SensorId)
                {
                    trainTwin = train;
                    break;
                }
            }

            // ensure 
            if (trainTwin == null)
                return;

            if(sensor.Position == SensorPosition.Front)
            {
                trainTwin.FrontTravelled += sensor.DistanceTravelled;
            }
            else
            {
                trainTwin.RearTravelled += sensor.DistanceTravelled;
            }

            trainTwin.Speed = sensor.Speed;

            // getting recommended speed
            trainTwin.RecommendedSpeed = trainTwin.Section.Speed;

            // checking if the train crossed the section
            if (trainTwin.FrontTravelled > trainTwin.Section.Length &&
                trainTwin.RearTravelled > trainTwin.Section.Length)
            {
                lock (_lockObj)
                {
                    _twinData.SensorTwins.Remove(trainTwin.FrontSensor.SensorId);
                    _twinData.SensorTwins.Remove(trainTwin.RearSensor.SensorId);
                    _twinData.TrainTwins.RemoveAll(t => t.TrainID == trainTwin.TrainID);
                }
            }
        }

    }
}
