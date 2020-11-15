using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SmartLabeling.Camera.Hubs;
using SmartLabeling.Camera.Models;
using SmartLabeling.Camera.Services;

namespace SmartLabeling.Camera
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartLabeling.Camera", Version = "v1" });
            });

            services.Configure<CameraSettings>(Configuration.GetSection("CameraSettings"));
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<CameraSettings>>().Value);
            services.AddSingleton<ICameraService, CameraService>();

            services.AddCors();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartLabeling.Camera v1"));
            }

            app.UseHttpsRedirection();

            app.UseCors(builder => builder.WithOrigins("http://localhost:5000").AllowAnyHeader().AllowAnyMethod().AllowCredentials());

            app.UseRouting();

            app.UseFileServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<CameraHub>("/camerahub");
                endpoints.MapControllers();
            });
        }
    }
}
