using Amazon.Runtime.SharedInterfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using WeatherApi.Models;
using WeatherApi.Models.Filter;
using WeatherApi.Services;

namespace WeatherApi.Repository
    {
    public class UserRepository : IUserRepository
        {
        private readonly IMongoCollection<ApiUser> _users;

        public UserRepository(MongoConnectionBuilder builder)
            {
            _users = builder.GetDatabase().GetCollection<ApiUser>("Users");
            }

        public ApiUser AuthenticateUser(string apiKey, UserRoles requiredRole)
            {
            var filter = Builders<ApiUser>.Filter.Eq(c => c.APIKey, apiKey);

            filter &= Builders<ApiUser>.Filter.Eq(c => c.Active, true);
            var user = _users.Find(filter).FirstOrDefault();

            if (user == null || !IsAllowedRole(user.Role, requiredRole))
                {
                return null;
                }
            return user;
            }

        public bool CreateUser(ApiUser user)
            {
            var filter = Builders<ApiUser>.Filter.Eq(c => c.Email, user.Email);
            var existingUser = _users.Find(filter).FirstOrDefault();
            if (existingUser != null)
                {
                return false;
                }

            user.APIKey = Guid.NewGuid().ToString();
            user.LastAccess = DateTime.Now;
            _users.InsertOne(user);
            return true;
            }

        public void UpdateLastLogin(string apiKey)
            {
            var filter = Builders<ApiUser>.Filter.Eq(c => c.APIKey, apiKey);

            var update = Builders<ApiUser>.Update.Set(u => u.LastAccess, DateTime.Now);
            _users.UpdateOne(filter, update);
            }

        public void Delete(string id)
            {
            ObjectId objid = ObjectId.Parse(id);
            var filter = Builders<ApiUser>.Filter.Eq(user => user._id, objid);
            _users.DeleteOne(filter);
            }

        public void DeleteMany(UserFilter userfilter)
            {
            var filter = GenerateFilterDefinition(userfilter);
            _users.DeleteMany(filter);
            }

        private bool IsAllowedRole(string userRole, UserRoles requiredRole)
            {
            if (!Enum.TryParse(userRole.ToUpper(), out UserRoles roleName))
                {
                return false;
                }

            int userRoleNumber = (int)roleName;
            int requiedRoleNumber = (int)requiredRole;
            return userRoleNumber <= requiedRoleNumber;

            }
        private FilterDefinition<ApiUser> GenerateFilterDefinition(UserFilter userFilter)
            {
            var builder = Builders<ApiUser>.Filter;
            var filter = builder.Empty;

            if (userFilter.LastAccess != null)
                {
                filter &= builder.Lte(apiUser => apiUser.LastAccess, userFilter.LastAccess.Value);
                }
            return filter;
            }
        }
    }