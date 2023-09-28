using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.PowerBI.Api.Models;
using WeatherApi.Middaleware;
using WeatherApi.Models;
using WeatherApi.Repository;
using WeatherApi.Services;
using WeatherApi.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Adds a Authorize button to the swagger docs that allows// the user to insert their
    //Api Key and store it until swagger closes.
    options.AddSecurityDefinition("apiKey", new OpenApiSecurityScheme
        {
        Description = "Enter your API Key in the box below",
        Name = "apiKey",
        In = ParameterLocation.Query,
        Type = SecuritySchemeType.ApiKey

        });

    //Makes swagger attach the Api Key from the authorise section above to every 
    //request as a paramter in the query string.
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "apiKey"
                },
                Name = "apiKey",
                In= ParameterLocation.Query
            },
            new List<string>()
        }
    });
    //Tells swagger to use the XML file and add it to your swagger docs. The XML file needs
    //to be generated by setting the 'Generate a file containing API documentation' in the
    //project settings.
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "WeatherApi.xml"));
});

//Adds the MongoConnectionSettings to the services container and sets it up to hold 
//the details of our conneciton settings in the appsettings.json file
builder.Services.Configure<MongoConnectionSettings>(builder.Configuration.GetSection("MongoConnectionSettings"));
//Adds our connection builder to the services container
builder.Services.AddScoped<MongoConnectionBuilder>();

builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("GooglePolicy", p =>
    {
        p.WithOrigins("https://www.google.com", "https://www.google.com.au");
        p.AllowAnyHeader();

        //Set the HTTP methods that these origins are allowed to send.
        p.WithMethods("GET", "PUT", "POST", "DELETE");
    });
    options.AddPolicy("youTubePolicy", p =>
    {
        p.WithOrigins("https://www.youtube.com");
        p.AllowAnyHeader();

        p.WithMethods("GET");
    });

});


var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    {
    app.UseSwagger();
    app.UseSwaggerUI();
    }

app.UseHttpsRedirection();
app.UseCors("youTubePolicy");

app.UseAuthorization();

//Simple custom middleware method that will run on every HTTP request before the
//request reaches the controller.
//Any logic after the next command will run on the way back to the user.
app.Use(async (context, next) =>
{
    Console.WriteLine("First Custom Middleware on way in.");
    //Passes control to the next item in the middleware pipeline, or the controller if no
    //more middleware items exist.
    await next();
    Console.WriteLine("First Custom Middleware on way out.");
});

//Includes our custom middleware class into the middleware pipeline. This needs to be
//included in our services so it can be referenced.
app.UseMiddleware<ExampleMiddlewareClass>();

app.MapControllers();

app.Run();
