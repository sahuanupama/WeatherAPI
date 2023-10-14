using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using WeatherApi.Middaleware;
using WeatherApi.Models;
using WeatherApi.Models.DTOs;
using WeatherApi.Models.Filter;
using WeatherApi.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeatherApi.Controllers
    {
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
        {
        public readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
            {
            _userRepository = userRepository;
            }

        //Ckeck if the user apiKey meets the required level(Admin Access) to add a
        //new user to the system.

        // POST api/<UserController>
        [HttpPost]
        public ActionResult CreateUser(string apiKey, UserCreateDTO userDTo)
            {
            if (IsAuthenticated(apiKey, UserRoles.ADMIN) == false)
                {
                return Unauthorized("You ate not Authorise to access ");
                }
            var user = new ApiUser
                {
                Name = userDTo.Name,
                Email = userDTo.Email,
                Role = userDTo.Role,
                Active = true,
                };

            var result = _userRepository.CreateUser(user);
            return Ok();
            }

        private bool IsAuthenticated(string apiKey, UserRoles requiredRole)
            {
            //Check if the user is authenticated and meets the required role level
            //for the action they are trying to perform
            if (_userRepository.AuthenticateUser(apiKey, requiredRole) == null)
                {
                return false;
                }
            _userRepository.UpdateLastLogin(apiKey);
            return true;
            }

        //User find by id and provide APIkey, if both match allow to delete user    
        [HttpDelete("{id}")]
        public ActionResult DeleteUser(string id, string apiKey)
            {
            if (String.IsNullOrEmpty(id))
                {
                return BadRequest();
                }
            if (IsAuthenticated(apiKey, UserRoles.ADMIN) == false)
                {
                return Unauthorized("you are not authorised");
                }

            _userRepository.Delete(id);
            return Ok();
            }


        [HttpDelete("DeleteOlderThan30Days")]
        public ActionResult DeleteUserOlderThanDays(string apikey)
            {
            var lastLoginDays = 30;
            if (IsAuthenticated(apikey, UserRoles.ADMIN) == false)
                {
                return Unauthorized("You ate not Authorise to access ");
                }

            UserFilter userFilter = new UserFilter
                {
                LastAccess = DateTime.Now.AddDays(Math.Abs((int)lastLoginDays) * -1),
                };
            _userRepository.DeleteMany(userFilter);
            return Ok();
            }

        }
    }

