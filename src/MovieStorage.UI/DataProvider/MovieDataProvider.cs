using System;
using MovieStorage.DataAccess;
using MovieStorage.Model;

namespace MovieStorage.UI.DataProvider;

public class MovieDataProvider : IMovieDataProvider
{
    private readonly Func<IDataService> _dataServiceCreator;

    public MovieDataProvider(Func<IDataService> dataServiceCreator)
    {
        _dataServiceCreator = dataServiceCreator;
    }

    public Movie GetMovieById(int id)
    {
        using var dataService = _dataServiceCreator();
        return dataService.GetMovieById(id);
    }

    public void SaveMovie(Movie movie)
    {
        using var dataService = _dataServiceCreator();
        dataService.SaveMovie(movie);
    }

    public void DeleteMovie(int id)
    {
        using var dataService = _dataServiceCreator();
        dataService.DeleteMovie(id);
    }
}