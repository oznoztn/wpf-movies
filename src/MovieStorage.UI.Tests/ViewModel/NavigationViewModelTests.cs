using Moq;
using MovieStorage.Model;
using MovieStorage.UI.DataProvider;
using MovieStorage.UI.Events;
using MovieStorage.UI.ViewModel;
using Prism.Events;

namespace MovieStorage.UI.Tests.ViewModel;

public class NavigationViewModelTests
{
    private readonly MovieDeletedEvent _movieDeletedEvent;
    private readonly MovieSavedEvent _movieSavedEvent;
    private readonly NavigationViewModel _viewModel;

    public NavigationViewModelTests()
    {
        _movieSavedEvent = new MovieSavedEvent();
        _movieDeletedEvent = new MovieDeletedEvent();

        var eventAggregatorMock = new Mock<IEventAggregator>();
        eventAggregatorMock.Setup(ea => ea.GetEvent<MovieSavedEvent>())
            .Returns(_movieSavedEvent);
        eventAggregatorMock.Setup(ea => ea.GetEvent<MovieDeletedEvent>())
            .Returns(_movieDeletedEvent);

        var navigationDataProviderMock = new Mock<INavigationDataProvider>();
        navigationDataProviderMock.Setup(dp => dp.GetAllMovies())
            .Returns(new List<LookupItem>
            {
                new() { Id = 1, DisplayMember = "Julia" },
                new() { Id = 2, DisplayMember = "Thomas" }
            });
        _viewModel = new NavigationViewModel(
            navigationDataProviderMock.Object,
            eventAggregatorMock.Object);
    }

    [Fact]
    public void ShouldLoadMovies()
    {
        _viewModel.Load();

        Assert.Equal(2, _viewModel.Movies.Count);

        var movie = _viewModel.Movies.SingleOrDefault(f => f.Id == 1);
        Assert.NotNull(movie);
        Assert.Equal("Julia", movie.DisplayMember);

        movie = _viewModel.Movies.SingleOrDefault(f => f.Id == 2);
        Assert.NotNull(movie);
        Assert.Equal("Thomas", movie.DisplayMember);
    }

    [Fact]
    public void ShouldLoadMoviesOnlyOnce()
    {
        _viewModel.Load();
        _viewModel.Load();

        Assert.Equal(2, _viewModel.Movies.Count);
    }

    [Fact]
    public void ShouldUpdateNavigationItemWhenMovieIsSaved()
    {
        _viewModel.Load();
        var navigationItem = _viewModel.Movies.First();

        var movieId = navigationItem.Id;

        _movieSavedEvent.Publish(
            new Movie
            {
                Id = movieId,
                Title = "The Matrix",
                Director = "The W."
            });

        Assert.Equal("The Matrix The W.", navigationItem.DisplayMember);
    }

    [Fact]
    public void ShouldAddNavigationItemWhenAddedMovieIsSaved()
    {
        _viewModel.Load();

        const int newMovieId = 97;

        _movieSavedEvent.Publish(new Movie
        {
            Id = newMovieId,
            Title = "The Matrix",
            Director = "The W."
        });

        Assert.Equal(3, _viewModel.Movies.Count);

        var addedItem = _viewModel.Movies.SingleOrDefault(f => f.Id == newMovieId);
        Assert.NotNull(addedItem);
        Assert.Equal("The Matrix The W.", addedItem.DisplayMember);
    }

    [Fact]
    public void ShouldRemoveNavigationItemWhenMovieIsDeleted()
    {
        _viewModel.Load();

        var deletedMovieId = _viewModel.Movies.First().Id;

        _movieDeletedEvent.Publish(deletedMovieId);

        Assert.Equal(1, _viewModel.Movies.Count);
        Assert.NotEqual(deletedMovieId, _viewModel.Movies.Single().Id);
    }
}