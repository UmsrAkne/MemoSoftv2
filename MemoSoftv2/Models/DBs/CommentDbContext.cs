namespace MemoSoftv2.Models.DBs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Npgsql;

    public class CommentDbContext : DbContext
    {
        public CommentDbContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<Comment> Comments { get; set; }

        public int SearchLimitCount { get; set; } = 100;

        public void AddComment(Comment c)
        {
            Comments.Add(c);
            SaveChanges();
        }

        public List<Comment> GetComments()
        {
            var favoriteComments = Comments.Where(c => c.IsFavorite)
                .OrderByDescending(c => c.CreationDateTime)
                .Take(SearchLimitCount)
                .ToList();

            var notFavoriteComments = Comments.Where(c => !c.IsFavorite)
                .OrderByDescending(c => c.CreationDateTime)
                .Take(SearchLimitCount)
                .ToList();

            favoriteComments.AddRange(notFavoriteComments);
            return favoriteComments;
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
