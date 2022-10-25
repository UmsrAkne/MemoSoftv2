using Npgsql;

namespace MemoSoftv2.Models.DBs
{
    public class DbContextWrapper
    {
        public CommentDbContext CommentDbContext { get; private set; } = new CommentDbContext();

        public NpgsqlConnectionStringBuilder ConnectionStringBuilder { get; set; } = default;

        public bool Connection => CommentDbContext != null && CommentDbContext.Database.CanConnect();

        public void RecreateDbContext()
        {
            CommentDbContext = new CommentDbContext();
            CommentDbContext.ConnectionStringBuilder.Username = ConnectionStringBuilder.Username;
            CommentDbContext.ConnectionStringBuilder.Password = ConnectionStringBuilder.Password;
            CommentDbContext.ConnectionStringBuilder.Host = ConnectionStringBuilder.Host;
            CommentDbContext.ConnectionStringBuilder.Port = ConnectionStringBuilder.Port;
            CommentDbContext.ConnectionStringBuilder.Database = ConnectionStringBuilder.Database;
            CommentDbContext.Database.EnsureCreated();
        }
    }
}