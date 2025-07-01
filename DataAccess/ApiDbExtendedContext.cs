using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public partial class ApiDbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder) {
            modelBuilder.SeedData();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            // Uncomment the next line only when adding a new migration
            //optionsBuilder.UseSqlServer("Server=localhost,1433;Database=ToDoAppDb;User Id=sa;Password=aaaaa12345;TrustServerCertificate=true;");
        }
    }
}
