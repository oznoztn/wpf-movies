using System.Text.Json;
using MovieStorage.Model;

namespace MovieStorage.DataAccess;

public class FileDataService : IDataService
{
    private const string StorageFile = "Movies.json";

    public Movie GetMovieById(int movieId)
    {
        var movies = ReadFromFile();
        return movies.Single(f => f.Id == movieId);
    }

    public void SaveMovie(Movie movie)
    {
        if (movie.Id <= 0)
            InsertMovie(movie);
        else
            UpdateMovie(movie);
    }

    public void DeleteMovie(int movieId)
    {
        var movies = ReadFromFile();
        var existing = movies.Single(f => f.Id == movieId);
        movies.Remove(existing);
        SaveToFile(movies);
    }

    public IEnumerable<LookupItem> GetAllMovies()
    {
        return ReadFromFile()
            .Select(f => new LookupItem
            {
                Id = f.Id,
                DisplayMember = $"{f.Title} {f.Director}"
            });
    }

    public void Dispose()
    {
        // Usually Service-Proxies are disposable.
        // This method is added as demo-purpose to show how to use an IDisposable in the client with a Func<T>.
        //      Look for example at the MovieDataProvider-class
    }

    private void UpdateMovie(Movie movie)
    {
        var movies = ReadFromFile();
        var existing = movies.Single(f => f.Id == movie.Id);
        var indexOfExisting = movies.IndexOf(existing);
        movies.Insert(indexOfExisting, movie);
        movies.Remove(existing);
        SaveToFile(movies);
    }

    private void InsertMovie(Movie movie)
    {
        var movies = ReadFromFile();
        var maxMovieId = movies.Count == 0 ? 0 : movies.Max(f => f.Id);
        movie.Id = maxMovieId + 1;
        movies.Add(movie);
        SaveToFile(movies);
    }

    private void SaveToFile(List<Movie> movieList)
    {
        var json = JsonSerializer.Serialize(movieList);
        File.WriteAllText(StorageFile, json);
    }

    private List<Movie> ReadFromFile()
    {
        if (!File.Exists(StorageFile))
        {
            return new List<Movie>
            {
                new()
                {
                    Id = 1,
                    Title = "The Matrix",
                    Director = "The Wachowskis",
                    ReleaseDate = new DateTime(2000, 01, 01),
                    IsSeen = true
                },
                new()
                {
                    Id = 2,
                    Title = "The Matrix Reloaded",
                    Director = "The Wachowskis",
                    ReleaseDate = new DateTime(2000, 01, 01),
                    IsSeen = true
                },
                new()
                {
                    Id = 3,
                    Title = "The Matrix Revolutions",
                    Director = "The Wachowskis",
                    ReleaseDate = new DateTime(2000, 01, 01),
                    IsSeen = true
                }
            };
        }

        var json = File.ReadAllText(StorageFile);
        return JsonSerializer.Deserialize<List<Movie>>(json)!;
    }
}