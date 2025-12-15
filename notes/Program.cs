using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using notes.Data;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Notes API",
        Version = "v1",
        Description = "API для управления заметками и авторами"
    });
});

// CORS: разрешаем все источники, методы и заголовки
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Конфигурация базы данных
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(9, 4, 0))
    )
);

var app = builder.Build();

// Проверка подключения к базе данных
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        if (context.Database.CanConnect())
        {
            logger.LogInformation("Подключение к базе данных успешно.");
            logger.LogInformation($"База данных: {context.Database.GetDbConnection().Database}");
            logger.LogInformation($"Сервер: {context.Database.GetDbConnection().DataSource}");
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

// Конвейер обработки запросов
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notes API v1");
    });
}

app.UseHttpsRedirection();
app.UseRouting();

// Используем CORS
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

// Статические файлы
app.UseDefaultFiles();
app.UseStaticFiles();

// Главная страница
app.MapGet("/", async (HttpContext context) =>
{
    var htmlPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");
    if (File.Exists(htmlPath))
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(htmlPath);
    }
    else
    {
        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync(@"
            <html>
                <head><title>Notes API</title></head>
                <body>
                    <h1>Notes API работает! ✅</h1>
                    <p><a href='/swagger'>Swagger UI</a></p>
                    <p><a href='/api/Notes'>Все заметки (API)</a></p>
                    <p><a href='/index.html'>Приложение заметок</a></p>
                </body>
            </html>
        ");
    }
});

app.Run();
