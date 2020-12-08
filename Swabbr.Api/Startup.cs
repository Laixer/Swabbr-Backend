using AutoMapper;
using Laixer.Identity.Dapper.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swabbr.Api;
using Swabbr.Api.Authentication;
using Swabbr.Api.Helpers;
using Swabbr.Core;
using Swabbr.Core.Extensions;
using Swabbr.Core.Interfaces.Factories;
using Swabbr.Infrastructure.Configuration;
using Swabbr.Infrastructure.Extensions;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Swabbr
{
    // TODO Look at this
    /// <summary>
    ///     Called on application startup to configure
    ///     service container and request pipeline.
    /// </summary>
    public class Startup
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public Startup(IConfiguration configuration) 
            => _configuration = configuration;

        /// <summary>
        ///     Sets up all our dependency injection.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // FUTURE: Uncouple from NewtonSoft.Json
            //         System.Text.Json isn't capable of handling all our types, an issue exists for this.
            services.AddControllers()
                .AddNewtonsoftJson();
            services.AddHealthChecks();

            // Setup authentication and authorization.
            SetupIdentity(services);
            SetupAuthentication(services);
            services.AddAuthorization(options =>
            {
                // FUTURE Add custom policies.
                // Note: This only expects the user to be authenticated, nothing else.
                //       This is the default for all endpoints, unless specified otherwise.
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
            });

            // Bind main configuration properties.
            services.Configure<SwabbrConfiguration>(_configuration.GetSection("Swabbr"));

            // Add core services.
            // Note: This also makes the AppContext injectable using the IAppContextFactory singleton.
            services.AddSwabbrCoreServices();

            // Add infrastructure services.
            services.AddSwabbrInfrastructureServices("DatabaseInternal", "BlobStorage");
            // Explicitly add Azure Notification Hub configuration.
            services.Configure<NotificationHubConfiguration>(_configuration.GetSection("AzureNotificationHub"));

            // Add API specific services
            services.AddOrReplace<IAppContextFactory, AspAppContextFactory>(ServiceLifetime.Singleton);
            services.AddTransient<UserUpdateHelper>();
            services.AddAutoMapper(mapper => MapperProfile.SetupProfile(mapper));

            // Add doc
            SetupSwagger(services);
        }

        /// <summary>
        ///     Configures our pipeline for requests.
        /// </summary>
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (app is null) 
            { 
                throw new ArgumentNullException(nameof(app)); 
            }
            if (env is null) 
            { 
                throw new ArgumentNullException(nameof(env)); 
            }

            // TODO This is beun
            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHsts();
            app.UseHttpsRedirection();

            app.UsePathBase(new PathString("/api"));
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health").WithMetadata(new AllowAnonymousAttribute());
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swabbr");
            });
        }

        /// <summary>
        ///     Adds swagger to the services.
        /// </summary>
        private static void SetupSwagger(IServiceCollection services) 
            => services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Swabbr", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(System.AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                // Require authorization header
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Contains the access token. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement 
                { 
                    { 
                        new OpenApiSecurityScheme 
                        { 
                            Reference = new OpenApiReference 
                            {
                                Type = ReferenceType.SecurityScheme, Id = "Bearer" 
                            } 
                        },
                        Array.Empty<string>() 
                    } 
                });
            });

        /// <summary>
        ///     Adds identity to the services.
        /// </summary>
        /// <remarks>
        ///     This uses Dapper to use identity along
        ///     with our npgsql database.
        /// </remarks>
        private void SetupIdentity(IServiceCollection services) =>
            services.AddIdentity<SwabbrIdentityUser, SwabbrIdentityRole>(setup =>
            {
                setup.Password.RequiredLength = 8;
                setup.User.RequireUniqueEmail = true;
            })
            .AddDapperStores(options =>
            {
                options.UserTable = "user";
                options.Schema = "application";
                options.MatchWithUnderscore = true;
                options.UseNpgsql<IdentityQueryRepository>(_configuration.GetConnectionString("DatabaseInternal"));
            })
            .AddDefaultTokenProviders();

        /// <summary>
        ///     Adds authentication to the services.
        /// </summary>
        private void SetupAuthentication(IServiceCollection services)
        {
            var jwtConfigSection = _configuration.GetSection("Jwt");
            var jwtConfig = jwtConfigSection.Get<JwtConfiguration>();
            var jwtKey = Encoding.ASCII.GetBytes(jwtConfig.SignatureKey);

            // Add the token service and jwt configuration
            services.AddSingleton<TokenService>();
            services.Configure<JwtConfiguration>(jwtConfigSection);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
                };
            });
        }
    }
}
