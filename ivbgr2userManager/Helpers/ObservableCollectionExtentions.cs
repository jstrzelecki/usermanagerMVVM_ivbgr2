using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ivbgr2userManager;

public static class ObservableCollectionExtentions
{
    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }
    } 
}