using MovieStorage.Model;

namespace MovieStorage.DataAccess;

public interface IDataService : IDisposable
{
    Movie GetMovieById(int movieId);

    void SaveMovie(Movie movie);

    void DeleteMovie(int movieId);

    IEnumerable<LookupItem> GetAllMovies();
}