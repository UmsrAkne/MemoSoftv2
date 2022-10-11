using Npgsql;

namespace MemoSoftv2.Models.DBs
{
    public class DbContextWrapper
    {
        public CommentDbContext CommentDbContext { get; private set; } = new CommentDbContext();

        public void RecreateDbContext(NpgsqlConnectionStringBuilder conBuilder)
        {
            CommentDbContext = new ();
            CommentDbContext.ConnectionStringBuilder.Username = conBuilder.Username;
            CommentDbContext.ConnectionStringBuilder.Password = conBuilder.Password;
            CommentDbContext.ConnectionStringBuilder.Host = conBuilder.Host;
            CommentDbContext.ConnectionStringBuilder.Port = conBuilder.Port;
            CommentDbContext.ConnectionStringBuilder.Database = conBuilder.Database;
            CommentDbContext.Database.EnsureCreated();
        }
    }
}