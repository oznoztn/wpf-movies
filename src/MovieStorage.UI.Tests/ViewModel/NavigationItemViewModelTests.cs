using Moq;
using MovieStorage.UI.Events;
using MovieStorage.UI.Tests.Extensions;
using MovieStorage.UI.ViewModel;
using Prism.Events;

namespace MovieStorage.UI.Tests.ViewModel;

public class NavigationItemViewModelTests
{
    private const int MovieId = 7;
    private readonly Mock<IEventAggregator> _eventAggregatorMock;
    private readonly NavigationItemViewModel _viewModel;

    public NavigationItemViewModelTests()
    {
        _eventAggregatorMock = new Mock<IEventAggregator>();

        _viewModel = new NavigationItemViewModel(MovieId,
            "Thomas", _eventAggregatorMock.Object);
    }

    [Fact]
    public void ShouldPublishOpenMovieEditViewEvent()
    {
        var eventMock = new Mock<OpenMovieEditViewEvent>();
        _eventAggregatorMock
            .Setup(ea => ea.GetEvent<OpenMovieEditViewEvent>())
            .Returns(eventMock.Object);

        _viewModel.OpenMovieEditViewCommand.Execute(null);

        eventMock.Verify(e => e.Publish(MovieId), Times.Once);
    }

    [Fact]
    public void ShouldRaisePropertyChangedEventForDisplayMember()
    {
        var fired = _viewModel.IsPropertyChangedFired(() => { _viewModel.DisplayMember = "Changed"; },
            nameof(_viewModel.DisplayMember));

        Assert.True(fired);
    }
}