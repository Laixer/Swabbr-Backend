using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
using Swabbr.Infrastructure.Configuration;
using Swabbr.Infrastructure.Data.Repositories;
using Swabbr.Infrastructure.Services;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            services.AddControllers();

            // Add routing options
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            var jwtConfigSection = Configuration.GetSection("Jwt");
            var jwtConfig = jwtConfigSection.Get<JwtConfiguration>();

            var notificationHubConfigSection = Configuration.GetSection("NotificationHub");
            var wowzaStreamingCloudSection = Configuration.GetSection("WowzaStreamingCloud");

            // Add configurations
            services.Configure<JwtConfiguration>(jwtConfigSection);
            services.Configure<NotificationHubConfiguration>(notificationHubConfigSection);
            services.Configure<WowzaStreamingCloudConfiguration>(wowzaStreamingCloudSection);

            var jwtKey = Encoding.ASCII.GetBytes(jwtConfig.SecretKey);

            // Add authentication
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

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
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();
            services.AddScoped<IFollowRequestRepository, FollowRequestRepository>();
            services.AddScoped<IVlogRepository, VlogRepository>();
            services.AddScoped<IVlogLikeRepository, VlogLikeRepository>();
            services.AddScoped<ILivestreamRepository, LivestreamRepository>();

            // Configure DI for services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ILivestreamingService, LivestreamingService>();

            // DI for stores
            services.AddTransient<IUserStore<SwabbrIdentityUser>, UserStore>();
            services.AddTransient<IRoleStore<SwabbrIdentityRole>, RoleStore>();

            services.AddIdentity<SwabbrIdentityUser, SwabbrIdentityRole>(setup =>
             {
                 // TODO Determine configuration for password strength
                 setup.Password.RequireDigit = false;
                 setup.Password.RequireUppercase = false;
                 setup.Password.RequireLowercase = false;
                 setup.Password.RequireNonAlphanumeric = false;
                 setup.User.RequireUniqueEmail = true;
             })
            .AddDefaultTokenProviders();

            // TODO Keep this workaround for 'page redirect instead of status code'(?!)
            services.ConfigureApplicationCookie(o =>
            {
                o.Events = new CookieAuthenticationEvents()
                {
                    OnRedirectToLogin = (ctx) =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api", StringComparison.InvariantCultureIgnoreCase) && ctx.Response.StatusCode == 200)
                        {
                            ctx.Response.StatusCode = 401;
                        }

                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = (ctx) =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api", StringComparison.InvariantCultureIgnoreCase) && ctx.Response.StatusCode == 200)
                        {
                            ctx.Response.StatusCode = 403;
                        }

                        return Task.CompletedTask;
                    }
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
            app.UseCors(c => c
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"Swabbr v1");
            });
        }
    }
}