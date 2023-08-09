using ShareInstances.Instances;
using Cube.Mapper;
using Cube.Mapper.Entities;

using System;
using System.Threading.Tasks;
using System.Data.SQLite;
using Dapper;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;

namespace Cube.Storage;

public class GuidoForklift //Cars from pixar (lol)
{
    private int capacity;
    private int offset = 0;
    private int artistOffset = 0;
    private int playlistOffset = 0;
    private int trackOfsset = 0;
    private int tagOffset = 0;

    private static ConcurrentQueue<ReadOnlyMemory<char>> queries = new();
    private static Engine _engine;
    private static Mapper.Mapper _mapper;

    public GuidoForklift(in string dbPath,
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
           playlist.DumpTracks(); 
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
    public async Task EditEntity<T>(T entity)
    {
        if (entity is Artist artist)
        {
           var mappedArtist = _mapper.MakeSnapshot<Artist>(artist);
           _engine.Edit<ArtistMap>((ArtistMap)mappedArtist.Item1);           
           _engine.EditStores(mappedArtist.Item2);
        }
        else if(entity is Playlist playlist)
        {
           playlist.DumpTracks();
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
    public async Task DeleteEntity<T>(T entity)
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

    public async Task<(IEnumerable<Artist>, IEnumerable<Playlist>, IEnumerable<Track>)> StartLoad()
    {
        (IEnumerable<ArtistMap>, IEnumerable<PlaylistMap>, IEnumerable<TrackMap>) load = await _engine.BringAll(offset:offset, capacity:capacity);
        await _mapper.GetEntityProjections<ArtistMap>(load.Item1);
        await _mapper.GetEntityProjections<PlaylistMap>(load.Item2);
        await _mapper.GetEntityProjections<TrackMap>(load.Item3);
        offset += capacity;
        artistOffset += capacity;
        playlistOffset += capacity;
        trackOfsset += capacity;
        return (_mapper.Artists, _mapper.Playlists, _mapper.Tracks);
    }

    public async Task<IEnumerable> LoadEntities<T>()
    {
        if(typeof(T) == typeof(ArtistMap))
        {
             var maps = await _engine.Bring<ArtistMap>(artistOffset, capacity);
            return await _mapper.GetEntityProjections<ArtistMap>(maps);
        }
        else if(typeof(T) == typeof(PlaylistMap))
        {
            var maps = await _engine.Bring<PlaylistMap>(playlistOffset, capacity);
            return await _mapper.GetEntityProjections<PlaylistMap>(maps);
        }
        else if(typeof(T) == typeof(TrackMap))
        {
            var maps = await _engine.Bring<TrackMap>(trackOfsset, capacity);
            return await _mapper.GetEntityProjections<TrackMap>(maps);
        }
        else if(typeof(T) == typeof(TagMap))
        {
            var maps = await _engine.Bring<TagMap>(tagOffset, capacity);
            return await _mapper.GetEntityProjections<TagMap>(maps);
        }
        else throw new Exception("Could not load entities of your type.");
    }

    //could not return in share instances
    //return as mappable instances
    public async Task<(T, Store, Store)> LoadSingleEntity<T>(Guid entityId)
    {
        T entityMap = await _engine.BringSingle<T>(entityId);
        (Store, Store) stores = await LoadEntityRelations<T>(entityId);
        return (entityMap, stores.Item1, stores.Item2); 
    }

    private async Task<(Store, Store)> LoadEntityRelations<T>(Guid id)
    {
        if (typeof(T) == typeof(ArtistMap)) //load stores for artist entity
        {
            var apStore = await _engine.BringStore(1, id);   
            var atStore = await _engine.BringStore(3, id);
            return (apStore, atStore);
        }
        else if (typeof(T) == typeof(PlaylistMap)) //loads stores for playlist entity
        {
            var paStore = await _engine.BringStore(2, id); 
            var ptStore = await _engine.BringStore(5, id);
            return (paStore, ptStore);
        }
        else if (typeof(T) == typeof(TrackMap)) // loads stores for track entity
        {
            var taStore = await _engine.BringStore(4, id);
            var tpStore = await _engine.BringStore(6, id);
            return (taStore, tpStore);
        }
        else if(typeof(T) == typeof(TagMap))
        {
            var tagStore = await _engine.BringStore(7, id);
            return (tagStore, default);
        }
        throw new Exception($"Can not upload relationship.");
    }

    public async Task<IEnumerable<T>> LoadEntitiesById<T>(ICollection<Guid> idCollection)
    {
       var result = await _engine.BringItemsById<T>(idCollection); 
       return result;
    }
    #endregion
    
}
