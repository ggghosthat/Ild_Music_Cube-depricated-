using ShareInstances;
using ShareInstances.Instances;
using Cube.Storage;

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

    public IList<Artist> Artists {get; private set;}
    public IList<Playlist> Playlists {get; private set;}
    public IList<Track> Tracks {get; private set;}


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
    }
    public async void AddPlaylistObj(Playlist playlist) 
    {
        await guidoForklift.AddEntity(playlist);
    }
    public async void AddTrackObj(Track track) 
    {
        await guidoForklift.AddEntity(track);
    }


    public async void EditArtistObj(Artist artist) 
    {
        await guidoForklift.EditEntity(artist);
    }
    
    public async void EditPlaylistObj(Playlist playlist)
    {
        await guidoForklift.EditEntity(playlist);
    }
    
    public async void EditTrackObj(Track track)
    {
        await guidoForklift.EditEntity(track);
    }


    public async void RemoveArtistObj(Artist artist) 
    {
        await guidoForklift.DeleteEntity(artist);
    }

    public async void RemovePlaylistObj(Playlist playlist)
    {
        await guidoForklift.DeleteEntity(playlist);
    }
    public async void RemoveTrackObj(Track track) 
    {
        await guidoForklift.DeleteEntity(track);
    }


    public void Save() {}
    public void SaveArtists() {}
    public void SavePlaylists() {}
    public void SaveTracks() {}
}
