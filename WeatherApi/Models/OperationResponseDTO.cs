namespace WeatherApi.Models
    {
    // API Response model class
    public class OperationResponseDTO<T>
        {
        public string Message { get; set; } = string.Empty;
        public bool WasSuccessful { get; set; } = true;
        public T? Value { get; set; }
        public int RecordsAffected { get; set; } = 0;
        }
    }
