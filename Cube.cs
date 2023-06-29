using ShareInstances;
using ShareInstances.Instances;

using System;
using System.Collections.Generic;

namespace Cube;
public class Cube : ICube
{
    public Guid CubeId => Guid.NewGuid();

    public string CubeName => "Genezis Cube";

    public int CubePage => 100;

    public IList<Artist> Artists {get; private set;}
    public IList<Playlist> Playlists {get; private set;}
    public IList<Track> Tracks {get; private set;}

    public void Init(){}



    public void AddArtistObj(Artist artist) {}
    public void AddPlaylistObj(Playlist playlist) {}
    public void AddTrackObj(Track track) {}


    public void EditArtistObj(Artist artist) {}
    public void EditPlaylistObj(Playlist playlist) {}
    public void EditTrackObj(Track track) {}


    public void RemoveArtistObj(Artist artist) {}
    public void RemovePlaylistObj(Playlist playlist) {}
    public void RemoveTrackObj(Track track) {}


    public void Save() {}
    public void SaveArtists() {}
    public void SavePlaylists() {}
    public void SaveTracks() {}
}