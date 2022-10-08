using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace MemoSoftv2.ViewModels
{
    public class ConnectionPageViewModel : BindableBase, IDialogAware
    {
        private int port;
        private string databaseName;
        private string host;
        private string password;
        private string userName;

        public event Action<IDialogResult> RequestClose;

        public string Title => "Connection Page";

        public int Port { get => port; set => SetProperty(ref port, value); }

        public string DatabaseName { get => databaseName; set => SetProperty(ref databaseName, value); }

        public string Host { get => host; set => SetProperty(ref host, value); }

        public string Password { get => password; set => SetProperty(ref password, value); }

        public string UserName { get => userName; set => SetProperty(ref userName, value); }

        public DelegateCommand CloseCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(default);
        });

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}