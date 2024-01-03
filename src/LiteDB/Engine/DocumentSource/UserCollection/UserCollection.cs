﻿namespace LiteDB.Engine;

internal class UserCollection : IDocumentSource
{
    private readonly string _name;
    private CollectionDocument? _collection; // will load on Initialize()

    public byte ColID => _collection?.ColID ?? 0;
    public string Name => _name;

    public UserCollection(string name)
    {
        _name = name;
    }

    public void Initialize(IMasterService masterService)
    {
        var master = masterService.GetMaster(false);

        if (master.Collections.TryGetValue(_name, out var collection))
        {
            _collection = collection;
        }
        else
        {
            throw ERR($"Collection {_name} does not exist");
        }
    }

    public CollectionDocument GetCollection() => _collection!;

    public IReadOnlyList<IndexDocument> GetIndexes() => _collection!.Indexes;

    public (IDataService dataService, IIndexService indexService) GetServices(IServicesFactory factory, ITransaction transaction) =>
        (factory.CreateDataService(transaction), factory.CreateIndexService(transaction));

    public void Dispose()
    {
        // in user collection, there is nothing to dispose
        _collection = null;
    }
}