using System.Text.Json.Serialization;
using DoctorAppointment.Contexts;
using DoctorAppointment.Interfaces;
using DoctorAppointment.Models;
using DoctorAppointment.Repositories;
using DoctorAppointment.Service;
using DoctorAppointment.Services;
using FirstAPI.Repositories;
using FirstAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DoctorAppointment.Misc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using DoctorAppointment.Hubs;
using DoctorAppointment.Policies;

var builder = WebApplication.CreateBuilder(args);

// Add OpenAPI/Swagger
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Clinic API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// Add Controllers and JSON options
builder.Services.AddControllers(options =>
{
    options.Filters.Add<CustomExceptionFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

// Add DbContext with PostgreSQL
builder.Services.AddDbContext<ClinicContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Logging
builder.Logging.AddLog4Net();

// Register repositories, services, filters
builder.Services.AddScoped<PatientRepo>();
builder.Services.AddScoped<SpecialityRepo>();
builder.Services.AddScoped<IPatientService, PatientServices>();
builder.Services.AddScoped<ISpecialityServices, SpecialityService>();
builder.Services.AddScoped<IRepository<int, Doctor>, DoctorRepo>();
builder.Services.AddScoped<IRepository<int, Speciality>, SpecialityRepo>();
builder.Services.AddScoped<IRepository<int, DoctorSpeciality>, DoctorSpecialityRepo>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddTransient<IRepository<string, User>, UserRepository>();
builder.Services.AddTransient<IEncryptionService, EncryptionService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddAutoMapper(typeof(User));
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<AppointmentRepo>();
builder.Services.AddScoped<CustomExceptionFilter>();
builder.Services.AddScoped<IRepository<int, Patient>, PatientRepo>();

// CORS policy name
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add SignalR
builder.Services.AddSignalR();

// Authorization policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ExperiencedDoctorOnly", policy =>
        policy.Requirements.Add(new MinimumExperienceRequirement(3)));
});
builder.Services.AddSingleton<IAuthorizationHandler, MinimumExperienceHandler>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Keys:JwtTokenKey"]))
        };
    });

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
