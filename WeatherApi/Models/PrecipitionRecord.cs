namespace WeatherApi.Models
    {
    public class PrecipitionRecord
        {
        public string objectId { get; set; }
        public string DeviceName { get; set; }
        public DateTime dateTime { get; set; }
        public double? Precipition { get; set; }
        }
    }
