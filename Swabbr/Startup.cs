﻿using Microsoft.AspNetCore.Authentication.Cookies;
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
using Swabbr.Infrastructure.Data;
using Swabbr.Infrastructure.Data.Entities;
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

            var jwtOptionsSection = Configuration.GetSection("Jwt");
            var jwtOptions = jwtOptionsSection.Get<JwtOptions>();

            // Configure authentication settings
            services.Configure<JwtOptions>(jwtOptionsSection);

            var jwtKey = Encoding.ASCII.GetBytes(jwtOptions.SecretKey);

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

            var connectionStringOptions = Configuration.GetSection("ConnectionStrings").Get<ConnectionStringOptions>();
            var cosmosDbOptions = Configuration.GetSection("CosmosDb").Get<CosmosDbOptions>();
            var connectionString = connectionStringOptions.ActiveConnectionString;
            var tableProperties = cosmosDbOptions.Tables;

            // Add CosmosDb. Ensure database and tables exist.
            services.AddCosmosDb(connectionString, tableProperties);

            // Configure DI for repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IFollowRequestRepository, FollowRequestRepository>();
            services.AddScoped<IVlogRepository, VlogRepository>();

            // Configure DI for services
            services.AddScoped<ITokenService, TokenService>();

            // DI for stores
            services.AddTransient<IUserStore<IdentityUserTableEntity>, UserStore>();
            services.AddTransient<IRoleStore<AppRole>, RoleStore>();

            services.AddIdentity<IdentityUserTableEntity, AppRole>(o =>
             {
                 // TODO Determine configuration
                 o.Password.RequireDigit = false;
                 o.Password.RequireUppercase = false;
                 o.Password.RequireLowercase = false;
                 o.Password.RequireNonAlphanumeric = false;
                 o.User.RequireUniqueEmail = true;
             })
            .AddDefaultTokenProviders();

            // TODO Workaround for page redirect instead of status code(!)
            services.ConfigureApplicationCookie(o =>
            {
                o.Events = new CookieAuthenticationEvents()
                {
                    OnRedirectToLogin = (ctx) =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                        {
                            ctx.Response.StatusCode = 401;
                        }

                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = (ctx) =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
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
            // For some reason
            if (/*env.IsDevelopment()*/Configuration.GetSection("ConnectionStrings").Get<ConnectionStringOptions>().Mode == ConnectionStringMode.Emulator)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production
                // scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Add CORS middleware
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"Swabbr v1 (Published on {DateTime.Now.Date.ToShortDateString()})");
            });
        }
    }
}