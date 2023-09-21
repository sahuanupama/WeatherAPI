using WeatherApi.Models;

namespace WeatherApi.Repository
    {
    public interface IUserRepository
        {
        bool CreateUser(ApiUser user);
        ApiUser AuthenticateUser(string apiKey, UserRoles requiredRole);
        void UpdateLastLogin(string apiKey);
        }
    }
