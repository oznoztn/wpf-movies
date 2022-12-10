using Autofac;
using MovieStorage.DataAccess;
using MovieStorage.UI.DataProvider;
using MovieStorage.UI.Dialogs;
using MovieStorage.UI.ViewModel;
using Prism.Events;

namespace MovieStorage.UI.Startup;

public class DependecyRegistrar
{
    public IContainer Register()
    {
        var builder = new ContainerBuilder();

        builder.RegisterType<EventAggregator>()
            .As<IEventAggregator>().SingleInstance();

        builder.RegisterType<MessageDialogService>()
            .As<IMessageDialogService>();

        builder.RegisterType<View.MainWindow>().AsSelf();
        builder.RegisterType<MainViewModel>().AsSelf();

        builder.RegisterType<MovieEditViewModel>()
            .As<IMovieEditViewModel>();

        builder.RegisterType<NavigationViewModel>()
            .As<INavigationViewModel>();

        builder.RegisterType<MovieDataProvider>()
            .As<IMovieDataProvider>();

        builder.RegisterType<NavigationDataProvider>()
            .As<INavigationDataProvider>();

        builder.RegisterType<FileDataService>()
            .As<IDataService>();

        return builder.Build();
    }
}