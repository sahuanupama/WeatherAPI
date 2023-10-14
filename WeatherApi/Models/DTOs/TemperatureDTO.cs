namespace WeatherApi.Models.DTOs
    {
    public class TemperatureDTO
        {
        public string DeviceName { get; set; }
        public DateTime dateTime { get; set; }
        public double? Temperature { get; set; }
        }
    }

