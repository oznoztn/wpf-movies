using System;
using System.Runtime.CompilerServices;
using MovieStorage.Model;
using MovieStorage.UI.ViewModel;

namespace MovieStorage.UI.Wrapper;

public class MovieWrapper : ViewModelBase
{
    private bool _isChanged;

    public MovieWrapper(Movie movie)
    {
        Model = movie;
    }

    public Movie Model { get; }

    public bool IsChanged
    {
        get => _isChanged;
        private set
        {
            _isChanged = value;
            OnPropertyChanged();
        }
    }

    public int Id => Model.Id;

    public string FirstName
    {
        get => Model.Title;
        set
        {
            Model.Title = value;
            OnPropertyChanged();
        }
    }

    public string LastName
    {
        get => Model.Director;
        set
        {
            Model.Director = value;
            OnPropertyChanged();
        }
    }

    public DateTime? Birthday
    {
        get => Model.ReleaseDate;
        set
        {
            Model.ReleaseDate = value;
            OnPropertyChanged();
        }
    }

    public bool IsDeveloper
    {
        get => Model.IsSeen;
        set
        {
            Model.IsSeen = value;
            OnPropertyChanged();
        }
    }

    public void AcceptChanges()
    {
        IsChanged = false;
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName != nameof(IsChanged)) IsChanged = true;
    }
}