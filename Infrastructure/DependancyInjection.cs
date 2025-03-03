using App.Core.Interface;
using Infrastructure.Database;
using Infrastructure.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static  class DependancyInjection
    {
        public static IServiceCollection AddInfraStructure(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddScoped<IAppDbContext,AppDbContext>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddSingleton<FirebaseAuthService>();
            services.AddDbContext<AppDbContext>((provider, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b=>b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            });
            return services;
        }
    }
}
