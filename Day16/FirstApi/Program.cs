var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// Enable Swagger in all environments (optional)
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();



app.Run();

