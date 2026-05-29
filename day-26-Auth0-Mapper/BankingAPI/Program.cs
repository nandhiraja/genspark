using BankingAPI.Contexts;
using BankingAPI.Interfaces;
using BankingAPI.Models;
using BankingAPI.Mappers;
using BankingAPI.Repositories;
using BankingAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security.AccessControl;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




#region Contexts
builder.Services.AddDbContext<BankingContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});
#endregion





#region Mappers
builder.Services.AddAutoMapper(m=> m.AddProfile(new MappingProfile()));
#endregion





#region Authenticaion
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
        ValidateLifetime = true

    };
});

builder.Services.AddAuthorization();
#endregion

#region  RateLimiting 
builder.Services.AddRateLimiter(options=>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                 options.AddFixedWindowLimiter("Fixed", limiterOptions =>
            {
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.PermitLimit = 5;
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 0;
                });
            });
#endregion



#region Repositories
builder.Services.AddScoped<IRepository<string, Account>, Repository<string,Account>>();
builder.Services.AddScoped<IRepository<int, Customer>, Repository<int, Customer>>();
builder.Services.AddScoped<IRepository<string, User>, Repository<string, User>>();
#endregion


#region Services
builder.Services.AddScoped<ICustomerInteract, CustomerService>();
builder.Services.AddScoped<IAuthenticationService, CustomerService>();
builder.Services.AddScoped<ITokenService,TokenService>();
builder.Services.AddScoped<ITransact, TransactionService>();
#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.Run();
