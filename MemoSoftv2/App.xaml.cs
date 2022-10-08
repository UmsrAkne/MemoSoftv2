using MemoSoftv2.Models.DBs;
using MemoSoftv2.ViewModels;
using Prism.Unity;
using Unity;

namespace MemoSoftv2
{
    using System.Windows;
    using MemoSoftv2.Views;
    using Prism.Ioc;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<ConnectionPage, ConnectionPageViewModel>();

            IUnityContainer container = containerRegistry.GetContainer();
            container.RegisterSingleton(typeof(CommentDbContext));
        }
    }
}