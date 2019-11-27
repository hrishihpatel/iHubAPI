using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using iHubAPI.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace iHubAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc(options =>
            {
                options.Filters.Add(new StandardException());
                options.Filters.Add(new ModelValidationFilter());
            });
            services.AddControllers()
                    .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true) ;

            // Service Registration.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // Register All types that are ending with Test as a Transient Scope. You may want to
            // ToDO: Need to make sure the following doesn't pull in extra stuff...
            //https://andrewlock.net/using-scrutor-to-automatically-register-your-services-with-the-asp-net-core-di-container/#the-asp-net-core-di-container
            services.Scan(scan =>
            {
                scan.FromCallingAssembly()
                    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Test", StringComparison.Ordinal) && type.IsInterface))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime();
            });

            //Register Swagger
            services.RegisterSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            
            // Enable Swagger and UI.
            app.UseSwagger();
            app.UseSwaggerUI(ui =>
            {
                ui.SwaggerEndpoint(url:"/swagger/v1/swagger.json", name: "iHubAPIs");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
    public static class ServiceCollectionExtensions
    {
        public static void RegisterAllTypes<T>(this IServiceCollection services, Assembly[] assemblies, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var typesFromAssemblies = assemblies.SelectMany(a => a.DefinedTypes.Where(x => x.GetInterfaces().Contains(typeof(T))));
            foreach (var type in typesFromAssemblies)
                services.Add(new ServiceDescriptor(typeof(T), type, lifetime));
        }

        public static void RegisterSwagger(this IServiceCollection services)
        {
            //Swagger Registration
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo { Title = "iHubAPI", Version = "v1" });

                //Auth for the SwaggerPage.
                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        }, new List<string>()
                    }
                });
            });
        }
    }
}
