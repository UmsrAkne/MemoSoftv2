namespace MemoSoftv2.Models.DBs
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Npgsql;

    public class CommentDbContext : DbContext
    {
        public CommentDbContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<Comment> Comments { get; set; }

        public void AddComment(Comment c)
        {
            Comments.Add(c);
            SaveChanges();
        }

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
