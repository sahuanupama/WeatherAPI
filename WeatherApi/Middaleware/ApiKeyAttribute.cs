using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using WeatherApi.Models;
using WeatherApi.Repository;

namespace WeatherApi.Middaleware
    {
    //Determines where the attribute class is allowed to be used.
    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    //A custom attribute that will be used to check APi keys against a required permission level.
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
        {
        //Private backing variable for our Required role property.
        private string requiredRole;
        //Property that will allow read only access to the variable from outside the class.
        public string RequiredRole
            {
            get { return requiredRole; }
            }
        //Constructor that takes the reauired role to access the attaached class or method.
        //If no role level is provided it will set the value to the highest level = 'Admin'
        public ApiKeyAttribute(string role = "ADMIN")
            {
            requiredRole = role;
            }


        //Default method that runs when this class is called as an attribute tag [ApiKey({role})]
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
            //Make sure an API key was provided with the requests
            //Accesses the HttpContext of the request, then accesses it's query string (URL).
            //Tries to find a value in the query string called 'apiKey'
            //If it finds the key, outputs it to the key variable.
            //If not, it will return false and run the if statement.
            if (context.HttpContext.Request.Query.TryGetValue("apiKey", out var key) == false)
                {
                //Send back a respose message if it fails
                context.Result = new ContentResult
                    {
                    StatusCode = 401,
                    Content = "No valid API key was provided"
                    };
                //Return back to the user and proceed no further.
                return;
                }

            //When the key is retrieved form the query, it is in a StringValues object which is puts
            //the data between curly braces e.g {keyValue}. We need to convert it back to a normal
            //string and remove the brackets before we can continue.
            var providedKey = key.ToString().Trim('{', '}');

            //Check if provided requiredRole matches valid role options
            if (Enum.TryParse(requiredRole.ToUpper(), out UserRoles neededRole) == false)
                {
                //Send back a respose message if it fails
                context.Result = new ContentResult
                    {
                    StatusCode = 500,
                    Content = "Something went wrong. Please try again later."
                    };
                //Return back to the user and proceed no further.
                return;
                }

            //Get access to our user repository by requesting it directly from the services. 
            var userRepo = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

            //Check the user's role matches the required level
            if (userRepo.AuthenticateUser(providedKey, neededRole) == null)
                {
                //Send back a respose message if it fails
                context.Result = new ContentResult
                    {
                    StatusCode = 403,
                    Content = "API key was invalid or does not have the required permissions"
                    };
                //Return back to the user and proceed no further.
                return;
                }
            //If ok let them through
            await next();

            }
        }
    }
