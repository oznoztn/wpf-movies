using System.Windows;
using MovieStorage.UI.ViewModel;

namespace MovieStorage.UI.View;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;

        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _viewModel.Load();
    }
}