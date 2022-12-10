using MovieStorage.Model;
using Prism.Events;

namespace MovieStorage.UI.Events;

public class MovieSavedEvent : PubSubEvent<Movie>
{
}