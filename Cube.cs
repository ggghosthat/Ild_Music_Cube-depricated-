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

    public int CubePage => 300;
    public readonly string dbPath = Path.Combine(Environment.CurrentDirectory, "storage.db");
    private GuidoForklift guidoForklift;

    public IEnumerable<Artist> Artists {get; private set;}
    public IEnumerable<Playlist> Playlists {get; private set;}
    public IEnumerable<Track> Tracks {get; private set;}


    public void Init()
    {
        guidoForklift = new (in dbPath, CubePage);       
        guidoForklift.ForkliftUp();
        (IEnumerable<Artist>, IEnumerable<Playlist>, IEnumerable<Track>) load = guidoForklift.StartLoad().Result;
        Artists = load.Item1.ToList();
        Playlists = load.Item2.ToList();
        Tracks = load.Item3.ToList();
    }



    public async void AddArtistObj(Artist artist) 
    {
        await guidoForklift.AddEntity(artist);
        if(Artists.Count() + 1 < CubePage)
        {
           Artists = await guidoForklift.LoadEntities<Artist>(); 
        }
    }
    public async void AddPlaylistObj(Playlist playlist) 
    {
        await guidoForklift.AddEntity(playlist);
        if(Playlists.Count() + 1 < CubePage)
        {
           Playlists = await guidoForklift.LoadEntities<Playlist>(); 
        }
    }
    public async void AddTrackObj(Track track) 
    {
        await guidoForklift.AddEntity(track);
        if(Tracks.Count() + 1 < CubePage)
        {
           Tracks = await guidoForklift.LoadEntities<Track>(); 
        }
    }


    public async void EditArtistObj(Artist artist) 
    {
        await guidoForklift.EditEntity(artist);
        if(Artists.Any(artist => artist.Id == artist.Id))
        {
           Artists = await guidoForklift.LoadEntities<Artist>(); 
        }
    }
    
    public async void EditPlaylistObj(Playlist playlist)
    {
        await guidoForklift.EditEntity(playlist);
        if(Playlists.Any(playlist => playlist.Id == playlist.Id))
        {
           Playlists = await guidoForklift.LoadEntities<Playlist>(); 
        }

    }
    
    public async void EditTrackObj(Track track)
    {
        await guidoForklift.EditEntity(track);
        if(Tracks.Any(track => track.Id == track.Id))
        {
           Tracks = await guidoForklift.LoadEntities<Track>(); 
        }

    }


    public async void RemoveArtistObj(Artist artist) 
    {
        await guidoForklift.DeleteEntity(artist);
        if(Artists.Count() - 1 < CubePage)
        {
           Artists = await guidoForklift.LoadEntities<Artist>(); 
        }
    }

    public async void RemovePlaylistObj(Playlist playlist)
    {
        await guidoForklift.DeleteEntity(playlist);
        if(Playlists.Count() - 1 < CubePage)
        {
           Playlists = await guidoForklift.LoadEntities<Playlist>(); 
        }
    }

    public async void RemoveTrackObj(Track track) 
    {
        await guidoForklift.DeleteEntity(track);
        if(Tracks.Count() - 1 < CubePage)
        {
           Tracks = await guidoForklift.LoadEntities<Track>(); 
        }
    }
}
