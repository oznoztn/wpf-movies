using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MovieStorage.UI.Command;
using MovieStorage.UI.Events;
using Prism.Events;

namespace MovieStorage.UI.ViewModel;

public class MainViewModel : ViewModelBase
{
    private readonly Func<IMovieEditViewModel> _movieEditVmCreator;
    private IMovieEditViewModel _selectedMovieEditViewModel;

    public MainViewModel(INavigationViewModel navigationViewModel,
        Func<IMovieEditViewModel> movieEditVmCreator,
        IEventAggregator eventAggregator)
    {
        NavigationViewModel = navigationViewModel;
        MovieEditViewModels = new ObservableCollection<IMovieEditViewModel>();
        _movieEditVmCreator = movieEditVmCreator;
        eventAggregator.GetEvent<OpenMovieEditViewEvent>().Subscribe(OnOpenMovieEditView);
        eventAggregator.GetEvent<MovieDeletedEvent>().Subscribe(OnMovieDeleted);
        CloseMovieTabCommand = new DelegateCommand(OnCloseMovieTabExecute);
        AddMovieCommand = new DelegateCommand(OnAddMovieExecute);
    }

    public ICommand AddMovieCommand { get; }

    public ICommand CloseMovieTabCommand { get; }

    public INavigationViewModel NavigationViewModel { get; }

    public ObservableCollection<IMovieEditViewModel> MovieEditViewModels { get; }

    public IMovieEditViewModel SelectedMovieEditViewModel
    {
        get => _selectedMovieEditViewModel;

        set
        {
            _selectedMovieEditViewModel = value;
            OnPropertyChanged();
        }
    }

    private void OnMovieDeleted(int movieId)
    {
        var movieEditVm = MovieEditViewModels.Single(vm => vm.Movie.Id == movieId);
        MovieEditViewModels.Remove(movieEditVm);
    }

    private void OnCloseMovieTabExecute(object obj)
    {
        var movieEditVm = (IMovieEditViewModel)obj;
        MovieEditViewModels.Remove(movieEditVm);
    }

    private void OnAddMovieExecute(object obj)
    {
        SelectedMovieEditViewModel = CreateAndLoadMovieEditViewModel(null);
    }

    private IMovieEditViewModel CreateAndLoadMovieEditViewModel(int? movieId)
    {
        var movieEditVm = _movieEditVmCreator();
        MovieEditViewModels.Add(movieEditVm);
        movieEditVm.Load(movieId);
        return movieEditVm;
    }

    private void OnOpenMovieEditView(int movieId)
    {
        var movieEditVm = MovieEditViewModels.SingleOrDefault(vm => vm.Movie.Id == movieId);
        if (movieEditVm == null) movieEditVm = CreateAndLoadMovieEditViewModel(movieId);
        SelectedMovieEditViewModel = movieEditVm;
    }

    public void Load()
    {
        NavigationViewModel.Load();
    }
}