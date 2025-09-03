using AcademyIO.API.Configurations;
using AcademyIO.Core.Enums;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder
    .AddJwt()
    .AddContext(EDatabases.SQLite)
    .AddRepositories()
    .AddServices()
    .AddSwaggerConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseDbMigrationHelper();

app.Run();

public partial class Program { }
