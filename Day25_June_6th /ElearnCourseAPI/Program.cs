using ElearnAPI;
using ElearnAPI.Data;
using ElearnAPI.Interfaces;
using ElearnAPI.Interfaces.Repositories;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.Middleware;
using ElearnAPI.Repositories;
using ElearnAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using ElearnAPI.SignalR;
using System.Security.Claims;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Serilog;
using Microsoft.Extensions.FileProviders;




var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddDbContext<ElearnDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IUploadRepository, UploadRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IUserFileProgressRepository, UserFileProgressRepository>();
builder.Services.AddScoped<IUserFileProgressRepository, UserFileProgressRepository>();
builder.Services.AddScoped<ICouponCodeRepository, CouponCodeRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();










// Services
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IUserFileProgressService, UserFileProgressService>();
builder.Services.AddScoped<ICouponCodeService, CouponCodeService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();





builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;


    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        context.HttpContext.Response.StatusCode = 429;

        var errorResponse = new
        {
            success = false,
            message = "Rate limit exceeded. Please try again later."
        };

        await context.HttpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken: token);
    };


    options.AddPolicy("StudentPolicy", context =>
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous_student";

        return RateLimitPartition.GetTokenBucketLimiter<string>(
            userId,
            _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = 100,
                TokensPerPeriod = 100,
                ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                AutoReplenishment = true,
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            });
    });


    options.AddPolicy("InstructorPolicy", context =>
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous_instructor";

        return RateLimitPartition.GetTokenBucketLimiter<string>(
            userId,
            _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = 3,
                TokensPerPeriod = 3,
                ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                AutoReplenishment = true,
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            });
    });


});



// JWT Authentication
var jwtConfig = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtConfig["Key"]!);

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig["Issuer"],
        ValidAudience = jwtConfig["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    opt.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/notificationHub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };

    
});



var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Update this as per your frontend port
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for SignalR
    });
});


// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog();


//https

// builder.Services.AddHttpsRedirection(options =>
// {
//     options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
//     options.HttpsPort = 7255;
// });

// builder.WebHost.ConfigureKestrel(options =>
// {
//     options.ListenAnyIP(5295); // HTTP
//     options.ListenAnyIP(7255, listenOptions =>
//     {
//         listenOptions.UseHttps("certs/dev-cert.pfx", "password123");
//     });
// });


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "E-learning API", Version = "v1" });

    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT"
    });

    o.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Middleware pipeline
app.UseMiddleware<ErrorHandlingMiddleware>();

// Use rate limiter
app.UseRateLimiter();

app.UseCors("FrontendPolicy");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();



app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "UploadedThumbnails")),
    RequestPath = "/UploadedThumbnails"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
    RequestPath = "/uploads"
});










app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

app.Run();


