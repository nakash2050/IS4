using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.AuthUtils.PolicyProvider;
using Api.Services;
using Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins("https://localhost:5003", "http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddControllers();

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.SaveToken = true;
                    options.Authority = "https://localhost:5001";
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidAudiences = new[] { "invoice", "customer", "https://localhost:5001/resources" }
                    };
                    // An alternate to options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents as DI Service is not resolved if option.EventType is used 
                    options.EventsType = typeof(CustomJwtBearerEvents);
                });

            // Register our custom Authorization handler
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

            // Overrides the DefaultAuthorizationPolicyProvider with our own
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

            services.AddAuthorization(options =>
            {
                // One static policy - All users must be authenticated
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();

                // options.AddPolicy("customer", policy =>
                // {
                //     policy.RequireAssertion(context => { 
                //         var authorizationFilterContext = context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext;

                //         var result = context.User.HasClaim(c => c.Type == "permissions" && (c.Value == "customer.read" || c.Value == "customer.contact"));

                //         if (!result)
                //         {
                //             authorizationFilterContext.Result = new JsonResult("Custom message") { StatusCode = 401 };
                //             context.Fail();
                //         }

                //         return result;
                //     });
                // });
                // options.AddPolicy("invoice", policy =>
                // {
                //     policy.RequireAssertion(context => context.User.HasClaim(c => c.Type == "permissions" && (c.Value == "invoice.read" || c.Value == "invoice.pay")));
                // });

            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
            });

            services.AddHttpContextAccessor();
            services.AddScoped<IUserInfo, UserInfo>();
            services.AddTransient<CustomJwtBearerEvents>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("default");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
            });
        }
    }
}
