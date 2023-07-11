using ShareInstances.Instances;
using Cube.Mapper;
using Cube.Mapper.Entities;

using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;

namespace Cube.Mapper;
public sealed class MapProfile : Profile
{    
    public MapProfile()
    {
        CreateMap<Artist, ArtistMap>()
            .ForMember(dest => dest.Buid, opt => opt.MapFrom(src => src.Id.ToByteArray()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToString()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description.ToString()))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.AvatarSource.ToArray()));

        CreateMap<Playlist, PlaylistMap>()
            .ForMember(dest => dest.Buid, opt => opt.MapFrom(src => src.Id.ToByteArray()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToString()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description.ToString()))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.AvatarSource.ToArray()));

        CreateMap<Track, TrackMap>()
            .ForMember(dest => dest.Buid, opt => opt.MapFrom(src => src.Id.ToByteArray()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToString()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description.ToString()))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.AvatarSource.ToArray()))
            .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src => src.IsValid))
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration));

        CreateMap<ICollection<Guid>, Store>()
            .ConvertUsing((src) => GenerateStore(src));

        CreateMap<ICollection<Track>, Store>()
            .ConvertUsing((src) => GenerateStore(src));
    }

    #region Private Methods
    private Store GenerateStore(ICollection<Guid> items)
    {
        var store = new Store(0);
        items.ToList().ForEach(i =>
        {
            store.Relates.Add(i);
        });

        return store;
    }

    private Store GenerateStore(ICollection<Track> tracks)
    {
        var store = new Store(0);
        tracks.ToList().ForEach(t =>
        {
            store.Relates.Add(t.Id);
        });

        return store;
    }
    #endregion 
}
