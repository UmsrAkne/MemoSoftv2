namespace MemoSoftv2.ViewModels
{
    using MemoSoftv2.Models.DBs;
    using Prism.Mvvm;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";
        private string inputText;
        private string systemMessage;

        private CommentDbContext commentDbContext = new CommentDbContext();

        public MainWindowViewModel()
        {
            try
            {
                commentDbContext.Database.EnsureCreated();
            }
            catch (Npgsql.NpgsqlException)
            {
                commentDbContext = null;
                SystemMessage = "PostgreSQL データベースへの接続に失敗しました";
            }
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public string InputText { get => inputText; set => SetProperty(ref inputText, value); }

        public string SystemMessage { get => systemMessage; set => SetProperty(ref systemMessage, value); }
    }
}
