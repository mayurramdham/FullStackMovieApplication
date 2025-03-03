using App.Core;
using Backend.Filter;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

namespace Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins("http://localhost:4200") // Allow frontend origin
                          .AllowAnyHeader()                   // Allow any headers
                          .AllowAnyMethod()                   // Allow any HTTP methods
                          .AllowCredentials();                // Allow credentials
                });
            });
            var configuration = builder.Configuration;

            Log.Logger = new LoggerConfiguration()
                  .ReadFrom.Configuration(configuration)
                  .CreateBootstrapLogger();

            builder.Host.UseSerilog();

            // Add services to the container.
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<AppExceptionFilterAttribute>();
            });
            //  builder.Services.AddControllers();
            builder.Services.AddApplication();
            builder.Services.AddInfraStructure(configuration);
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddMicrosoftIdentityWebApiAuthentication(configuration);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();


            //adding Jwt token 
            builder.Services.AddSwaggerGen(options =>
            {
                var JwtSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Enter Your JWT Access Token",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme,
                    }
                };

                options.AddSecurityDefinition("Bearer", JwtSecurityScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {JwtSecurityScheme, Array.Empty<string>() }
                });
            });
       

            //Jwt Configuration
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "CustomAuth"; // Default to your custom JWT
                options.DefaultChallengeScheme = "CustomAuth";
                options.DefaultScheme = "CustomAuth";
            })


   // Custom JWT Authentication (Your own authentication system)
   .AddJwtBearer("CustomAuth", options =>
   {
       options.RequireHttpsMetadata = false;
       options.SaveToken = true;

       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
           ValidAudience = builder.Configuration["JwtConfig:Audience"],
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidateLifetime = false,
           ValidateIssuerSigningKey = true,
           ClockSkew = TimeSpan.Zero,
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Key"])),
       };
   })
   // Firebase Authentication
   .AddJwtBearer("Firebase", options =>
   {
       options.Authority = "https://securetoken.google.com/movieapplicationlogin";
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = true,
           ValidIssuer = "https://securetoken.google.com/movieapplicationlogin",
           ValidateAudience = true,
           ValidAudience = "movieapplicationlogin",
           ValidateLifetime = true
       };
   });

            //Authorization Policy authentication
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("CustomAuthPolicy", policy =>
                    policy.RequireAuthenticatedUser().AddAuthenticationSchemes("CustomAuth"));

                options.AddPolicy("FirebasePolicy", policy =>
                    policy.RequireAuthenticatedUser().AddAuthenticationSchemes("Firebase"));
            });


            builder.Services.AddAuthentication();

            var app = builder.Build();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images")),
                RequestPath = "/Images"
            });
            app.UseCors("AllowAll");
            app.UseAuthorization();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseAuthentication();


            app.MapControllers();

            app.Run();
        }
    }
}
