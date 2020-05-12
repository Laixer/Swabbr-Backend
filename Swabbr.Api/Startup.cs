using Dapper;
using Laixer.Identity.Dapper.Extensions;
using Laixer.Infra.Npgsql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swabbr.Api.Authentication;
using Swabbr.Api.Configuration;
using Swabbr.Api.Services;
using Swabbr.Api.Utility;
using Swabbr.AzureMediaServices.Clients;
using Swabbr.AzureMediaServices.Configuration;
using Swabbr.AzureMediaServices.Extensions;
using Swabbr.AzureMediaServices.Interfaces.Clients;
using Swabbr.AzureMediaServices.Services;
using Swabbr.Core.Configuration;
using Swabbr.Core.Factories;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Interfaces.Factories;
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
using Swabbr.Infrastructure.Repositories;
using Swabbr.Infrastructure.Utility;
using Swabbr.WowzaStreamingCloud.Configuration;
using Swabbr.WowzaStreamingCloud.Services;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Swabbr
{

    /// <summary>
    /// TODO Clean this mess up
    /// </summary>
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
            services.AddCors();
            services.AddControllers(c => { }).AddNewtonsoftJson();
            services.AddRouting(options => { options.LowercaseUrls = true; });

            // Add mvc for antiforgery token usage
            services.AddMvc();

            // Setup logging explicitly
            services.AddLogging((config) =>
            {
                config.AddAzureWebAppDiagnostics();
            });

            // Add Swagger
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

            // Add configurations
            services.Configure<JwtConfiguration>(Configuration.GetSection("Jwt"));
            services.Configure<NotificationHubConfiguration>(options =>
            {
                Configuration.GetSection("NotificationHub").Bind(options);
                options.ConnectionString = Configuration.GetConnectionString("AzureNotificationHub");
            });
            services.Configure<AMSConfiguration>(Configuration.GetSection("AzureMediaServices"));
            services.Configure<SwabbrConfiguration>(Configuration.GetSection("SwabbrConfiguration"));
            services.Configure<LogicAppsConfiguration>(Configuration.GetSection("LogicAppsConfiguration"));

            // Check configuration
            var servicesBuilt = services.BuildServiceProvider();
            servicesBuilt.GetRequiredService<IOptions<SwabbrConfiguration>>().Value.ThrowIfInvalid();
            servicesBuilt.GetRequiredService<IOptions<NotificationHubConfiguration>>().Value.ThrowIfInvalid();
            servicesBuilt.GetRequiredService<IOptions<AMSConfiguration>>().Value.ThrowIfInvalid();
            servicesBuilt.GetRequiredService<IOptions<LogicAppsConfiguration>>().Value.ThrowIfInvalid();

            // Add postgresql database functionality
            NpgsqlSetup.Setup();
            // SqlMapper.AddTypeHandler(new FollowRequestStatusHandler()); // TODO Look at this
            services.AddTransient<IDatabaseProvider, NpgsqlDatabaseProvider>();
            services.Configure<NpgsqlDatabaseProviderOptions>(options => { options.ConnectionString = Configuration.GetConnectionString("DatabaseInternal"); });

            // Configure DI for data repositories
            services.AddTransient<IFollowRequestRepository, FollowRequestRepository>();
            services.AddTransient<ILivestreamRepository, LivestreamRepository>();
            services.AddTransient<INotificationRegistrationRepository, NotificationRegistrationRepository>();
            services.AddTransient<IReactionRepository, ReactionRepository>();
            services.AddTransient<IRequestRepository, RequestRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserWithStatsRepository, UserWithStatsRepository>();
            services.AddTransient<IVlogLikeRepository, VlogLikeRepository>();
            services.AddTransient<IVlogRepository, VlogRepository>();

            // Configure DI for services
            services.AddTransient<IDeviceRegistrationService, DeviceRegistrationService>();
            services.AddTransient<IFollowRequestService, FollowRequestService>();
            services.AddTransient<IHashDistributionService, HashDebugDistributionService>();
            services.AddTransient<ILivestreamPlaybackService, AMSLivestreamPlaybackService>();
            services.AddTransient<ILivestreamPoolService, AMSLivestreamPoolService>();
            services.AddTransient<ILivestreamService, AMSLivestreamService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<INotificationTestingService, NotificationTestingService>(); // TODO Remove
            services.AddTransient<IReactionService, ReactionService>();
            services.AddTransient<IReactionUploadService, ReactionUploadService>();
            services.AddTransient<IStorageService, AMSStorageService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<ITranscodingService, AMSTranscodingService>();
            services.AddTransient<IVlogService, VlogService>();
            services.AddTransient<IVlogTriggerService, VlogTriggerService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserStreamingHandlingService, UserStreamingHandlingService>();
            services.AddTransient<IUserWithStatsService, UserWithStatsService>();

            // Configure DI for client services
            services.AddTransient<INotificationClient, NotificationClient>();
            services.AddTransient<INotificationJsonExtractor, NotificationJsonExtractor>();
            services.AddTransient<INotificationBuilder, NotificationBuilder>();
            services.AddTransient<IAMSClient, AMSClient>();
            services.AddSingleton<IHttpClientFactory, HttpClientFactory>();

            // TODO Debug remove
            services.AddTransient<AMSDebugService>();

            // Add Identity middleware
            services.AddIdentity<SwabbrIdentityUser, SwabbrIdentityRole>(setup =>
            {
                setup.Password.RequireDigit = true;
                setup.Password.RequireUppercase = true;
                setup.Password.RequireLowercase = true;
                setup.Password.RequireNonAlphanumeric = true;
                setup.Password.RequiredLength = 8;
                setup.User.RequireUniqueEmail = true;
            })
            .AddDapperStores(options =>
            {
                options.UserTable = "user";
                options.Schema = "public";
                options.MatchWithUnderscore = true;
                options.UseNpgsql<IdentityQueryRepository>(Configuration.GetConnectionString("DatabaseInternal"));
            })
            .AddDefaultTokenProviders();

            // TODO Remove all this debug stuff
            //var logger = services.BuildServiceProvider().GetService<ILoggerFactory>().CreateLogger("startup logger");
            //logger.LogError("REMOVE THIS CONFIG PRINTER!");
            //logger.LogError($"Existing config items: {Configuration.AsEnumerable().Count()}");
            //logger.LogError($"DailyVlogRequestLimit = {Configuration.GetSection("SwabbrConfiguration").GetValue<int>("DailyVlogRequestLimit")}");
            //foreach (var pair in Configuration.AsEnumerable())
            //{
            //    logger.LogError($"Configuration pair exists for key: {pair.Key}");
            //}
            //logger.LogError("FINISHED CONFIG PRINTING");
            //var swabbrConfiguration = Configuration.GetSection("SwabbrConfiguration");

            // Add authentication middleware
            // TODO Double get
            var jwtConfigSection = Configuration.GetSection("Jwt");
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // The default HSTS value is 30 days. You may want to change this for production
            // scenarios, see https://aka.ms/aspnetcore-hsts.
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

            app.UsePathBase(new PathString("/api/v1/"));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Add Swagger API definition middleware
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"Swabbr v1");
            });
        }
    }
}