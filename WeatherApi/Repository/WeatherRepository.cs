using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;
using Microsoft.PowerBI.Api.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NuGet.Packaging.Signing;
using System;
using System.Text.RegularExpressions;
using WeatherApi.Models;
using WeatherApi.Models.Filter;
using WeatherApi.Models.WeatherFilter;
using WeatherApi.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WeatherApi.Repository
    {
    public class WeatherRepository : IWeatherRepository
        {
        private readonly IMongoCollection<Weather> _weatherChange;

        //private readonly IMongoCollection<Weather> _precipitationFilter;
        // private readonly ILogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        ///
        public WeatherRepository(MongoConnectionBuilder connection)
            {
            // _weatherChange = connection.GetDatabase().GetCollection<Weather>("WeatherChange");
            _weatherChange = connection.GetDatabase().GetCollection<Weather>("Weather");
            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createWeather"></param>
        /// 
        public void Created(Weather createWeather)
            {
            _weatherChange.InsertOne(createWeather);
            }

        /// <summary>
        /// Create list of weather.
        /// Insert many data in the weather database.
        /// </summary>
        /// <param name="WeatherList"></param>
        public void CreateMany(List<Weather> WeatherList)
            {
            _weatherChange.InsertMany(WeatherList);
            }

        public OperationResponseDTO<Weather> Delete(string id)
            {
            ObjectId objId = ObjectId.Parse(id);
            var filter = Builders<Weather>.Filter.Eq(weather => weather._id, objId);
            var result = _weatherChange.DeleteOne(filter);

            if (result.DeletedCount > 0)
                {
                return new OperationResponseDTO<Weather>
                    {
                    Message = "Note Deleted Successfully",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.DeletedCount)
                    };
                }
            else
                {
                return new OperationResponseDTO<Weather>
                    {
                    Message = "No notes deleted. Please check details and try again.",
                    WasSuccessful = false,
                    RecordsAffected = 0
                    };
                }
            }
        public OperationResponseDTO<Weather> DeleteMany(WeatherFilter weatherFilter)
            {
            var filter = GenerateFilterDefinition(weatherFilter);
            var result = _weatherChange.DeleteMany(filter);
            if (result.DeletedCount > 0)
                {
                return new OperationResponseDTO<Weather>
                    {
                    Message = "Note/s Deleted Successfully",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.DeletedCount)
                    };
                }
            else
                {
                return new OperationResponseDTO<Weather>
                    {
                    Message = "No notes deleted. Please check details and try again.",
                    WasSuccessful = false,
                    RecordsAffected = 0
                    };
                }

            }

        public IEnumerable<Weather> GetAll()
            {
            var builder = Builders<Weather>.Filter;
            var filter = builder.Empty;
            return _weatherChange.Find(filter).ToEnumerable();
            }

        public IEnumerable<Weather> GetAll(WeatherFilter weatherFilter)
            {
            var filter = GenerateFilterDefinition(weatherFilter);
            return _weatherChange.Find(filter).SortByDescending(w => w.Precipitation).Limit(10).
                ToEnumerable();
            }

        public Weather GetById(string id)
            {
            ObjectId ObjId = ObjectId.Parse(id);

            var filter = Builders<Weather>.Filter.Eq(Weather => Weather._id, ObjId);

            return _weatherChange.Find(filter).FirstOrDefault();
            }

        public void Update(string id, Weather updateWeather)
            {
            ObjectId objId = ObjectId.Parse(id);
            var filter = Builders<Weather>.Filter.Eq(Weather => Weather._id, objId);
            var builder = Builders<Weather>.Update;

            var update = builder.Set(weather => weather.Latitude, updateWeather.Latitude)
                                 .Set(weather => weather.Longitude, updateWeather.Longitude);
            _weatherChange.UpdateOneAsync(filter, update);
            }

        public void Replace(string id, Weather updateWeather)
            {
            ObjectId objId = ObjectId.Parse(id);

            var filter = Builders<Weather>.Filter.Eq(Weather => Weather._id, objId);
            updateWeather._id = objId;
            _weatherChange.ReplaceOne(filter, updateWeather);
            }

        /*public void Delete(string id)
            {
            ObjectId objId = ObjectId.Parse(id);
            var filter = Builders<Weather>.Filter.Eq(weather => weather._id, objId);
            _weatherChange.DeleteOne(filter);
            }*/

        /*public void DeleteMany(WeatherFilter weatherFilter)
            {
            var filter = GenerateFilterDefinition(weatherFilter);
            _weatherChange.DeleteMany(filter);
            }*/

        private UpdateDefinition<Weather> GenerateUpdateDefinition(WeatherPatchDetailsDto details)
            {
            return null;
            }

        private FilterDefinition<Weather> GenerateFilterDefinition(WeatherFilter weatherFilter)
            {
            var builder = Builders<Weather>.Filter;
            var filter = builder.Empty;

            if (String.IsNullOrEmpty(weatherFilter.DeviceName) == false)
                {
                var cleanedString = Regex.Escape(weatherFilter.DeviceName);
                filter &= builder.Regex(weather => weather.DeviceName, BsonRegularExpression.Create(cleanedString));
                }

            if (weatherFilter.AbovePrecipitation != null)
                {
                filter &= builder.Lte(weather => weather.Precipitation, weatherFilter.AbovePrecipitation.Value);
                }

            if (weatherFilter.BelowPrecipitation != null)
                {
                filter &= builder.Gte(weather => weather.Precipitation, weatherFilter.BelowPrecipitation.Value);
                }

            if (weatherFilter.BeforeTime.HasValue)
                {
                filter &= builder.Lte(weather => weather.Time, weatherFilter.BeforeTime);
                }
            if (weatherFilter.AfterTime.HasValue)
                {
                filter &= builder.Gte(weather => weather.Time, weatherFilter.AfterTime);
                }

            if (weatherFilter.AboveTemperature != null)
                {
                filter &= builder.Lte(weather => weather.Temperature, weatherFilter.AboveTemperature.Value);
                }
            if (weatherFilter.BelowTemperature != null)
                {
                filter &= builder.Gte(weather => weather.Temperature, weatherFilter.BelowTemperature.Value);
                }


            if (weatherFilter.AboveAtmosphericPressure != null)
                {
                filter &= builder.Lte(weather => weather.AtmosphericPressure, weatherFilter.AboveAtmosphericPressure.Value);
                }
            if (weatherFilter.BelowAtmosphericPressure != null)
                {
                filter &= builder.Gte(weather => weather.AtmosphericPressure, weatherFilter.BelowAtmosphericPressure.Value);
                }


            if (weatherFilter.AboveSolarRadiation != null)
                {
                filter &= builder.Lte(weather => weather.SolarRadiation, weatherFilter.AboveSolarRadiation.Value);
                }
            if (weatherFilter.BelowSolarRadiation != null)
                {
                filter &= builder.Gte(weather => weather.SolarRadiation, weatherFilter.BelowSolarRadiation.Value);
                }

            return filter;
            }

        public void UpdateMany(WeatherPatchDetailsDto UpdateDto)
            {
            ObjectId objId = ObjectId.Parse(UpdateDto.objectId);
            var filter = Builders<Weather>.Filter.Eq(Weather => Weather._id, objId);

            var update = Builders<Weather>.Update.Set(w => w.Precipitation, UpdateDto.precipitation);
            _weatherChange.UpdateOne(filter, update);
            }

        public void Create(Weather createdWeather)
            {
            _weatherChange.InsertOne(createdWeather);
            }

        public void UpdateMany(WeatherFilter filter)
            {
            throw new NotImplementedException();
            }


        void IWeatherRepository.DeleteMany(WeatherFilter filter)
            {
            throw new NotImplementedException();
            }

        OperationResponseDTO<Weather> IGenericRespository<Weather>.Update(string id, Weather item)
            {
            throw new NotImplementedException();
            }

        Weather IWeatherRepository.GetMaxPercipitation(WeatherFilter weatherFilter)
            {
            var filter = GenerateFilterDefinition(weatherFilter);
            return _weatherChange.Find(filter).SortByDescending(w => w.Precipitation).Limit(1).ToList().First();
            }

        Weather IWeatherRepository.GetMaxTemperature(WeatherFilter weatherFilter)
            {
            var filter = GenerateFilterDefinition(weatherFilter);
            return _weatherChange.Find(filter).SortByDescending(w => w.Temperature).Limit(1).ToList().First();
            }
        }
    }



