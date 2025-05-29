
using SecondWebApi.Contexts;
using Microsoft.EntityFrameworkCore;
using SecondWebApi.Interfaces;
using SecondWebApi.Models;
using SecondWebApi.Repositories;
using SecondWebApi.Interfaces.Mappers;
using SecondWebApi.Services.Mappers;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<ClinicContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IRepository<int, Doctor>, DoctorRepository>();
builder.Services.AddScoped<IRepository<int, Patient>, PatientRepository>();
builder.Services.AddScoped<IRepository<string, Appointment>, AppointmentRepository>();
builder.Services.AddScoped<IRepository<int, Speciality>, SpecialityRepository>();
builder.Services.AddScoped<IRepository<int, DoctorSpeciality>, DoctorSpecialityRepository>();

builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<ISpecialityService, SpecialityService>();
builder.Services.AddScoped<IPatientService, PatientService>();

builder.Services.AddScoped<IDoctorSpecialityMapper, DoctorSpecialityMapper>();
builder.Services.AddScoped<IDoctorMapper, DoctorMapper>();
builder.Services.AddScoped<ISpecialityMapper, SpecialityMapper>();



builder.Services.AddControllers().AddJsonOptions(options =>
 {
     options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
     options.JsonSerializerOptions.WriteIndented = true;
 });



var app = builder.Build();


// builder.Services.AddScoped<AppointmentService>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();




app.Run();
