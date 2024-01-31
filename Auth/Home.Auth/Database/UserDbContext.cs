using Home.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;

namespace Home.Auth.Database {
    public class UserDbContext : DbContext {
        private readonly string databaseFilePath;
        private readonly string connectionString;

        public DbSet<User> Users { get; set; }

        public UserDbContext() {
            var filePath = Path.Combine(Environment.CurrentDirectory, "temp.db");
            connectionString = $"Data Source={filePath}";
        }

        public UserDbContext(string connectionString) {
            this.connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(connectionString);
    }
}
