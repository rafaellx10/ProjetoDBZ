using Microsoft.EntityFrameworkCore;
using ProjetoDBZ.Data;

var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte a controllers e Swagger tradicional
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("AppBdConnectionString");
builder.Services.AddDbContext<AppDbContext>(options =>
{
  options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

var app = builder.Build();

// Habilita Swagger somente em ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();