using Microsoft.OpenApi.Models;
using Kartabl_Backend.Application;
using Kartabl_Backend.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Register Core Application & Infrastructure Layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add Controllers and Swagger/OpenAPI services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Kartabl API",
        Version = "v1",
        Description = "Document Verification System API"
    });

    // 1. Define JWT Bearer Security Scheme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT Access Token"
    });

    // 2. Apply Security Requirement Globally
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Run Database Migrations & Seeding using the extension method
await app.InitialiseDatabaseAsync();

// Enable Swagger UI in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kartabl API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// Authentication MUST be placed before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();