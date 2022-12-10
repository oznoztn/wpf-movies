using MovieStorage.Model;

namespace MovieStorage.UI.DataProvider;

public interface IMovieDataProvider
{
    Movie GetMovieById(int id);

    void SaveMovie(Movie movie);

    void DeleteMovie(int id);
}