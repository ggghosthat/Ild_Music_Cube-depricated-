using System;

namespace Cube.Mapper.Entities;
public record TrackMap : IMappable
{
    public string Buid {get; set;}
    public string Pathway {get; set;}
    public string Name {get; set;}
    public string Description {get; set;}
    public byte[] Avatar {get; set;}
    public int IsValid {get; set;}
    public int Duration {get; set;}
}
