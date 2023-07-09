using ShareInstances.Instances;
using Cube.Mapper.Entities;

using System;
using System.Linq;
using System.Collections.Concurrent;
using AutoMapper;

namespace Cube.Mapper;

//this class perform mapping operations and has been created to be used within using scopes
public sealed class Mapper : IDisposable
{
    private IMapper _mapper;

    private IMappable entity;
    private IList<IMappable> stores;
    
    private ConcurrentDictionary<IMappable, IList<IMappable>> multimapStore;
    public Mapper()
    {  
        MapperConfiguration configure = new MapperConfiguration(cfg => 
        {
            cfg.AddProfile(new MapProfile());
        });

        _mapper = new AutoMapper.Mapper(configure);
    }

    #region Private Methods
    private async Task GetMappedStaff<T>(ICollection<T> raws)
    {
        var opt = new ParallelOptions {MaxDegreeOfParallelism=4};
        await Parallel.ForEachAsync(raws, opt, async (raw, token) => {
            if (raw is Artist artist)
            {
                var mappedEntity = _mapper.Map<ArtistMap>(artist);
                
                var artistPlaylistStore = _mapper.Map<Store>(artist.Playlists);
                artistPlaylistStore.SetHolder(artist.Id);
                artistPlaylistStore.Tag = 1;

                var artistTrackStore = _mapper.Map<Store>(artist.Tracks);
                artistTrackStore.SetHolder(artist.Id);
                artistTrackStore.Tag = 2;      

                multimapStore.TryAdd(mappedEntity,
                                     new List<IMappable> {artistPlaylistStore, artistTrackStore});                      
            }
            else if(raw is Playlist playlist)
            {                        
                var mappedEntity = _mapper.Map<PlaylistMap>(playlist);

                var playlistArtistStore = _mapper.Map<Store>(playlist.Artists);
                playlistArtistStore.SetHolder(playlist.Id);
                playlistArtistStore.Tag = 3;

                var playlistTrackStore = _mapper.Map<Store>(playlist.Tracky);
                playlistTrackStore.SetHolder(playlist.Id);
                playlistTrackStore.Tag = 4;

                multimapStore.TryAdd(mappedEntity,
                                     new List<IMappable> {playlistArtistStore, playlistTrackStore});                      
            }
            else if(raw is Track track)
            {
                var mappedEntity = _mapper.Map<TrackMap>(track);

                var trackArtistStore = _mapper.Map<Store>(track.Artists);
                trackArtistStore.SetHolder(track.Id);
                trackArtistStore.Tag = 5;

                var trackPlaylistStore = _mapper.Map<Store>(track.Playlists);
                trackPlaylistStore.SetHolder(track.Id);
                trackPlaylistStore.Tag = 6;

                multimapStore.TryAdd(mappedEntity,
                                     new List<IMappable> {trackArtistStore, trackPlaylistStore});                      
            }
        });
    }
    #endregion

    #region Public Methods
    public (IMappable, IList<IMappable>) MakeSnapshot<T>(T entity)
    {
        this.stores = new List<IMappable>();
        if (entity is Artist artist)
        {
            this.entity = _mapper.Map<ArtistMap>(artist);
            
            var artistPlaylistStore = _mapper.Map<Store>(artist.Playlists);
            artistPlaylistStore.SetHolder(artist.Id);
            artistPlaylistStore.Tag = 1;

            this.stores.Add(artistPlaylistStore);

            var artistTrackStore = _mapper.Map<Store>(artist.Tracks);
            artistTrackStore.SetHolder(artist.Id);
            artistTrackStore.Tag = 2;
                        
            this.stores.Add(artistTrackStore);
            return (this.entity, this.stores);
        }
        else if(entity is Playlist playlist)
        {                        
            this.entity = _mapper.Map<PlaylistMap>(playlist);

            var playlistArtistStore = _mapper.Map<Store>(playlist.Artists);
            playlistArtistStore.SetHolder(playlist.Id);
            playlistArtistStore.Tag = 3;
            
            this.stores.Add(playlistArtistStore);

            var playlistTrackStore = _mapper.Map<Store>(playlist.Tracky);
            playlistTrackStore.SetHolder(playlist.Id);
            playlistTrackStore.Tag = 4;
            
            this.stores.Add(playlistTrackStore);

            return (this.entity, this.stores);
        }
        else if(entity is Track track)
        {
            this.entity = _mapper.Map<TrackMap>(track);

            var trackArtistStore = _mapper.Map<Store>(track.Artists);
            trackArtistStore.SetHolder(track.Id);
            trackArtistStore.Tag = 5;
                        
            this.stores.Add(trackArtistStore);

            var trackPlaylistStore = _mapper.Map<Store>(track.Playlists);
            trackPlaylistStore.SetHolder(track.Id);
            trackPlaylistStore.Tag = 6;
                        
            this.stores.Add(trackPlaylistStore);

            return (this.entity, this.stores);
        }
        else return default;
    }

    public async Task<IDictionary<IMappable, IList<IMappable>>> MakeSnapshot<T>(ICollection<T> entities)
    {
        multimapStore = new ConcurrentDictionary<IMappable, IList<IMappable>>();
        await GetMappedStaff<T>(entities);
        return multimapStore.ToDictionary(pair => pair.Key, pair => pair.Value, multimapStore.Comparer);
    }

    public void Clean()
    {
        entity = null;
        stores = null;

        multimapStore.Clear();
        multimapStore = null;
        GC.Collect();
    }

    public void Dispose() 
    {
        Clean();
    }
    #endregion
}