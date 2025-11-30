using SGBV.Application;
using SGBV.Infrastructure.API.ServicesExtension;
using SGBV.Infrastructure.Persistence;
using SGBV.Infrastructure.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddFilters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Persistence
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplicationLayer(builder.Configuration);
builder.Services.AddSharedLayer(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseGlobalException();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();