using System;

namespace Cube.Mapper.Entities;
public record TrackMap
{
    public byte[] Buid {get; set;}
    public string Pathway {get; set;}
    public string Name {get; set;}
    public string Description {get; set;}
    public byte[] Avatar {get; set;}
    public bool IsValid {get; set;}
    public TimeSpan Duration {get; set;}
}