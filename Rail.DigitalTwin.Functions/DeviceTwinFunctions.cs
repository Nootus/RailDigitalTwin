using Rail.DigitalTwin.Core.Models;
using Rail.DigitalTwin.Core.Utilities;

namespace Rail.DigitalTwin.Functions
{
    public static class DeviceTwinFunctions
    {
        private static readonly TwinData _twinData = TwinData.Instance;
        private static readonly EventQueue<LocationSensorModel> _sensorQueue = EventQueue<LocationSensorModel>.Instance;

        private static readonly object _lockObj = new object();

        public static void ProcessLocationSensor(LocationSensorModel sensor)
        {
            // getting sensorid
            LocationSensorModel sensorTwin = _twinData.SensorTwins[sensor.SensorId];
            double distanceTravelled = DistanceCalculator.CalculateDistance(sensor.CurrentLocation, sensorTwin.CurrentLocation);

            // check for geofense & calculating the distance travelled by sensor
            if (sensor.CurrentLocation.Longitude > _twinData.SectionTwin.StartLocation.Longitude)
            {
                // checking when the sensor entered the section
                if (sensorTwin.DistanceTravelled == 0)
                {
                    sensorTwin.DistanceTravelled = DistanceCalculator.CalculateDistance(sensor.CurrentLocation, _twinData.SectionTwin.StartLocation);
                }
                else
                {
                    sensorTwin.DistanceTravelled = distanceTravelled;
                }

                //sensorTwin.distanceTravelled = distanceTravelled;
            }
            else
            {
                sensorTwin.DistanceTravelled = 0;
            }

            sensorTwin.Speed = distanceTravelled / sensor.TimeElapsed;
            sensorTwin.CurrentLocation = sensor.CurrentLocation;

            _sensorQueue.Enqueue(sensorTwin);
        }
    }
}
