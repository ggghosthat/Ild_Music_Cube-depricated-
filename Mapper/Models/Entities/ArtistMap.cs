using System;

namespace Cube.Mapper.Entities;
public record ArtistMap : IMappable
{
    public byte[] Buid { get; set; }    
    public string Name { get; set; }
    public string Description { get; set; }
    public byte[] Avatar { get; set; }
}