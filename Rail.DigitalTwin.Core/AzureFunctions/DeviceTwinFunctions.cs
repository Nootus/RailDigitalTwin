using Rail.DigitalTwin.Core.Models;
using Rail.DigitalTwin.Core.Utilities;

namespace Rail.DigitalTwin.Core.AzureFunctions
{
    public static class DeviceTwinFunctions
    {
        private static readonly EventQueue<LocationSensorModel> _sensorQueue = EventQueue<LocationSensorModel>.Instance;

        public static void ProcessLocationSensor(LocationSensorModel sensorModel, Location currentLocation, Location sectionStartLocation)
        {
            double distanceTravelled = DistanceCalculator.CalculateDistance(sensorModel.Location, currentLocation);

            // check for geofence & calculating the distance travelled by sensor
            if (currentLocation.Longitude > sectionStartLocation.Longitude)
            {
                // checking when the sensor entered the section
                if (sensorModel.DistanceTravelled == 0)
                {
                    sensorModel.DistanceTravelled = DistanceCalculator.CalculateDistance(currentLocation, sectionStartLocation);
                }
                else
                {
                    sensorModel.DistanceTravelled = distanceTravelled;
                }
            }
            else
            {
                sensorModel.DistanceTravelled = 0;
            }

            sensorModel.Speed = distanceTravelled / sensorModel.TimeElapsed;
            sensorModel.Location = currentLocation;

            _sensorQueue.Enqueue(sensorModel);
        }
    }
}
