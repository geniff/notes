using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using notes.Data;
using notes.Models;
using System.Linq.Expressions;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Правильная конфигурация Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Notes API",
        Version = "v1",
        Description = "API для управления заметками и авторами"
    });
});

builder.Services.AddCors();

// Конфигурация базы данных
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(9, 4, 0))));

var app = builder.Build();

// Проверка подключения к базе данных
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var canConnect = context.Database.CanConnect();

        if (canConnect)
        {
            logger.LogInformation("Подключение к базе данных успешно.");
            logger.LogInformation($"📊 База данных: {context.Database.GetDbConnection().Database}");
            logger.LogInformation($"🔗 Сервер: {context.Database.GetDbConnection().DataSource}");

        }
        else
        {
            logger.LogWarning("Не удалось подключиться к базе данных.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ошибка при подключении к базе данных.");
    }
}

// Настройка конвейера запросов
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notes API v1");
    });
}

// Маршруты API
app.MapGet("obj2", () => new { Автор = "Глеб", Id = 2 });
app.MapGet("/string", () => new DayModel().GetDay());
app.MapGet("/number", () => { return 2; });
app.MapGet("/data", () => { return DateTime.Now; })
    .WithOpenApi(operation => 
    {
        operation.Summary = "Получить текущую дату и время";
        operation.Description = "Этот эндпоинт возвращает текущую дату и время сервера.";
        return operation;
    });
app.MapGet("/obj", () => new { День = "Вторник", Асия = "Курмаева", Время = DateTime.Now });

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());

app.UseAuthorization();
app.MapControllers();
app.Run();