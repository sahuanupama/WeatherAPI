namespace WeatherApi.Models.WeatherFilter
    {
    public class WeatherFilter
        {
        public string? DeviceName { get; set; }

        public double? AbovePrecipitation { get; set; }
        public double? BelowPrecipitation { get; set; }

        public DateTime? BeforeTime { get; set; }
        public DateTime? AfterTime { get; set; }


        public double? AboveTemperature { get; set; }
        public double? BelowTemperature { get; set; }
        public double? AboveAtmosphericPressure { get; set; }
        public double? BelowAtmosphericPressure { get; set; }
        public double? AboveSolarRadiation { get; set; }
        public double? BelowSolarRadiation { get; set; }
        }
    }
