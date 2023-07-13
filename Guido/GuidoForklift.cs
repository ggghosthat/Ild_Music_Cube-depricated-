using ShareInstances.Instances;

using System;
using System.Threading.Tasks;
using System.Data.SQLite;
using Dapper;
using System.Collections.Concurrent;
using System.IO;

namespace Cube.Storage;

public class GuidoForklift //Cars from pixar (lol)
{
    private int capacity;

    private ConcurrentQueue<ReadOnlyMemory<char>> queries = new();
    private Engine _engine;
    public GuidoForklift(string dbPath,
                        int capacity)
    {
        _engine = new (dbPath);
        this.capacity = capacity;
    }

    //check database and table existance
    //in negative case it creates from scratch
    public void ForkliftUp()
    {
        _engine.StartEngine();
    }

    #region CRUD
    //insert new entity
    public async Task AddEntity<T>(T entity)
    {
        if (entity is Artist artist)
        {
        }
        else if(entity is Playlist playlist)
        {            
        }
        else if(entity is Track track)
        {
        }
    }

    //update(edit) existed entity
    public void EditEntity<T>(T entity)
    {}

    //delete specific entity by it own id
    public void DeleteEntity<T>(Guid entityId)
    {}

    //upload whole stuff from db
    public void Load()
    {}
    #endregion
    
}
