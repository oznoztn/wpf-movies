using System.ComponentModel;
using System.Windows.Input;
using MovieStorage.Model;
using MovieStorage.UI.Command;
using MovieStorage.UI.DataProvider;
using MovieStorage.UI.Dialogs;
using MovieStorage.UI.Events;
using MovieStorage.UI.Wrapper;
using Prism.Events;

namespace MovieStorage.UI.ViewModel;

public interface IMovieEditViewModel
{
    MovieWrapper Movie { get; }
    void Load(int? movieId);
}

public class MovieEditViewModel : ViewModelBase, IMovieEditViewModel
{
    private readonly IMovieDataProvider _dataProvider;
    private readonly IEventAggregator _eventAggregator;
    private readonly IMessageDialogService _messageDialogService;
    private MovieWrapper _movie;

    public MovieEditViewModel(IMovieDataProvider dataProvider,
        IEventAggregator eventAggregator,
        IMessageDialogService messageDialogService)
    {
        _dataProvider = dataProvider;
        _eventAggregator = eventAggregator;
        _messageDialogService = messageDialogService;
        SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
        DeleteCommand = new DelegateCommand(OnDeleteExecute, OnDeleteCanExecute);
    }

    public ICommand SaveCommand { get; }

    public ICommand DeleteCommand { get; }

    public MovieWrapper Movie
    {
        get => _movie;
        set
        {
            _movie = value;
            OnPropertyChanged();
        }
    }

    public void Load(int? movieId)
    {
        var movie = movieId.HasValue
            ? _dataProvider.GetMovieById(movieId.Value)
            : new Movie();

        Movie = new MovieWrapper(movie);

        Movie.PropertyChanged += Movie_PropertyChanged;

        InvalidateCommands();
    }

    private void Movie_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        InvalidateCommands();
    }

    private void InvalidateCommands()
    {
        ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
    }

    private void OnSaveExecute(object obj)
    {
        _dataProvider.SaveMovie(Movie.Model);
        Movie.AcceptChanges();
        _eventAggregator.GetEvent<MovieSavedEvent>().Publish(Movie.Model);
    }

    private bool OnSaveCanExecute(object arg)
    {
        return Movie != null && Movie.IsChanged;
    }

    private void OnDeleteExecute(object obj)
    {
        var result = _messageDialogService.ShowYesNoDialog("Delete Movie",
            $"Do you really want to delete the movie '{Movie.FirstName} {Movie.LastName}'");
        if (result == MessageDialogResult.Yes)
        {
            _dataProvider.DeleteMovie(Movie.Id);
            _eventAggregator.GetEvent<MovieDeletedEvent>().Publish(Movie.Id);
        }
    }

    private bool OnDeleteCanExecute(object arg)
    {
        return Movie != null && Movie.Id > 0;
    }
}