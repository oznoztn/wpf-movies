using System.Windows;
using Autofac;
using MovieStorage.UI.Startup;

namespace MovieStorage.UI;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var bootStrapper = new DependecyRegistrar();
        var container = bootStrapper.Register();

        var mainWindow = container.Resolve<View.MainWindow>();
        mainWindow.Show();
    }
}