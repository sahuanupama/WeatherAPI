namespace WeatherApi.Middaleware
    {
    //A custom middleware class that can be added to the middleware pipeline.
    //Uses the IMiddleware interface to ensure it has the right method to be conmapitble with
    //the pipeline.
    //NOTE: To use this middleware you need to add it to the services and add it in the correct
    //position of the Use sections of the program.cs class.
    public class ExampleMiddlewareClass : IMiddleware
        {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
            {
            Console.WriteLine("2nd Middleware class on way in.");
            //Passes control to the next item in the middleware pipeline, or the controller if no
            //more middleware items exist.
            await next(context);
            Console.WriteLine("2nd Middleware class on way out.");
            }
        }
    }
