using Moq;
using MovieStorage.Model;
using MovieStorage.UI.Events;
using MovieStorage.UI.Tests.Extensions;
using MovieStorage.UI.ViewModel;
using MovieStorage.UI.Wrapper;
using Prism.Events;

namespace MovieStorage.UI.Tests.ViewModel;

public class MainViewModelTests
{
    private readonly Mock<IEventAggregator> _eventAggregatorMock;
    private readonly MovieDeletedEvent _movieDeletedEvent;
    private readonly List<Mock<IMovieEditViewModel>> _movieEditViewModelMocks;
    private readonly Mock<INavigationViewModel> _navigationViewModelMock;
    private readonly OpenMovieEditViewEvent _openMovieEditViewEvent;
    private readonly MainViewModel _viewModel;

    public MainViewModelTests()
    {
        _movieEditViewModelMocks = new List<Mock<IMovieEditViewModel>>();
        _navigationViewModelMock = new Mock<INavigationViewModel>();

        _openMovieEditViewEvent = new OpenMovieEditViewEvent();
        _movieDeletedEvent = new MovieDeletedEvent();
        _eventAggregatorMock = new Mock<IEventAggregator>();
        _eventAggregatorMock.Setup(ea => ea.GetEvent<OpenMovieEditViewEvent>())
            .Returns(_openMovieEditViewEvent);
        _eventAggregatorMock.Setup(ea => ea.GetEvent<MovieDeletedEvent>())
            .Returns(_movieDeletedEvent);

        _viewModel = new MainViewModel(_navigationViewModelMock.Object,
            CreateMovieEditViewModel, _eventAggregatorMock.Object);
    }

    private IMovieEditViewModel CreateMovieEditViewModel()
    {
        var movieEditViewModelMock = new Mock<IMovieEditViewModel>();
        movieEditViewModelMock.Setup(vm => vm.Load(It.IsAny<int>()))
            .Callback<int?>(movieId =>
            {
                movieEditViewModelMock.Setup(vm => vm.Movie)
                    .Returns(new MovieWrapper(new Movie { Id = movieId.Value }));
            });
        _movieEditViewModelMocks.Add(movieEditViewModelMock);
        return movieEditViewModelMock.Object;
    }

    [Fact]
    public void ShouldCallTheLoadMethodOfTheNavigationViewModel()
    {
        _viewModel.Load();

        _navigationViewModelMock.Verify(vm => vm.Load(), Times.Once);
    }

    [Fact]
    public void ShouldAddMovieEditViewModelAndLoadAndSelectIt()
    {
        const int movieId = 7;
        _openMovieEditViewEvent.Publish(movieId);

        Assert.Equal(1, _viewModel.MovieEditViewModels.Count);
        var movieEditVm = _viewModel.MovieEditViewModels.First();
        Assert.Equal(movieEditVm, _viewModel.SelectedMovieEditViewModel);
        _movieEditViewModelMocks.First().Verify(vm => vm.Load(movieId), Times.Once);
    }

    [Fact]
    public void ShouldAddMovieEditViewModelAndLoadItWithIdNullAndSelectIt()
    {
        _viewModel.AddMovieCommand.Execute(null);

        Assert.Equal(1, _viewModel.MovieEditViewModels.Count);
        var movieEditVm = _viewModel.MovieEditViewModels.First();
        Assert.Equal(movieEditVm, _viewModel.SelectedMovieEditViewModel);
        _movieEditViewModelMocks.First().Verify(vm => vm.Load(null), Times.Once);
    }

    [Fact]
    public void ShouldAddMovieEditViewModelsOnlyOnce()
    {
        _openMovieEditViewEvent.Publish(5);
        _openMovieEditViewEvent.Publish(5);
        _openMovieEditViewEvent.Publish(6);
        _openMovieEditViewEvent.Publish(7);
        _openMovieEditViewEvent.Publish(7);

        Assert.Equal(3, _viewModel.MovieEditViewModels.Count);
    }

    [Fact]
    public void ShouldRaisePropertyChangedEventForSelectedMovieEditViewModel()
    {
        var movieEditVmMock = new Mock<IMovieEditViewModel>();
        var fired = _viewModel.IsPropertyChangedFired(
            () => { _viewModel.SelectedMovieEditViewModel = movieEditVmMock.Object; },
            nameof(_viewModel.SelectedMovieEditViewModel));

        Assert.True(fired);
    }

    [Fact]
    public void ShouldRemoveMovieEditViewModelOnCloseMovieTabCommand()
    {
        _openMovieEditViewEvent.Publish(7);

        var movieEditVm = _viewModel.SelectedMovieEditViewModel;

        _viewModel.CloseMovieTabCommand.Execute(movieEditVm);

        Assert.Equal(0, _viewModel.MovieEditViewModels.Count);
    }

    [Fact]
    public void ShouldRemoveMovieEditViewModelOnMovieDeletedEvent()
    {
        const int deletedMovieId = 7;

        _openMovieEditViewEvent.Publish(deletedMovieId);
        _openMovieEditViewEvent.Publish(8);
        _openMovieEditViewEvent.Publish(9);

        _movieDeletedEvent.Publish(deletedMovieId);

        Assert.Equal(2, _viewModel.MovieEditViewModels.Count);
        Assert.True(_viewModel.MovieEditViewModels.All(vm => vm.Movie.Id != deletedMovieId));
    }
}