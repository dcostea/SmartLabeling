using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SmartLabeling.Sensors.Models;

namespace SmartLabeling.Sensors
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSignalR();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartLabeling.Sensors", Version = "v1" });
            });

            services.Configure<SensorsSettings>(Configuration.GetSection("SensorsSettings"));
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<SensorsSettings>>().Value);

            services.AddCors();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartLabeling.Sensors v1"));
            }

            app.UseCors(builder => builder.WithOrigins("http://localhost:5000").AllowAnyHeader().AllowAnyMethod().AllowCredentials());

            app.UseFileServer();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<SensorHub>("/sensorhub");
                endpoints.MapControllers();
            });
        }
    }
}
