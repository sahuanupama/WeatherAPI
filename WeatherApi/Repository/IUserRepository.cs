using MongoNotesAPI.Repositories;
using WeatherApi.Models;
using WeatherApi.Models.Filter;

namespace WeatherApi.Repository
    {
    public interface IUserRepository
        {
        bool CreateUser(ApiUser user);
        ApiUser AuthenticateUser(string apiKey, UserRoles requiredRole);
        void UpdateLastLogin(string apiKey);
        void Delete(String id);
        void DeleteMany(UserFilter userfilter);
        }
    }
