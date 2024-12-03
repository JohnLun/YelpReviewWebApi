using Microsoft.EntityFrameworkCore;
using AnimalCrossingAPI.Models;
using Microsoft.OpenApi.Models;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AnimalCrossingAPI.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AnimalCrossingAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // JWT Bearer token configuration
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false, // Don't validate the issuer
                        ValidateAudience = false, // Don't validate the audience
                        ValidateLifetime = true, // Ensure the token hasn't expired
                        ValidateIssuerSigningKey = true, // Validate the signing key
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-signing-key")) // Use your actual secret key
                    };
                });


            // Add Authorization with a scope-based policy
            builder.Services.AddAuthorization(opts =>
            {
                opts.AddPolicy("RequireEditReviewScope", policy =>
                    policy.Requirements.Add(new ScopeRequirement("write:review"))); // Scope authorization policy
            });

            // Register authorization handler
            builder.Services.AddSingleton<IAuthorizationHandler, ScopeAuthorizationHandler>();

            // Add services to the container.
            builder.Services.AddDbContext<YelpReviewDbContext>();

            builder.Services.AddControllers();

            // Configure Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opts =>
            {
                opts.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Yelp Reviews API",
                    Version = "v1",
                    Description = "An API to view reviews in the style of Yelp."
                });
                opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "AnimalCrossingApi.xml"));

                // Add JWT authorization to Swagger
                var security = new OpenApiSecurityScheme
                {
                    Name = HeaderNames.Authorization,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    In = ParameterLocation.Header,
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                opts.AddSecurityDefinition(security.Reference.Id, security);
                opts.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { security, Array.Empty<string>() }
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
