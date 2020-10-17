using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WarehouseSystem.Options;

namespace WarehouseSystem.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUserAuthentication(this IServiceCollection services)
        {
            var options = services.BuildServiceProvider().GetService<IOptions<SecurityOptions>>().Value;
            
            services
                .AddAuthentication(opt =>
                {
                    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(options.Secret)),
                        ValidateAudience = false,
                        ValidateIssuer = false
                    };
                });

            return services;
        }
    }
}