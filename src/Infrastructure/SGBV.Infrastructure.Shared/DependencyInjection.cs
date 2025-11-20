using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SGBV.Application.DTOs;
using SGBV.Application.Interfaces.Services;
using SGBV.Domain.Settings;
using SGBV.Infrastructure.Shared.Services;

namespace SGBV.Infrastructure.Shared;

public static class DependencyInjection
{
    public static void AddSharedLayer(this IServiceCollection services, IConfiguration configuration)
    {
        #region Settings
        
        services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));
            
        #endregion
        
        #region JWT

        services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = configuration["JWTSettings:Issuer"],
                ValidAudience = configuration["JWTSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"] ?? string.Empty))
            };
            options.Events = new JwtBearerEvents()
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new JWTResponse(true, "The token has expired"));
                        return context.Response.WriteAsync(result);
                    }

                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var generalResult = JsonConvert.SerializeObject(new JWTResponse(true, "Invalid token or authentication error"));
                    return context.Response.WriteAsync(generalResult);
                },
                
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var result = JsonConvert.SerializeObject(new JWTResponse(true, "An unexpected error occurred during authentication"));
                    return context.Response.WriteAsync(result);
                },
                OnForbidden = context =>
                {
                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";
                    var result = JsonConvert.SerializeObject(new JWTResponse(true,
                        "You are not authorized to access this content"));

                    return context.Response.WriteAsync(result);
                }
            };

        });
            
        #endregion
        
        #region Services
        
        services.AddScoped<IAuthService, AuthService>();
            
        #endregion
    }
}
