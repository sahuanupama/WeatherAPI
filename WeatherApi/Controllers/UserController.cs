using Microsoft.AspNetCore.Mvc;
using WeatherApi.Models;
using WeatherApi.Models.DTOs;
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
            /*if(IsAuthenticated(apiKey, UserRoles.ADMIN) == false)
            {
                return Unauthorized("You ate not Authorise to access ");
            
            } */
            var user = new ApiUser
                {
                Name = userDTo.Name,
                Email = userDTo.Email,
                Role = userDTo.Role,
                Active = true,
                LastAccess = userDTo.LastAccess,
                APIKey = apiKey
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



        }
    }
