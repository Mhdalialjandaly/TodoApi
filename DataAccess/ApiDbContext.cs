using DataAccess.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public partial class ApiDbContext : IdentityDbContext<User>
    {
        public ApiDbContext() { }
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
            //Database.EnsureCreated();
        }

        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TodoItem>().Property
             (t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
