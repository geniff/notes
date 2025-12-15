using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using notes.Data;
using notes.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace notes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Получение коментариев для заметки
        // GET: api/comments/note/1
        [HttpGet("note/{noteId}")]
        public async Task<IActionResult> GetCommentsForNote(int noteId)
        {
            var comments = await _context.Comments
                .Where(c => c.NoteId == noteId)
                .OrderBy(c => c.Created)
                .Select(c => new
                {
                    c.IdComment,
                    c.NoteId,
                    c.Comment,
                    c.Created,
                    Author = _context.Authors
                                     .Where(a => a.AuthorId == c.AuthorId)
                                     .Select(a => a.Login)
                                     .FirstOrDefault() ?? "Гость"
                })
                .ToListAsync();

            return Ok(comments);
        }

        // POST: api/comments
        // Создание комментария
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] Comments comment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            comment.Created = DateTime.Now;

            if (comment.AuthorId == 0)
            {
                // Поиск гостя в базе или fallback
                var guest = await _context.Authors
                                   .FirstOrDefaultAsync(a => a.Login == "Гость");
                comment.AuthorId = guest?.AuthorId ?? 1;
            }

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            var authorName = await _context.Authors
                                           .Where(a => a.AuthorId == comment.AuthorId)
                                           .Select(a => a.Login)
                                           .FirstOrDefaultAsync() ?? "Гость";

            return Ok(new
            {
                comment.IdComment,
                comment.NoteId,
                comment.Comment,
                comment.Created,
                Author = authorName
            });
        }
    }
}
