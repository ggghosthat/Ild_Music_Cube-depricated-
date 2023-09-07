using ShareInstances;
using ShareInstances.Instances;
using ShareInstances.Statistics;
using Cube.Storage;
using Cube.Storage.Guido.Engine;
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

    private int pageCount = 300;
    public int CubePage => pageCount;

    private string dbPath = Path.Combine(Environment.CurrentDirectory, "storage.db");
    private GuidoForklift guidoForklift;

    public IEnumerable<Artist> Artists {get; private set;}
    public IEnumerable<Playlist> Playlists {get; private set;}
    public IEnumerable<Track> Tracks {get; private set;}


    public void Init()
    {
        guidoForklift = new (in dbPath, CubePage);       
        guidoForklift.ForkliftUp();
        LoadUp().Wait();
    }

    public async Task LoadUp()
    {
        var load = await guidoForklift.StartLoad();
        Artists = load.Item1;
        Playlists = load.Item2;
        Tracks = load.Item3;
    }

    public void SetPath(ref string inputPath)
    {
        dbPath = inputPath;
    }

    public async Task AddArtistObj(Artist artist) 
    {
        await guidoForklift.AddEntity(artist);
        if((Artists.Count() + 1) < (artistOffset * CubePage))
        {
           Artists = await guidoForklift.LoadEntities<Artist>(artistOffset); 
        }
    }

    public async Task AddPlaylistObj(Playlist playlist) 
    {
        await guidoForklift.AddEntity(playlist);
        Console.WriteLine(Playlists is null);
        if((Playlists.Count() + 1) < (playlistOffset * CubePage))
        {
           Playlists = await guidoForklift.LoadEntities<Playlist>(playlistOffset); 
        }
    }

    public async Task AddTrackObj(Track track) 
    {
        await guidoForklift.AddEntity(track);
        if((Tracks.Count() + 1) < (trackOffset * CubePage))
        {
           Tracks = await guidoForklift.LoadEntities<Track>(trackOffset); 
        }
    }


    public async Task EditArtistObj(Artist artist) 
    {
        await guidoForklift.EditEntity(artist);
        if(Artists.Any(artist => artist.Id == artist.Id))
        {
           Artists = await guidoForklift.LoadEntities<Artist>(artistOffset); 
        }
    }    

    public async Task EditPlaylistObj(Playlist playlist)
    {
        await guidoForklift.EditEntity(playlist);
        if(Playlists.Any(playlist => playlist.Id == playlist.Id))
        {
           Playlists = await guidoForklift.LoadEntities<Playlist>(playlistOffset); 
        }
    }

    public async Task EditTrackObj(Track track)
    {
        await guidoForklift.EditEntity(track);
        if(Tracks.Any(track => track.Id == track.Id))
        {
           Tracks = await guidoForklift.LoadEntities<Track>(trackOffset); 
        }
    }


    public async Task RemoveArtistObj(Artist artist) 
    {
        await guidoForklift.DeleteEntity(artist);
        if((Artists?.Count() - 1) < (artistOffset * CubePage))
        {
           Artists = await guidoForklift.LoadEntities<Artist>(artistOffset); 
        }
    }

    public async Task RemovePlaylistObj(Playlist playlist)
    {
        await guidoForklift.DeleteEntity(playlist);
        if((Playlists?.Count() - 1) < (playlistOffset * CubePage))
        {
           Playlists = await guidoForklift.LoadEntities<Playlist>(playlistOffset); 
        }
    }

    public async Task RemoveTrackObj(Track track) 
    {
        await guidoForklift.DeleteEntity(track);
        if((Tracks?.Count() - 1) < (trackOffset * CubePage))
        {
           Tracks = await guidoForklift.LoadEntities<Track>(trackOffset); 
        }
    }


    public async Task LoadItems<T>()
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

    public async Task<IEnumerable<T>> Search<T>(ReadOnlyMemory<char> searchTerm)
    {
        return await guidoForklift.Search<T>(searchTerm);
    }

    
    public async Task<Artist> ExtendSingle(Artist artist)
    {
        return await guidoForklift.ExtendArtist(artist);
    }

    public async Task<Playlist> ExtendSingle(Playlist playlist)
    {
        return await guidoForklift.ExtendPlaylist(playlist);
    }

    public async Task<Track> ExtendSingle(Track track)
    {
        return await guidoForklift.ExtendTrack(track);
    }


    public async Task<InspectFrame> CheckArtistRelates(Artist artist)
    {              
        var factPlaylistRelates = await guidoForklift.FilterRelates(1, artist.Playlists);
        var factTrackRelates = await guidoForklift.FilterRelates(2, artist.Tracks);
        
        var diffPlaylists = artist.Playlists.Except(factTrackRelates);
        var diffTracks = artist.Tracks.Except(factTrackRelates);
        
        return new InspectFrame(tag:0, 
                                factPlaylistRelates.Count(),
                                factTrackRelates.Count(),
                                artist.Playlists.Count(),
                                artist.Tracks.Count());
    }

    public async Task<InspectFrame> CheckPlaylistRelates(Playlist playlist)
    {
        var factArtistRelates = await guidoForklift.FilterRelates(0, playlist.Artists);
        var factTrackRelates = await guidoForklift.FilterRelates(2, playlist.Tracky);
        
        return new InspectFrame(tag:1, 
                                factArtistRelates.Count(),
                                factTrackRelates.Count(),
                                playlist.Artists.Count(),
                                playlist.Tracky.Count());
    }

    public async Task<InspectFrame> CheckTrackRelates(Track track)
    {
        var factArtistRelates = await guidoForklift.FilterTrackRelates(track.Id, true);
        var factPlaylistRelates = await guidoForklift.FilterTrackRelates(track.Id, false);
   
        return new InspectFrame(tag:2, 
                                factArtistRelates.Count(),
                                factPlaylistRelates.Count());
    }

    public async Task<CounterFrame> SnapCounterFrame()
    {
        return default;
    }
}
