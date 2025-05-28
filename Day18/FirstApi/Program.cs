

using FirstApi.Models;
using FirstApi.Repositories.Interfaces;
using FirstApi.Repositories;
using FirstApi.Services.Interfaces;
using FirstApi.Services;
using FirstApi.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IRepository<Patient>, PatientRepository>();
builder.Services.AddScoped<IRepository<Doctor>, DoctorRepository>();


builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
var app = builder.Build();



// Enable Swagger in all environments (optional)
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();



app.Run();

