using System;
using System.Collections.Generic;
using MovieStorage.DataAccess;
using MovieStorage.Model;

namespace MovieStorage.UI.DataProvider;

internal class NavigationDataProvider : INavigationDataProvider
{
    private readonly Func<IDataService> _dataServiceCreator;

    public NavigationDataProvider(Func<IDataService> dataServiceCreator)
    {
        _dataServiceCreator = dataServiceCreator;
    }

    public IEnumerable<LookupItem> GetAllMovies()
    {
        using var dataService = _dataServiceCreator();
        return dataService.GetAllMovies();
    }
}