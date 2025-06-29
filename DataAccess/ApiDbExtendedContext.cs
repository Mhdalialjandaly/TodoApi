using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public partial class ApiDbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.SeedData();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {         
                if (string.IsNullOrWhiteSpace(ConnectionHandler.ConnectionString))
                    ConnectionHandler.ConnectionString =
                @"Server=.;Database=ApiDb;User Id=sa;Password=aaaaa12345;TrustServerCertificate=true;";

                optionsBuilder.UseSqlServer(ConnectionHandler.ConnectionString);
            
        }
    }

    public static class ConnectionHandler
    {
        public static string? ConnectionString { get; set; }
    }
}
