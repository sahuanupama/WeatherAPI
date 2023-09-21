namespace WeatherApi.Models.DTOs
{
    public class UserCreateDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }   
        public string Role { get; set; }
        public DateTime LastAccess { get; set; }
        public string? ApiKey { get; set; }
    }
}
