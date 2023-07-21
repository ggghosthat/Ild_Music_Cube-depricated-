using ShareInstances.Instances;
using Cube.Mapper;
using Cube.Mapper.Entities;

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

    private static ConcurrentQueue<ReadOnlyMemory<char>> queries = new();
    private static Engine _engine;
    private static Mapper.Mapper _mapper;

    public GuidoForklift(string dbPath,
                        int capacity)
    {
        _engine = new (dbPath);
        _mapper = new();

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
           var mappedArtist = _mapper.MakeSnapshot<Artist>(artist);
           _engine.Add<ArtistMap>((ArtistMap)mappedArtist.Item1);           
           _engine.AddStores(mappedArtist.Item2);
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
