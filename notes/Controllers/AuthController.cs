namespace notes.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using notes.Data;
    using notes.Models;
    using System.Security.Cryptography;
    using System.Text;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // DI 
        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new AuthResponse { Message = "Логин и пароль обязательны" });
                }

                if (request.Password.Length < 6)
                {
                    return BadRequest(new AuthResponse { Message = "Пароль должен быть не менее 6 символов" });
                }

                // Проверка существующего пользователя
                if (await _context.Authors.AnyAsync(a => a.Login == request.Login))
                {
                    return BadRequest(new AuthResponse { Message = "Пользователь с таким логином уже существует" });
                }

                // Создание пользователя
                var author = new Author
                {
                    Login = request.Login,
                    PasswordHash = HashPassword(request.Password),
                    Rights = 'u' // обычный пользователь
                };

                _context.Authors.Add(author);
                await _context.SaveChangesAsync();

                // Скрываем пароль для ответа
                author.PasswordHash = "***";

                return Ok(new AuthResponse
                {
                    Message = "Регистрация успешна",
                    Author = author
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResponse { Message = $"Ошибка сервера: {ex.Message}" });
            }
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new AuthResponse { Message = "Логин и пароль обязательны" });
                }

                // Поиск пользователя
                var author = await _context.Authors
                    .FirstOrDefaultAsync(a => a.Login == request.Login);

                if (author == null || !VerifyPassword(request.Password, author.PasswordHash))
                {
                    return Unauthorized(new AuthResponse { Message = "Неверный логин или пароль" });
                }

                // Скрываем пароль для ответа
                author.PasswordHash = "***";

                return Ok(new AuthResponse
                {
                    Message = "Авторизация успешна",
                    Author = author
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResponse { Message = $"Ошибка сервера: {ex.Message}" });
            }
        }

        // хэширую пароль
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // сверяю пароль с хэшем
        private bool VerifyPassword(string password, string passwordHash)
        {
            return HashPassword(password) == passwordHash;
        }
    }
}
