namespace MemoSoftv2.ViewModels
{
    using Prism.Mvvm;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";
        private string inputText;

        public MainWindowViewModel()
        {
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public string InputText { get => inputText; set => SetProperty(ref inputText, value); }
    }
}
