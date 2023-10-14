using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using WeatherApi.Middaleware;
using WeatherApi.Models;
using WeatherApi.Models.DTOs;
using WeatherApi.Models.WeatherFilter;
using WeatherApi.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeatherApi.Controllers
    {
    [EnableCors("GooglePolicy")]
    [Route("api/[controller]")]
    [ApiController]
    //[ApiKey("ADMIN")]
    public class WeatherController : ControllerBase
        {
        private readonly IWeatherRepository _repository;
        private readonly IUserRepository _userRepository;

        public WeatherController(IWeatherRepository repository, IUserRepository userRepository)
            {
            _repository = repository;
            _userRepository = userRepository;
            }


        // public WeatherController() { }
        // GET: api/<WeatherController>
        [HttpGet]
        [HttpHead]
        public IEnumerable<Weather> Get()
            {
            return _repository.GetAll();
            }

        // GET: api/Notes/Filtered
        [HttpGet("Filtered")]
        public IEnumerable<Weather> Get([FromQuery] WeatherFilter weatherFilter)
            {
            //Sends a message to the repository ot request it to retrieve all
            //entries from the database
            return _repository.GetAll(weatherFilter);
            }

        // GET: api/Notes/Filtered
        [HttpGet("MaxPrecipitation")]
        public ActionResult GetMaxPrecipitation(string deviceName)
            {
            //Sends a message to the repository ot request it to retrieve all
            //entries from the database

            WeatherFilter weatherFilter = new WeatherFilter
                {
                DeviceName = deviceName
                };
            Weather weather = _repository.GetMaxPercipitation(weatherFilter);

            PercipitationDTO percipitationDTO = new PercipitationDTO
                {
                DeviceName = weather.DeviceName,
                Precipitation = weather.Precipitation,
                DateTime = weather.Time,
                };

            return Ok(percipitationDTO);
            }




        [HttpHead("Filtered")]
        public ActionResult GetHeaders([FromQuery] WeatherFilter filter)
            {
            var record = _repository.GetAll(filter).OrderByDescending(n => n.Time).FirstOrDefault();

            HttpContext.Response.Headers.Add("Last_Created", record.Time.ToString());
            int count = _repository.GetAll(filter).Count();
            HttpContext.Response.Headers.Add("record-count", count.ToString());

            return Ok();
            }


        // GET api/<WeatherController>/5
        [HttpGet("DataAtGivenDateTime")]
        public ActionResult GetByDeviceNameAndDataAtGivenDateTime(string deviceName, DateTime afterTime, DateTime beforeTime)
            {
            if (string.IsNullOrEmpty(deviceName))
                {
                return BadRequest();
                }
            WeatherFilter weatherFilter = new WeatherFilter
                {
                DeviceName = deviceName,
                AfterTime = afterTime,
                BeforeTime = beforeTime
                };
            return Ok(_repository.GetAll(weatherFilter));
            }




        // GET api/<WeatherController>/5
        [HttpGet("MaxTemperatureGivenDateTimeRange")]
        public ActionResult GetMaxTempForGivenDateTimeRange(DateTime afterTime, DateTime beforeTime)
            {

            WeatherFilter weatherFilter = new WeatherFilter
                {
                AfterTime = afterTime,
                BeforeTime = beforeTime
                };

            Weather weather = _repository.GetMaxTemperature(weatherFilter);

            TemperatureDTO temperatureDTO = new TemperatureDTO
                {
                DeviceName = weather.DeviceName,
                dateTime = weather.Time,
                Temperature = weather.Temperature
                };
            return Ok(temperatureDTO);
            }



        // GET api/<WeatherController>/5
        [HttpGet("{id}")]
        public ActionResult Get(string id)
            {
            if (string.IsNullOrEmpty(id))
                {
                return BadRequest();
                }
            return Ok(_repository.GetById(id));
            }

        // POST api/<WeatherController>
        [HttpPost]
        public ActionResult Post([FromBody] Weather? createWeather)
            {
            if (createWeather == null)
                {
                return BadRequest();
                }
            _repository.Create(createWeather);
            return Ok();
            }


        [HttpPost("PostMany")]
        public ActionResult PostMany([FromBody] List<Weather> createWeather)
            {
            if (createWeather == null || createWeather.Count == 0)
                {
                return BadRequest();
                }
            _repository.CreateMany(createWeather);
            return Ok();
            }



        // PUT api/<WeatherController>/5
        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] Weather updateWeather)
            {
            if (string.IsNullOrEmpty(id) || updateWeather == null)
                {
                return BadRequest();
                }
            _repository.Update(id, updateWeather);
            return Ok();
            }

        // PATCH api/Weather/UpdateMany
        [HttpPatch("UpdateMany")]
        /* public ActionResult UpdateMany([FromBody] WeatherPatchDetailsDto? details)
             {
             if (details == null)
                 {
                 return BadRequest();
                 }
             if ((details.precipitation)) &&
                  (detils.objectId)
                 {
        return BadRequest();
                 }
        //Check that we have at least 1 valid filter optoin set, otherwise the update will
            //target every document in the collection.
            if ((details.Filter.percipitationMatch) &&
              (details.Filter.objectIdMatch) &&
              details.Filter.CreatedAfter == null && details.Filter.CreatedBefore == null)
                {
                return BadRequest();
                }

            _repository.UpdateMany(details);
            return Ok();
            }*/




        // DELETE api/<WeatherController>/5
        [HttpDelete("{id}")]
        [ApiKey("ADMIN")]
        public ActionResult Delete(string id, string apiKey)
            {
            if (string.IsNullOrWhiteSpace(id))
                {
                return BadRequest();
                }
            //Process the reauest and store the details regarding the success/failure of the 
            //request
            var result = _repository.Delete(id);
            //If the request show a failure, inform the user.
            if (result.WasSuccessful == false)
                {
                return BadRequest(result);
                }
            return Ok();
            }

        /*//DELETE: api/Notes/DeleteOlderThanGivenDays
        [HttpDelete("DeleteOlderThanGivenDays")]
        public ActionResult DeleteOlderThanDays([FromQuery] int? days)
            {
            //Check if a days value is provided and that it complies with our business rules 
            if (days == null || days <= 30)
                {
                return BadRequest();
                }

            WeatherFilter filter = new WeatherFilter
                {
                //Add a created before filter to our filter details to be used for building our
                //filter definitions later. The calculation in the add days section ensures the value
                //will result in a past date, not a future one by accident.
                BeforeTime = DateTime.Now.AddDays(Math.Abs((int)days) * -1)
                };

            //Process the reauest and store the details regarding the success/failure of the 
            //request
            var result = _repository.DeleteMany(filter);
            //If the request show a failure, inform the user.
            if (result.WasSuccessful == false)
                {
                result.Message = "No records found within that range";
                return Ok(result);
                }
            //Otherwise, send an Ok(200) message
            return Ok(result);
            }*/
        /*
                [HttpDelete("DeleteByAbovePrecipitation")]
                public ActionResult DeleteByAbovePrecipitation([FromQuery] double? searchTerm)
                    {
                    //Validate our user input to ensure it meets our busines rules.
                    if (searchTerm == null || searchTerm.Length <= 3)
                        {
                        return BadRequest("This endpoint requires a search parameter provided " +
                                                                        "of more then 3 Characters ");
                        }

                    WeatherFilter filter = new WeatherFilter
                        {
                        //Create a new note filter object which takes the search term and will later be
                        //passed to the delete method to delete based upon thr term provided.
                        AbovePrecipitation = searchTerm
                        };

                    //Process the reauest and store the details regarding the success/failure of the 
                    //request
                    var result = _repository.DeleteMany(filter);
                    //If the request show a failure, inform the user.
                    if (result.WasSuccessful == false)
                        {
                        return BadRequest(result);
                        }
                    //Otherwise, send an Ok(200) message
                    return Ok(result);
                    }
        *//*
        [HttpGet("MaxPrecitiptationRecord")]
        public void precitiptationRecord<recod>.precipitionDto(string )*/
        }
    }
