using Microsoft.EntityFrameworkCore;
using Editari.Data;

var builder = WebApplication.CreateBuilder(args);

// Lidhja me databazën MSSQL duke përdorur AppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // opsionale
app.UseAuthorization();

app.MapControllers();
app.Run();
