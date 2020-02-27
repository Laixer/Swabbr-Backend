using Dapper;
using Laixer.Identity.Dapper.Extensions;
using Laixer.Infra.Npgsql;
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
using Swabbr.Api.DapperUtility;
using Swabbr.Api.Options;
using Swabbr.Api.Services;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Services;
using Swabbr.Infrastructure.Configuration;
using Swabbr.Infrastructure.Repositories;
using Swabbr.WowzaStreamingCloud.Configuration;
using Swabbr.WowzaStreamingCloud.Services;
using System;
using System.IO;
using System.Reflection;
using System.Text;

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

            // Add configurations
            services.Configure<JwtConfiguration>(Configuration.GetSection("Jwt"));
            services.Configure<NotificationHubConfiguration>(Configuration.GetSection("NotificationHub"));
            services.Configure<WowzaStreamingCloudConfiguration>(Configuration.GetSection("WowzaStreamingCloud"));

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

            // Add postgresql database functionality
            NpgsqlSetup.Setup();
            SqlMapper.AddTypeHandler(new UriHandler());
            services.AddTransient<IDatabaseProvider, NpgsqlDatabaseProvider>();
            services.Configure<NpgsqlDatabaseProviderOptions>(options => { options.ConnectionStringName = "DatabaseInternal"; });

            // Configure DI for data repositories
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserWithStatsRepository, UserWithStatsRepository>();
            services.AddTransient<IFollowRequestRepository, FollowRequestRepository>();
            services.AddTransient<IVlogRepository, VlogRepository>();
            services.AddTransient<IVlogLikeRepository, VlogLikeRepository>();
            services.AddTransient<IReactionRepository, ReactionRepository>();
            services.AddTransient<ILivestreamRepository, LivestreamRepository>();
            services.AddTransient<INotificationRegistrationRepository, NotificationRegistrationRepository>();

            // Configure DI for services
            services.AddTransient<IUserService, UserService>();
            //services.AddTransient<IVlogService, VlogService>();
            services.AddTransient<IVlogTriggerService, VlogTriggerService>();
            //services.AddTransient<IReactionService, ReactionService>();
            services.AddTransient<IFollowRequestService, FollowRequestService>();
            services.AddTransient<ILivestreamingService, WowzaLivestreamingService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<INotificationService, NotificationServiceDummy>(); // TODO Replace
            services.AddTransient<IUserStreamingHandlingService, UserStreamingHandlingService>();

            // Configure DI for client services
            //services.AddTransient<INotificationClient, NotificationClient>();

            // Add background services
            //services.AddHostedService<VlogTriggerHostedService>();

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