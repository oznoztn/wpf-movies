namespace MovieStorage.Model;

public class Movie
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Director { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public bool IsSeen { get; set; }
}