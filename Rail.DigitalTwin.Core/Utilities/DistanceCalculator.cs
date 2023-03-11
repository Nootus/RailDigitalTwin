using Rail.DigitalTwin.Core.Models;

namespace Rail.DigitalTwin.Core.Utilities
{
    public static class DistanceCalculator
    {
        public static double CalculateDistance(Location point1, Location point2)
        {
            var lat1 = point1.Latitude * (Math.PI / 180.0);
            var long1 = point1.Longitude * (Math.PI / 180.0);
            var lat2 = point2.Latitude * (Math.PI / 180.0);
            var long2 = point2.Longitude * (Math.PI / 180.0) - long1;
            var dist = Math.Pow(Math.Sin((lat2 - lat1) / 2.0), 2.0) +
                     Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(long2 / 2.0), 2.0);
            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(dist), Math.Sqrt(1.0 - dist)));
        }

        // dist in meters
        public static Location GetPoint2(Location point1, double dist)
        {
            Location point2 = new Location(point1.Latitude, point1.Longitude);
            double longchangeonem = 0.0000093960227;
            point2.Longitude += longchangeonem * dist;
            return point2;
        }
    }
}
