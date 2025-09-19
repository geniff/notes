using Microsoft.EntityFrameworkCore;

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