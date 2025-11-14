using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CommentableAPI.Infrastructure.Data;
using CommentableAPI.Infrastructure.Data.Seeders;
using CommentableAPI.Application.Services.Auth;
using CommentableAPI.Infrastructure.Services.Auth;
using CommentableAPI.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure Database (SQLite for development, PostgreSQL for production)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Use SQLite for development/testing
        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "commentable.db");
        options.UseSqlite($"Data Source={dbPath}");
        Console.WriteLine($"📦 Using SQLite database at: {dbPath}");
    }
    else
    {
        // Use PostgreSQL for production
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Database connection string not configured");
        options.UseNpgsql(connectionString);
        Console.WriteLine("🐘 Using PostgreSQL database");
    }
});

// Register application services
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException("JWT secret key not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger/OpenAPI
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Commentable API",
        Description = "A comprehensive REST API for managing comments with polymorphic relationships, reactions, and moderation features.",
        Contact = new OpenApiContact
        {
            Name = "API Support",
            Email = "support@commentable.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Enable annotations
    options.EnableAnnotations();

    // Add XML comments if available
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Add security definition for JWT Bearer
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

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

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSvelteKit", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "api/swagger/{documentName}/swagger.json";
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Commentable API v1");
        options.RoutePrefix = "api/swagger";
        options.DocumentTitle = "Commentable API Documentation";
        options.DisplayRequestDuration();
        options.EnableDeepLinking();
        options.EnableFilter();
        options.ShowExtensions();
        options.EnableValidator();

        // Customize UI
        options.InjectStylesheet("/swagger-ui/custom.css");
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        options.DefaultModelsExpandDepth(2);
    });
}

// HIPAA & Security Compliance Middlewares
app.UseSecurityHeaders();
app.UseAuditLogging();

app.UseHttpsRedirection();
app.UseCors("AllowSvelteKit");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Auto-create database in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        // EnsureCreated() works better for SQLite without formal migrations
        var created = await dbContext.Database.EnsureCreatedAsync();
        if (created)
        {
            Console.WriteLine("✓ Database created successfully");
        }
        else
        {
            Console.WriteLine("✓ Database already exists");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️  Database setup failed: {ex.Message}");
    }
}

// Health check endpoint
app.MapGet("/api/v1/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.0",
    environment = app.Environment.EnvironmentName
}))
.WithName("HealthCheck")
.WithTags("Health")
.Produces<object>(StatusCodes.Status200OK);

// Database seeding endpoint (development only)
app.MapPost("/api/v1/seed", async (ApplicationDbContext dbContext) =>
{
    try
    {
        var seeder = new DatabaseSeeder(dbContext);
        await seeder.SeedAllAsync();
        return Results.Ok(new { message = "Database seeded successfully" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("SeedDatabase")
.WithTags("Database")
.Produces<object>(StatusCodes.Status200OK)
.Produces<object>(StatusCodes.Status400BadRequest);

app.Run();
