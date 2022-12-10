using System.Collections.Generic;
using MovieStorage.Model;

namespace MovieStorage.UI.DataProvider;

public interface INavigationDataProvider
{
    IEnumerable<LookupItem> GetAllMovies();
}