using System.Windows;
using UICharts.Core.Interfaces;
using UICharts.Desktop.Views;
using UICharts.Infrastructure.Services;


namespace UICharts
{
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindowView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IProjectService, ProjectService>();
        }
    }
}