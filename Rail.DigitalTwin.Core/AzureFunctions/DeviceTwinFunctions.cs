using Rail.DigitalTwin.Core.Models;
using Rail.DigitalTwin.Core.Utilities;

namespace Rail.DigitalTwin.Core.AzureFunctions
{
    public static class DeviceTwinFunctions
    {
        private static readonly EventQueue<LocationSensorModel> _sensorQueue = EventQueue<LocationSensorModel>.Instance;

        public static async Task ProcessLocationSensor(DeviceModel deviceModel)
        {
            // getting from DigitalTwinFunctions
            SectionModel sectionModel = await DigitalTwinFunctions.GetSectionAsync();
            LocationSensorModel sensorModel = new LocationSensorModel(
                                                        await DigitalTwinFunctions.GetSensorByIDAsync(deviceModel.DeviceID));


            Location deviceLocation = new Location(deviceModel.Latitude, deviceModel.Longitude);
            double distanceTravelled = DistanceCalculator.CalculateDistance(deviceLocation, sensorModel.Location);

            // check for geofence & calculating the distance travelled by sensor
            if (deviceLocation.Longitude > sectionModel.StartLocation.Longitude)
            {
                // checking when the sensor entered the section
                if (sensorModel.DistanceTravelled == 0)
                {
                    sensorModel.DistanceTravelled = DistanceCalculator.CalculateDistance(deviceLocation, sectionModel.StartLocation);
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

            sensorModel.Speed = distanceTravelled / deviceModel.TimeElapsed  ;
            sensorModel.Location = deviceLocation;

            await DigitalTwinFunctions.ProcessLocationSensorAsync(sensorModel);
            //_sensorQueue.Enqueue(sensorModel);
        }
    }
}
