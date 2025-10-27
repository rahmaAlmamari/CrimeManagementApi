using BCrypt.Net;
using CrimeManagementApi.Data;
using CrimeManagementApi.Mappings;
using CrimeManagementApi.Middleware;
using CrimeManagementApi.Models;
using CrimeManagementApi.Models.Enums;
using CrimeManagementApi.Repositories.Implementations;
using CrimeManagementApi.Repositories.Interfaces;
using CrimeManagementApi.Services;
using CrimeManagementApi.Services.Implementations;
using CrimeManagementApi.Services.Interfaces;
using CrimeManagementApi.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;

QuestPDF.Settings.License = LicenseType.Community;

// 🔹 Optional console output for admin password hash
Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("Admin@123"));

var builder = WebApplication.CreateBuilder(args);

// 🔹 DEBUG: Check which configuration is loaded
Console.WriteLine("============================================");
Console.WriteLine($"🔹 Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"🔹 Connection String: {builder.Configuration.GetConnectionString("DefaultConnection")}");
Console.WriteLine($"🔹 DB_PASSWORD env var: {Environment.GetEnvironmentVariable("DB_PASSWORD")}");
Console.WriteLine("============================================");

// 🔹 FIX: Add Development appsettings loading
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// ======================================================
// 🔹 Logging Configuration (Serilog)
// ======================================================
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ======================================================
// 🔹 Database (PostgreSQL) - ADD RETRY LOGIC
// ======================================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// REST OF YOUR CODE STAYS THE SAME...
builder.Services.AddHttpContextAccessor();

// ======================================================
// 🔹 Dependency Injection (Repositories & Services)
// ======================================================
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICaseRepository, CaseRepository>();
builder.Services.AddScoped<IEvidenceRepository, EvidenceRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICaseService, CaseService>();
builder.Services.AddScoped<IEvidenceService, EvidenceService>();
builder.Services.AddScoped<ICrimeReportService, CrimeReportService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ILongPollingService, LongPollingService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ICaseAssigneeService, CaseAssigneeService>();
builder.Services.AddScoped<IParticipantService, ParticipantService>();
builder.Services.AddScoped<ICaseParticipantService, CaseParticipantService>();
builder.Services.AddScoped<ICaseReportService, CaseReportService>();

//=======================================================
// Add Twilio configuration
builder.Services.Configure<TwilioSettings>(
    builder.Configuration.GetSection("TwilioSettings"));

// Register SMS service
builder.Services.AddScoped<ISmsService, TwilioSmsService>();

// ======================================================
// 🔹 Email Configuration (SMTP)
// ======================================================
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// ======================================================
// 🔹 Caching + Middleware
// ======================================================
builder.Services.AddMemoryCache();
builder.Services.AddScoped<RateLimitingMiddleware>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "CrimeCache_";
});

// ======================================================
// 🔹 Controllers, AutoMapper, JSON, Compression
// ======================================================
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        // Prevent circular reference issues in JSON
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        //  Serialize enums as strings instead of numeric values
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddResponseCompression();

// ======================================================
// 🔹 CORS Policy
// ======================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:3000", "https://your-frontend-url.com")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// ======================================================
// 🔹 Health Checks
// ======================================================
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection"));

// ======================================================
// 🔹 Swagger Configuration (JWT + File Upload Support)
// ======================================================
builder.Services.AddSwaggerGen(c =>
{
    c.MapType<ClearanceLevel>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetNames(typeof(ClearanceLevel))
                    .Select(name => new OpenApiString(name))
                    .Cast<IOpenApiAny>()
                    .ToList()
    });

    // 🔸 JWT Auth setup
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter JWT token (without 'Bearer ' prefix)",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });

    // Allow non-nullable reference types
    c.SupportNonNullableReferenceTypes();

    // Enable file upload UI in Swagger
    c.OperationFilter<FileUploadOperationFilter>();
});

// ======================================================
// 🔹 JWT Authentication
// ======================================================
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

// ======================================================
// 🔹 Build Application
// ======================================================
var app = builder.Build();

// ======================================================
// 🔹 Middleware Pipeline (Secured & Ordered)
// ======================================================
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHsts();

app.UseXContentTypeOptions();
app.UseReferrerPolicy(opts => opts.NoReferrer());
app.UseXXssProtection(opts => opts.EnabledWithBlockMode());
app.UseResponseCompression();

app.UseSerilogRequestLogging();
app.UseGlobalException();
app.UseJwtValidation();
app.UseCors("AllowFrontend");
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

// ======================================================
// 🔹 Database Seeding (Default Admin, etc.)
// ======================================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(db);
}

// ======================================================
// 🔹 Run Application
// ======================================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var conn = db.Database.GetConnectionString();
    var users = await db.Users.ToListAsync();

    Console.WriteLine("============================================");
    Console.WriteLine($"[ENV] Environment: {app.Environment.EnvironmentName}");
    Console.WriteLine($"[DB] Connection: {conn}");
    Console.WriteLine($"[DB] Users count: {users.Count}");
    foreach (var u in users)
    {
        Console.WriteLine($"[USER] {u.Id} | {u.Email} | {u.Username} | {u.Role}");
    }
    Console.WriteLine("============================================");
}

app.Run();