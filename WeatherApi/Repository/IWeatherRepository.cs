using WeatherApi.Models;
using WeatherApi.Models.WeatherFilter;

namespace WeatherApi.Repository
    {
    public interface IWeatherRepository : IGenericRespository<Weather>
        {
        IEnumerable<Weather> GetAll(WeatherFilter weatherFilter);
        void CreateMany(List<Weather> weatherList);
        void DeleteMany(WeatherFilter filter);
        //OperationResponseDTO<Weather> DeleteMany(WeatherFilter filter);
        // OperationResponseDTO<Weather> UpdateMany(WeatherPatchDetailsDto details);
        void UpdateMany(WeatherFilter filter);
        }
    }
