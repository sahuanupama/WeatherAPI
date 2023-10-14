using WeatherApi.Middaleware;

namespace WeatherApi
    {
    public class Startup
        {
        //ICoonfiguration is used to read settings and connection strings from AppSettings.json file
        public Startup(IConfiguration configuration)
            {
            Configuration = configuration;
            }
        public IConfiguration Configuration { get; }
        // ConfigureService method is a place where is a where we can register your dependent classes with the built-in Ioc container.
        //This method is called by the run time. Use this method to add services the HTTP request pipeline.

        public void ConfigureServices(IServiceCollection services)
            {
            services.AddRazorPages();
            }

        // This method is called by the runtime . use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
            if (env.IsDevelopment())
                {
                app.UseDeveloperExceptionPage();
                }
            else
                {
                app.UseExceptionHandler("/Error");
                //The default HSTS value is 30 days. you may want to change this for production scenarios,
                app.UseHsts();
                }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
            }

        }
    }
