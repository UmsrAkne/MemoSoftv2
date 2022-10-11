using System.Windows;
using MemoSoftv2.Models.DBs;
using MemoSoftv2.ViewModels;
using MemoSoftv2.Views;
using Prism.Ioc;
using Prism.Unity;
using Unity;

namespace MemoSoftv2
{
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
            container.RegisterSingleton(typeof(DbContextWrapper));
        }
    }
}