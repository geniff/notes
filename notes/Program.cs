using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using notes.Data;
using System.Linq.Expressions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"), 
        new MySqlServerVersion(new Version(9, 4, 0))));

// Add services to the container.

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var canConnect = context.Database.CanConnect();

        if (canConnect)
        {
            logger.LogInformation("Database connection successful.");
            logger.LogInformation($"📊 База данных: {context.Database.GetDbConnection().Database}");
            logger.LogInformation($"🔗 Сервер: {context.Database.GetDbConnection().DataSource}");
        }
        else
        {
            logger.LogWarning("Database connection failed.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ошибка при подключении к базе данных.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("obj2", () => new { Автор = "Глеб", Id = 2 });
app.MapGet("/string", () => { return "Test"; });
app.MapGet("/number", () => { return 2; });
app.MapGet("/data", () => { return DateTime.Now; });
app.MapGet("/obj", () => new {День = "Вторник", Асия = "Курмаева", Время = DateTime.Now });

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors(policy =>
    policy.WithOrigins("http://localhost:32771")
          .AllowAnyMethod()
          .AllowAnyHeader());

app.UseAuthorization();

app.MapControllers();

app.Run();


// Класс контекста базы данных
namespace notes.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        // Добавление DbSet для моделей
        public DbSet<notes.Models.Author> Authors { get; set; }
        public DbSet<notes.Models.Note> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Дополнительная конфигурация моделей, если необходимо
        }

    }
}
