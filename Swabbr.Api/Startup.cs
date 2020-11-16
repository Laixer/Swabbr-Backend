using Dapper;
using Laixer.Identity.Dapper.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swabbr.Api.Authentication;
using Swabbr.Api.Configuration;
using Swabbr.Api.Services;
using Swabbr.AzureMediaServices.Clients;
using Swabbr.AzureMediaServices.Configuration;
using Swabbr.AzureMediaServices.Extensions;
using Swabbr.AzureMediaServices.Interfaces.Clients;
using Swabbr.AzureMediaServices.Interfaces.Services;
using Swabbr.AzureMediaServices.Services;
using Swabbr.Core.Configuration;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Interfaces.Notifications;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Notifications;
using Swabbr.Core.Services;
using Swabbr.Core.Types;
using Swabbr.Core.Utility;
using Swabbr.Infrastructure.Configuration;
using Swabbr.Infrastructure.Database;
using Swabbr.Infrastructure.Notifications;
using Swabbr.Infrastructure.Notifications.JsonExtraction;
using Swabbr.Infrastructure.Providers;
using Swabbr.Infrastructure.Repositories;
using Swabbr.Infrastructure.Utility;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Swabbr
{
    /// <summary>
    /// Startup configuration for all dependency injections.
    /// </summary>
    public class Startup
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Sets up all our dependency injection.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add configurations
            services.Configure<JwtConfiguration>(_configuration.GetSection("Jwt"));
            services.Configure<NotificationHubConfiguration>(options =>
            {
                _configuration.GetSection("NotificationHub").Bind(options);
                options.ConnectionString = _configuration.GetConnectionString("AzureNotificationHub");
            });
            services.Configure<AMSConfiguration>(_configuration.GetSection("AzureMediaServices"));
            services.Configure<SwabbrConfiguration>(_configuration.GetSection("SwabbrConfiguration"));
            services.Configure<LogicAppsConfiguration>(_configuration.GetSection("LogicAppsConfiguration"));

            // Setup request related services
            services.AddCors();
            services.AddControllers(c => { }).AddNewtonsoftJson();
            services.AddRouting(options => { options.LowercaseUrls = true; });
            SetupIdentity(services);
            SetupAuthentication(services);
            services.AddApiVersioning(options => { options.ReportApiVersions = true; });

            // Setup logging and insights

#if DEBUG == false
            services.AddApplicationInsightsTelemetry();
#endif

            services.AddLogging((config) =>
            {
                config.AddAzureWebAppDiagnostics();
            });

            // Setup doc
            SetupSwagger(services);

            // Add postgresql database functionality
            NpgsqlSetup.Setup();
            services.AddTransient<IDatabaseProvider, NpgsqlDatabaseProvider>();
            services.Configure<NpgsqlDatabaseProviderOptions>(options => { options.ConnectionString = _configuration.GetConnectionString("DatabaseInternal"); });

            // Configure DI for data repositories
            services.AddTransient<IFollowRequestRepository, FollowRequestRepository>();
            services.AddTransient<ILivestreamRepository, LivestreamRepository>();
            services.AddTransient<INotificationRegistrationRepository, NotificationRegistrationRepository>();
            services.AddTransient<IReactionRepository, ReactionRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserWithStatsRepository, UserWithStatsRepository>();
            services.AddTransient<IVlogLikeRepository, VlogLikeRepository>();
            services.AddTransient<IVlogRepository, VlogRepository>();

            // Configure DI for services
            services.AddTransient<IAMSTokenService, AMSTokenService>();
            services.AddTransient<IDeviceRegistrationService, DeviceRegistrationService>();
            services.AddTransient<IFollowRequestService, FollowRequestService>();
            services.AddTransient<IHashDistributionService, HashDistributionService>();
            services.AddTransient<IHealthCheckService, HealthCheckService>();
            services.AddTransient<ILivestreamPoolService, AMSLivestreamPoolService>();
            services.AddTransient<ILivestreamService, AMSLivestreamService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<INotificationTestingService, NotificationTestingService>();
            services.AddTransient<IPlaybackService, AMSPlaybackService>();

            // TODO This seems incorrect (other services still use IReactionService)
            services.AddTransient<IReactionService, AMSReactionService>();
            services.AddTransient<IReactionWithThumbnailService, AMSReactionWithThumbnailService>();

            services.AddTransient<IStorageService, AMSStorageService>();
            services.AddTransient<ITokenService, TokenService>();

            // TODO This seems incorrect (other services still use IVlogService)
            services.AddTransient<IVlogService, VlogService>();
            services.AddTransient<IVlogWithThumbnailService, VlogWithThumbnailService>();

            services.AddTransient<IVlogTriggerService, VlogTriggerService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserStreamingHandlingService, UserStreamingHandlingService>();
            services.AddTransient<IUserWithStatsService, UserWithStatsService>();

            // Configure DI for clients and helpers
            services.AddTransient<INotificationClient, NotificationClient>();
            services.AddTransient<INotificationJsonExtractor, NotificationJsonExtractor>();
            services.AddTransient<INotificationBuilder, NotificationBuilder>();
            services.AddSingleton<IAMSClient, AMSClient>();
        }

        /// <summary>
        /// Configures our pipeline for requests.
        /// </summary>
        /// <param name="app"><see cref="IApplicationBuilder"/></param>
        /// <param name="env"><see cref="IWebHostEnvironment"/></param>
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (app == null) { throw new ArgumentNullException(nameof(app)); }
            if (env == null) { throw new ArgumentNullException(nameof(env)); }

            app.UseHsts();
            app.UseHttpsRedirection();

            app.UseRouting();

            // CORS policy
            app.UseCors(cp => cp
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            // Add authentication and authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Add Swagger API definition middleware
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swabbr v1");
            });
        }

        /// <summary>
        /// Adds swagger to the services.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        private static void SetupSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Swabbr", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                // Require authorization header
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Contains the access token. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme { Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                        Array.Empty<string>() } }
                );
            });
        }

        /// <summary>
        /// Adds identity to the services.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        private void SetupIdentity(IServiceCollection services)
        {
            // Add Identity middleware
            services.AddIdentity<SwabbrIdentityUser, SwabbrIdentityRole>(setup =>
            {
                setup.Password.RequiredLength = 6;
                setup.User.RequireUniqueEmail = true;
            })
            .AddDapperStores(options =>
            {
                options.UserTable = "user";
                options.Schema = "public";
                options.MatchWithUnderscore = true;
                options.UseNpgsql<IdentityQueryRepository>(_configuration.GetConnectionString("DatabaseInternal"));
            })
            .AddDefaultTokenProviders();
        }

        /// <summary>
        /// Adds authentication to the services.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        private void SetupAuthentication(IServiceCollection services)
        {
            var jwtConfigSection = _configuration.GetSection("Jwt");
            var jwtConfig = jwtConfigSection.Get<JwtConfiguration>();
            var jwtKey = Encoding.ASCII.GetBytes(jwtConfig.SecretKey);

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
