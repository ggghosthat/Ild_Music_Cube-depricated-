using ShareInstances;
using ShareInstances.Instances;
using Cube.Storage;
using Cube.Mapper;
using Cube.Mapper.Entities;

using System;
using System.IO;
using System.Collections.Generic;

namespace Cube;
public class Cube : ICube
{
    public Guid CubeId => Guid.NewGuid();

    public string CubeName => "Genezis Cube";

    private int artistOffset = 0;
    private int playlistOffset = 0;
    private int trackOffset = 0;

    public int CubePage => 300;

    public readonly string dbPath = Path.Combine(Environment.CurrentDirectory, "storage.db");
    private GuidoForklift guidoForklift;

    public IEnumerable<Artist> Artists {get; private set;}
    public IEnumerable<Playlist> Playlists {get; private set;}
    public IEnumerable<Track> Tracks {get; private set;}


    public async void Init()
    {
        guidoForklift = new (in dbPath, CubePage);       
        guidoForklift.ForkliftUp();
        var load = await guidoForklift.StartLoad();
        Artists = load.Item1;
        Playlists = load.Item2;
        Tracks = load.Item3;
    }


    public async void AddArtistObj(Artist artist) 
    {
        await guidoForklift.AddEntity(artist);
        if((Artists.Count() + 1) < (artistOffset * CubePage))
        {
           Artists = await guidoForklift.LoadEntities<Artist>(artistOffset); 
        }
    }

    public async void AddPlaylistObj(Playlist playlist) 
    {
        await guidoForklift.AddEntity(playlist);
        if((Playlists.Count() + 1) < (playlistOffset * CubePage))
        {
           Playlists = await guidoForklift.LoadEntities<Playlist>(playlistOffset); 
        }
    }

    public async void AddTrackObj(Track track) 
    {
        await guidoForklift.AddEntity(track);
        if((Tracks.Count() + 1) < (trackOffset * CubePage))
        {
           Tracks = await guidoForklift.LoadEntities<Track>(trackOffset); 
        }
    }


    public async void EditArtistObj(Artist artist) 
    {
        await guidoForklift.EditEntity(artist);
        if(Artists.Any(artist => artist.Id == artist.Id))
        {
           Artists = await guidoForklift.LoadEntities<Artist>(artistOffset); 
        }
    }    

    public async void EditPlaylistObj(Playlist playlist)
    {
        await guidoForklift.EditEntity(playlist);
        if(Playlists.Any(playlist => playlist.Id == playlist.Id))
        {
           Playlists = await guidoForklift.LoadEntities<Playlist>(playlistOffset); 
        }
    }

    public async void EditTrackObj(Track track)
    {
        await guidoForklift.EditEntity(track);
        if(Tracks.Any(track => track.Id == track.Id))
        {
           Tracks = await guidoForklift.LoadEntities<Track>(trackOffset); 
        }
    }


    public async void RemoveArtistObj(Artist artist) 
    {
        await guidoForklift.DeleteEntity(artist);
        if((Artists.Count() - 1) < (artistOffset * CubePage))
        {
           Artists = await guidoForklift.LoadEntities<Artist>(artistOffset); 
        }
    }

    public async void RemovePlaylistObj(Playlist playlist)
    {
        await guidoForklift.DeleteEntity(playlist);
        if((Playlists.Count() - 1) < (playlistOffset * CubePage))
        {
           Playlists = await guidoForklift.LoadEntities<Playlist>(playlistOffset); 
        }
    }

    public async void RemoveTrackObj(Track track) 
    {
        await guidoForklift.DeleteEntity(track);
        if((Tracks.Count() - 1) < (trackOffset * CubePage))
        {
           Tracks = await guidoForklift.LoadEntities<Track>(trackOffset); 
        }
    }


    public async void LoadItems<T>()
    {
        if(typeof(T) == typeof(Artist))
        {
            artistOffset++;
            Artists = await guidoForklift.LoadEntities<Artist>(artistOffset);
        }
        else if(typeof(T) == typeof(Playlist))
        {
            playlistOffset++;
            Playlists = await guidoForklift.LoadEntities<Playlist>(playlistOffset);
        }
        else if(typeof(T) == typeof(Track))
        {
            trackOffset++;
            Tracks = await guidoForklift.LoadEntities<Track>(trackOffset);
        }
    }

    public async Task UnloadItems<T>()
    {
        if(typeof(T) == typeof(Artist))
        {
            artistOffset--;
            var artistsList = Artists.ToList();
            int start = Artists.Count() - CubePage;
            artistsList.RemoveRange(start, CubePage);
            Artists = artistsList;
        }
        else if(typeof(T) == typeof(Playlist))
        {
            playlistOffset--;
            var playlistList = Playlists.ToList();
            int start = Playlists.Count() - CubePage;
            playlistList.RemoveRange(start, CubePage);
            Playlists = playlistList;
        }
        else if(typeof(T) == typeof(Track))
        {
            playlistOffset--;
            var trackList = Tracks.ToList();
            int start = Tracks.Count() - CubePage;
            trackList.RemoveRange(start, CubePage);
            Tracks = trackList;
        }
    }

    
    public async Task<Artist> ExtendSingleArtist(Artist artist)
    {
        return await guidoForklift.ExtendArtist(artist);
    }

    public async Task<Playlist> ExtendSinglePlaylist(Playlist playlist)
    {
        return await guidoForklift.ExtendPlaylist(playlist);
    }

    public async Task<Track> ExtendSingleTrack(Track track)
    {
        return await guidoForklift.ExtendTrack(track);
    }

}
