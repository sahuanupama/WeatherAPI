namespace WeatherApi.Models.DTOs
    {
    public class PercipitationDTO
        {
        public string DeviceName { get; set; }
        public DateTime DateTime { get; set; }
        public Double? Precipitation { get; set; }
        }
    }
