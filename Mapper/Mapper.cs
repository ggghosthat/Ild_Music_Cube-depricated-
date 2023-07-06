using ShareInstances.Instances;
using Cube.Mapper.Entities;

using System.Collections;
using AutoMapper;

namespace Cube.Mapper;
public sealed class Mapper
{
    private IMapper _mapper;

    private ICollection<ArtistMap> _artistCache = new List<ArtistMap>();
    private ICollection<PlaylistMap> _playlistCache = new List<PlaylistMap>();
    private ICollection<TrackMap> _trackCache = new List<TrackMap>();

    private Dictionary<int, List<Store>> _mapCache = new Dictionary<int, List<Store>>()
    {
        {1, new List<Store>() },
        {2, new List<Store>() },
        {3, new List<Store>() },
        {4, new List<Store>() },
        {5, new List<Store>() },
        {6, new List<Store>() }
    };

    public Mapper()
    {  
        MapperConfiguration configure = new MapperConfiguration(cfg => 
        {
            cfg.AddProfile(new MapProfile());
        });

        _mapper = new AutoMapper.Mapper(configure);
    }

    #region Public Methods
    public void MakeSnapshot<T>(T entity)
    {
        if (entity is Artist artist)
        {
            var artistMap = _mapper.Map<ArtistMap>(artist);
            
            var artistPlaylistStore = _mapper.Map<Store>(artist.Playlists);
            artistPlaylistStore.SetHolder(artist.Id);
            artistPlaylistStore.Tag = 1;

            if(artistPlaylistStore.Count > 0)
            {
                _mapCache[1].Add(artistPlaylistStore);
            }

            var artistTrackStore = _mapper.Map<Store>(artist.Tracks);
            artistTrackStore.SetHolder(artist.Id);
            artistTrackStore.Tag = 2;
            
            if(artistTrackStore.Count > 0)
            {
                _mapCache[2].Add(artistTrackStore);
            }

            _artistCache.Add(artistMap);
        }
        else if(entity is Playlist playlist)
        {            
            var playlistMap = _mapper.Map<PlaylistMap>(playlist);

            var playlistArtistStore = _mapper.Map<Store>(playlist.Artists);
            playlistArtistStore.SetHolder(playlist.Id);
            playlistArtistStore.Tag = 3;
            
            if(playlistArtistStore.Count > 0)
            {
                _mapCache[3].Add(playlistArtistStore);
            }

            var playlistTrackStore = _mapper.Map<Store>(playlist.Tracky);
            playlistTrackStore.SetHolder(playlist.Id);
            playlistTrackStore.Tag = 4;
            
            if(playlistTrackStore.Count > 0)
            {
                _mapCache[4].Add(playlistTrackStore);
            }

            _playlistCache.Add(playlistMap);
        }
        else if(entity is Track track)
        {
            var trackMap = _mapper.Map<TrackMap>(track);

            var trackArtistStore = _mapper.Map<Store>(track.Artists);
            trackArtistStore.SetHolder(track.Id);
            trackArtistStore.Tag = 5;
            
            if(trackArtistStore.Count > 0)
            {
                _mapCache[5].Add(trackArtistStore);
            }

            var trackPlaylistStore = _mapper.Map<Store>(track.Playlists);
            trackPlaylistStore.SetHolder(track.Id);
            trackPlaylistStore.Tag = 6;
            
            if(trackPlaylistStore.Count > 0)
            {
                _mapCache[6].Add(trackPlaylistStore);
            }

            _trackCache.Add(trackMap);
        }
    }
    
    public void Clean() => _mapCache.Clear();
    #endregion
}