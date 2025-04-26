using System;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Lidhja me databazën MSSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Shtojmë Controllerat dhe Swagger
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Konfiguro pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();  // Mund ta lëmë koment për thjeshtësi
app.UseAuthorization();
app.MapControllers();
app.Run();

// Placeholder for ApplicationDbContext
internal class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}