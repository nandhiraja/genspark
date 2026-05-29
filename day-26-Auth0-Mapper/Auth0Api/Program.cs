using Auth0.AspNetCore.Authentication.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add Auth0 services to the container (Updated syntax for the latest Beta)
builder.Services.AddAuth0ApiAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"];
    options.Audience = builder.Configuration["Auth0:Audience"]; 
});


builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Order matters here! Authentication must sit exactly above Authorization
app.UseAuthentication();
app.UseAuthorization();

// --- TEST ENDPOINTS ---

// Public endpoint - anyone can access this
app.MapGet("/api/public", () => 
    Results.Ok(new { Message = "This endpoint is public" }))
    .WithName("GetPublic");

// Protected endpoint - requires a valid token from Auth0
app.MapGet("/api/private", () => 
    Results.Ok(new { Message = "This endpoint requires authentication" }))
    .RequireAuthorization()
    .WithName("GetPrivate");

app.Run();