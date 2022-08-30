namespace MemoSoftv2.Models.DBs
{
    using Microsoft.EntityFrameworkCore;
    using Npgsql;

    public class CommentDbContext : DbContext
    {
        public DbSet<Comment> Comments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();

            builder.Port = 5433;
            builder.Username = "postgres";
            builder.Password = "password";
            builder.Host = "localhost";
            builder.Database = "testdb";

            optionsBuilder.UseNpgsql(builder.ToString());
        }
    }
}
