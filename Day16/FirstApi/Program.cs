

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

builder.Services.AddSingleton<IRepository<Doctor>, DoctorRepository>();
builder.Services.AddSingleton<IRepository<Patient>, PatientRepository>();
builder.Services.AddSingleton<IRepository<Appointment>, AppointmentRepository>();

builder.Services.AddSingleton<IService<Doctor>, DoctorService>();
builder.Services.AddSingleton<IService<Patient>, PatientService>();
builder.Services.AddSingleton<IService<Appointment>, AppointmentService>();

var app = builder.Build();



// Enable Swagger in all environments (optional)
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();



app.Run();

