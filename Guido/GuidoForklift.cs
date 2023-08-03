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
    private int offset = 0;

    private static ConcurrentQueue<ReadOnlyMemory<char>> queries = new();
    private static Engine _engine;
    private static Mapper.Mapper _mapper;

    public GuidoForklift(string dbPath,
                        int capacity = 300)
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
           var mappedPlaylist = _mapper.MakeSnapshot<Playlist>(playlist);
           _engine.Add<PlaylistMap>((PlaylistMap)mappedPlaylist.Item1);           
           _engine.AddStores(mappedPlaylist.Item2);

        }
        else if(entity is Track track)
        {
           var mappedTrack = _mapper.MakeSnapshot<Track>(track);
           _engine.Add<TrackMap>((TrackMap)mappedTrack.Item1);           
           _engine.AddStores(mappedTrack.Item2);

        }
    }

    //update(edit) existed entity
    public void EditEntity<T>(T entity)
    {
        if (entity is Artist artist)
        {
           var mappedArtist = _mapper.MakeSnapshot<Artist>(artist);
           _engine.Edit<ArtistMap>((ArtistMap)mappedArtist.Item1);           
           _engine.EditStores(mappedArtist.Item2);
        }
        else if(entity is Playlist playlist)
        {
           var mappedPlaylist = _mapper.MakeSnapshot<Playlist>(playlist);
           _engine.Edit<PlaylistMap>((PlaylistMap)mappedPlaylist.Item1);           
           _engine.EditStores(mappedPlaylist.Item2);
        }
        else if(entity is Track track)
        {
           var mappedTrack = _mapper.MakeSnapshot<Track>(track);
           _engine.Edit<TrackMap>((TrackMap)mappedTrack.Item1);           
           _engine.EditStores(mappedTrack.Item2);
        }
    }

    //delete specific entity by it own id
    public void DeleteEntity<T>(T entity)
    {
        if (entity is Artist artist)
        {
          _engine.Delete<Artist>(ref artist); 
        }
        else if(entity is Playlist playlist)
        {
          _engine.Delete<Playlist>(ref playlist); 
        }
        else if(entity is Track track)
        {
          _engine.Delete<Track>(ref track); 
        }

    }

    public void Load()
    {
        (IEnumerable<ArtistMap>, IEnumerable<PlaylistMap>, IEnumerable<TrackMap>) load = _engine.BringAll(offset:offset, capacity:capacity);
        offset += capacity;
        
    }

    public IEnumerable<T> LoadEntities<T>()
    {
        IEnumerable<T> result = _engine.Bring<T>(offset:offset, capacity:capacity);
        offset += capacity;
        return result;
    }

    public async Task<(T, Store, Store)> LoadSingleEntity<T>(Guid entityId)
    {
        T resultEntityMap = await _engine.BringSingle<T>(entityId);
        (Store, Store) stores = await LoadEntityRelations(1, entityId);
        return (resultEntityMap, stores.Item1, stores.Item2);
    }

    public async Task<(Store, Store)> LoadEntityRelations(int entityIndex, Guid id)
    {
        if (entityIndex == 1) //load stores for artist entity
        {
            var apStore = await _engine.BringStore(1, id);   
            var atStore = await _engine.BringStore(3, id);
            return (apStore, atStore);
        }
        else if (entityIndex == 2) //loads stores for playlist entity
        {
            var paStore = await _engine.BringStore(2, id); 
            var ptStore = await _engine.BringStore(5, id);
            return (paStore, ptStore);
        }
        else if (entityIndex == 3) // loads stores for track entity
        {
            var taStore = await _engine.BringStore(4, id);
            var tpStore = await _engine.BringStore(6, id);
            return (taStore, tpStore);
        }
        else if(entityIndex == 4)
        {
            //implement tag relationships uploading
        }
        throw new Exception($"Can not upload relationship with your entity index: {entityIndex}");
    }
    #endregion
    
}
