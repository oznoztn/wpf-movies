using System.Collections.ObjectModel;
using System.Linq;
using MovieStorage.Model;
using MovieStorage.UI.DataProvider;
using MovieStorage.UI.Events;
using Prism.Events;

namespace MovieStorage.UI.ViewModel;

public interface INavigationViewModel
{
    void Load();
}

public class NavigationViewModel : ViewModelBase,
    INavigationViewModel
{
    private readonly INavigationDataProvider _dataProvider;
    private readonly IEventAggregator _eventAggregator;

    public NavigationViewModel(INavigationDataProvider dataProvider,
        IEventAggregator eventAggregator)
    {
        Movies = new ObservableCollection<NavigationItemViewModel>();
        _dataProvider = dataProvider;
        _eventAggregator = eventAggregator;
        _eventAggregator.GetEvent<MovieSavedEvent>().Subscribe(OnMovieSaved);
        _eventAggregator.GetEvent<MovieDeletedEvent>().Subscribe(OnMovieDeleted);
    }

    public ObservableCollection<NavigationItemViewModel> Movies { get; }

    public void Load()
    {
        Movies.Clear();
        foreach (var movie in _dataProvider.GetAllMovies())
            Movies.Add(new NavigationItemViewModel(
                movie.Id, movie.DisplayMember, _eventAggregator));
    }

    private void OnMovieDeleted(int movieId)
    {
        var navigationItem = Movies.Single(n => n.Id == movieId);
        Movies.Remove(navigationItem);
    }

    private void OnMovieSaved(Movie movie)
    {
        var displayMember = $"{movie.Title} {movie.Director}";
        var navigationItem = Movies.SingleOrDefault(n => n.Id == movie.Id);
        if (navigationItem != null)
        {
            navigationItem.DisplayMember = displayMember;
        }
        else
        {
            navigationItem = new NavigationItemViewModel(movie.Id,
                displayMember,
                _eventAggregator);
            Movies.Add(navigationItem);
        }
    }
}