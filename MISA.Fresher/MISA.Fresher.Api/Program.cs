using Misa.Fresher.Infrastructure.Repository;
using MISA.Fresher.Core.Interface.Repository;
using MISA.Fresher.Core.Interface.Service;
using MISA.Fresher.Core.Middleware;
using MISA.Fresher.Core.Service;

var builder = WebApplication.CreateBuilder(args);
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

// cáu hình DI:
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // frontend
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowFrontend");
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
