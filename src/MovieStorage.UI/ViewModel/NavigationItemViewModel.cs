using System.Windows.Input;
using MovieStorage.UI.Command;
using MovieStorage.UI.Events;
using Prism.Events;

namespace MovieStorage.UI.ViewModel;

public class NavigationItemViewModel : ViewModelBase
{
    private readonly IEventAggregator _eventAggregator;
    private string _displayMember;

    public NavigationItemViewModel(int id,
        string displayMember,
        IEventAggregator eventAggregator)
    {
        Id = id;
        DisplayMember = displayMember;
        OpenMovieEditViewCommand = new DelegateCommand(OnMovieEditViewExecute);
        _eventAggregator = eventAggregator;
    }

    public int Id { get; }
    public ICommand OpenMovieEditViewCommand { get; }

    public string DisplayMember
    {
        get => _displayMember;

        set
        {
            _displayMember = value;
            OnPropertyChanged();
        }
    }

    private void OnMovieEditViewExecute(object obj)
    {
        _eventAggregator.GetEvent<OpenMovieEditViewEvent>()
            .Publish(Id);
    }
}