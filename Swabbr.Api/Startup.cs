using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swabbr.Api.Authentication;
using Swabbr.Api.Extensions;
using Swabbr.Api.Options;
using Swabbr.Api.Services;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Services;
using Swabbr.Infrastructure.Configuration;
using Swabbr.Infrastructure.Repositories;
using Swabbr.Infrastructure.Services;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using WowzaStreamingCloud.Configuration;

namespace Swabbr
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
            services.AddCors();
            services.AddControllers(c =>
            {
            }).AddNewtonsoftJson();

            // Add routing options
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            var jwtConfigSection = Configuration.GetSection("Jwt");
            var jwtConfig = jwtConfigSection.Get<JwtConfiguration>();
            var jwtKey = Encoding.ASCII.GetBytes(jwtConfig.SecretKey);

            var notificationHubConfigSection = Configuration.GetSection("NotificationHub");
            var wowzaStreamingCloudSection = Configuration.GetSection("WowzaStreamingCloud");

            // Add configurations
            services.Configure<JwtConfiguration>(jwtConfigSection);
            services.Configure<NotificationHubConfiguration>(notificationHubConfigSection);
            services.Configure<WowzaStreamingCloudConfiguration>(wowzaStreamingCloudSection);

            // Add OpenAPI definition
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

            var cosmosDbConfig = Configuration.GetSection("CosmosDb").Get<CosmosDbConfiguration>();
            var connectionStringConfig = cosmosDbConfig.ConnectionStrings;
            var connectionString = connectionStringConfig.ActiveConnectionString;
            var tableProperties = cosmosDbConfig.Tables;

            // Add CosmosDb. Ensure database and tables exist.
            services.AddCosmosDb(connectionString, tableProperties);

            // Configure DI for data repositories
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IFollowRequestRepository, FollowRequestRepository>();
            services.AddTransient<IVlogRepository, VlogRepository>();
            services.AddTransient<IVlogLikeRepository, VlogLikeRepository>();
            services.AddTransient<IReactionRepository, ReactionRepository>();
            services.AddTransient<ILivestreamRepository, LivestreamRepository>();
            services.AddTransient<INotificationRegistrationRepository, NotificationRegistrationRepository>();

            // Configure DI for services
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IVlogService, VlogService>();
            services.AddTransient<IReactionService, ReactionService>();
            services.AddTransient<IFollowRequestService, FollowRequestService>();
            services.AddTransient<ILivestreamingService, LivestreamingService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<ITokenService, TokenService>();

            // Configure DI for client services
            services.AddTransient<INotificationClient, NotificationClient>();
            services.AddTransient<ILivestreamingClient, LivestreamingClient>();

            // Configure DI for identity stores
            services.AddTransient<IUserStore<SwabbrIdentityUser>, UserStore>();
            services.AddTransient<IRoleStore<SwabbrIdentityRole>, RoleStore>();

            // Add background services
            services.AddHostedService<VlogTriggerHostedService>();

            // Add identity middleware
            services.AddIdentity<SwabbrIdentityUser, SwabbrIdentityRole>(setup =>
             {
                 //TODO: Determine configuration for password strength
                 setup.Password.RequireDigit = false;
                 setup.Password.RequireUppercase = false;
                 setup.Password.RequireLowercase = false;
                 setup.Password.RequireNonAlphanumeric = false;
                 setup.User.RequireUniqueEmail = true;
             })
            .AddDefaultTokenProviders();

            // Add authentication middleware
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