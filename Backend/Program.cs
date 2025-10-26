using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddScoped<Backend.Services.PatientService>();
builder.Services.AddControllers();

// Disable automatic ProblemDetails for invalid model state to allow custom handling
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();

app.UseMiddleware<Backend.Middleware.ErrorHandlerMiddleware>();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
