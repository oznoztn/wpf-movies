using Moq;
using MovieStorage.Model;
using MovieStorage.UI.DataProvider;
using MovieStorage.UI.Dialogs;
using MovieStorage.UI.Events;
using MovieStorage.UI.Tests.Extensions;
using MovieStorage.UI.ViewModel;
using Prism.Events;

namespace MovieStorage.UI.Tests.ViewModel;

public class MovieEditViewModelTests
{
    private const int MovieId = 5;
    private readonly Mock<IMovieDataProvider> _dataProviderMock;
    private readonly Mock<IEventAggregator> _eventAggregatorMock;
    private readonly Mock<IMessageDialogService> _messageDialogServiceMock;
    private readonly Mock<MovieDeletedEvent> _movieDeletedEventMock;
    private readonly Mock<MovieSavedEvent> _movieSavedEventMock;
    private readonly MovieEditViewModel _viewModel;

    public MovieEditViewModelTests()
    {
        _movieDeletedEventMock = new Mock<MovieDeletedEvent>();
        _movieSavedEventMock = new Mock<MovieSavedEvent>();

        _eventAggregatorMock = new Mock<IEventAggregator>();
        _eventAggregatorMock.Setup(ea => ea.GetEvent<MovieSavedEvent>())
            .Returns(_movieSavedEventMock.Object);
        _eventAggregatorMock.Setup(ea => ea.GetEvent<MovieDeletedEvent>())
            .Returns(_movieDeletedEventMock.Object);

        _dataProviderMock = new Mock<IMovieDataProvider>();
        _dataProviderMock.Setup(dp => dp.GetMovieById(MovieId))
            .Returns(new Movie { Id = MovieId, Title = "Thomas" });

        _messageDialogServiceMock = new Mock<IMessageDialogService>();

        _viewModel = new MovieEditViewModel(_dataProviderMock.Object,
            _eventAggregatorMock.Object,
            _messageDialogServiceMock.Object);
    }

    [Fact]
    public void ShouldLoadMovie()
    {
        _viewModel.Load(MovieId);

        Assert.NotNull(_viewModel.Movie);
        Assert.Equal(MovieId, _viewModel.Movie.Id);

        _dataProviderMock.Verify(dp => dp.GetMovieById(MovieId), Times.Once);
    }

    [Fact]
    public void ShouldRaisePropertyChangedEventForMovie()
    {
        var fired = _viewModel.IsPropertyChangedFired(
            () => _viewModel.Load(MovieId),
            nameof(_viewModel.Movie));

        Assert.True(fired);
    }

    [Fact]
    public void ShouldDisableSaveCommandWhenMovieIsLoaded()
    {
        _viewModel.Load(MovieId);

        Assert.False(_viewModel.SaveCommand.CanExecute(null));
    }

    [Fact]
    public void ShouldEnableSaveCommandWhenMovieIsChanged()
    {
        _viewModel.Load(MovieId);

        _viewModel.Movie.FirstName = "Changed";

        Assert.True(_viewModel.SaveCommand.CanExecute(null));
    }

    [Fact]
    public void ShouldDisableSaveCommandWithoutLoad()
    {
        Assert.False(_viewModel.SaveCommand.CanExecute(null));
    }

    [Fact]
    public void ShouldRaiseCanExecuteChangedForSaveCommandWhenMovieIsChanged()
    {
        _viewModel.Load(MovieId);
        var fired = false;
        _viewModel.SaveCommand.CanExecuteChanged += (s, e) => fired = true;
        _viewModel.Movie.FirstName = "Changed";
        Assert.True(fired);
    }

    [Fact]
    public void ShouldRaiseCanExecuteChangedForSaveCommandAfterLoad()
    {
        var fired = false;
        _viewModel.SaveCommand.CanExecuteChanged += (s, e) => fired = true;
        _viewModel.Load(MovieId);
        Assert.True(fired);
    }

    [Fact]
    public void ShouldRaiseCanExecuteChangedForDeleteCommandAfterLoad()
    {
        var fired = false;
        _viewModel.DeleteCommand.CanExecuteChanged += (s, e) => fired = true;
        _viewModel.Load(MovieId);
        Assert.True(fired);
    }

    [Fact]
    public void ShouldRaiseCanExecuteChangedForDeleteCommandWhenAcceptingChanges()
    {
        _viewModel.Load(MovieId);
        var fired = false;
        _viewModel.Movie.FirstName = "Changed";
        _viewModel.DeleteCommand.CanExecuteChanged += (s, e) => fired = true;
        _viewModel.Movie.AcceptChanges();
        Assert.True(fired);
    }

    [Fact]
    public void ShouldCallSaveMethodOfDataProviderWhenSaveCommandIsExecuted()
    {
        _viewModel.Load(MovieId);
        _viewModel.Movie.FirstName = "Changed";

        _viewModel.SaveCommand.Execute(null);
        _dataProviderMock.Verify(dp => dp.SaveMovie(_viewModel.Movie.Model), Times.Once);
    }

    [Fact]
    public void ShouldAcceptChangesWhenSaveCommandIsExecuted()
    {
        _viewModel.Load(MovieId);
        _viewModel.Movie.FirstName = "Changed";

        _viewModel.SaveCommand.Execute(null);
        Assert.False(_viewModel.Movie.IsChanged);
    }

    [Fact]
    public void ShouldPublishMovieSavedEventWhenSaveCommandIsExecuted()
    {
        _viewModel.Load(MovieId);
        _viewModel.Movie.FirstName = "Changed";

        _viewModel.SaveCommand.Execute(null);
        _movieSavedEventMock.Verify(e => e.Publish(_viewModel.Movie.Model), Times.Once);
    }

    [Fact]
    public void ShouldCreateNewMovieWhenNullIsPassedToLoadMethod()
    {
        _viewModel.Load(null);

        Assert.NotNull(_viewModel.Movie);
        Assert.Equal(0, _viewModel.Movie.Id);
        Assert.Null(_viewModel.Movie.FirstName);
        Assert.Null(_viewModel.Movie.LastName);
        Assert.Null(_viewModel.Movie.Birthday);
        Assert.False(_viewModel.Movie.IsDeveloper);

        _dataProviderMock.Verify(dp => dp.GetMovieById(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void ShouldEnableDeleteCommandForExistingMovie()
    {
        _viewModel.Load(MovieId);
        Assert.True(_viewModel.DeleteCommand.CanExecute(null));
    }

    [Fact]
    public void ShouldDisableDeleteCommandForNewMovie()
    {
        _viewModel.Load(null);
        Assert.False(_viewModel.DeleteCommand.CanExecute(null));
    }

    [Fact]
    public void ShouldDisableDeleteCommandWithoutLoad()
    {
        Assert.False(_viewModel.DeleteCommand.CanExecute(null));
    }

    [Theory]
    [InlineData(MessageDialogResult.Yes, 1)]
    [InlineData(MessageDialogResult.No, 0)]
    public void ShouldCallDeleteMovieWhenDeleteCommandIsExecuted(
        MessageDialogResult result, int expectedDeleteMovieCalls)
    {
        _viewModel.Load(MovieId);

        _messageDialogServiceMock.Setup(ds => ds.ShowYesNoDialog(It.IsAny<string>(),
            It.IsAny<string>())).Returns(result);

        _viewModel.DeleteCommand.Execute(null);

        _dataProviderMock.Verify(dp => dp.DeleteMovie(MovieId),
            Times.Exactly(expectedDeleteMovieCalls));
        _messageDialogServiceMock.Verify(ds => ds.ShowYesNoDialog(It.IsAny<string>(),
            It.IsAny<string>()), Times.Once);
    }

    [Theory]
    [InlineData(MessageDialogResult.Yes, 1)]
    [InlineData(MessageDialogResult.No, 0)]
    public void ShouldPublishMovieDeletedEventWhenDeleteCommandIsExecuted(
        MessageDialogResult result, int expectedPublishCalls)
    {
        _viewModel.Load(MovieId);

        _messageDialogServiceMock.Setup(ds => ds.ShowYesNoDialog(It.IsAny<string>(),
            It.IsAny<string>())).Returns(result);

        _viewModel.DeleteCommand.Execute(null);

        _movieDeletedEventMock.Verify(e => e.Publish(MovieId),
            Times.Exactly(expectedPublishCalls));

        _messageDialogServiceMock.Verify(ds => ds.ShowYesNoDialog(It.IsAny<string>(),
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void ShouldDisplayCorrectMessageInDeleteDialog()
    {
        _viewModel.Load(MovieId);

        var f = _viewModel.Movie;
        f.FirstName = "Thomas";
        f.LastName = "Huber";

        _viewModel.DeleteCommand.Execute(null);

        _messageDialogServiceMock.Verify(d => d.ShowYesNoDialog("Delete Movie",
                $"Do you really want to delete the movie '{f.FirstName} {f.LastName}'"),
            Times.Once);
    }
}