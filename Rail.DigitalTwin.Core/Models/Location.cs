using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rail.DigitalTwin.Core.Models
{
    public class Location
    {
        public Location() { } 
        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public Location(string twinContent)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            var location = JsonSerializer.Deserialize<Location>(twinContent, options);
            Latitude = location!.Latitude; 
            Longitude = location.Longitude;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
