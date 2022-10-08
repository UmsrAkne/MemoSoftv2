using Prism.Mvvm;

namespace MemoSoftv2.ViewModels
{
    public class ConnectionPageViewModel : BindableBase
    {
        private int port;
        private string databaseName;
        private string host;
        private string password;
        private string userName;

        public int Port { get => port; set => SetProperty(ref port, value); }

        public string DatabaseName { get => databaseName; set => SetProperty(ref databaseName, value); }

        public string Host { get => host; set => SetProperty(ref host, value); }

        public string Password { get => password; set => SetProperty(ref password, value); }

        public string UserName { get => userName; set => SetProperty(ref userName, value); }
    }
}