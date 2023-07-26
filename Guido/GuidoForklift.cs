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

    //upload whole stuff from db
    public void Load()
    {}
    #endregion
    
}
