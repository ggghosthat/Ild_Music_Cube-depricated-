using System;

namespace Cube.Mapper.Entities;
public record PlaylistMap
{
    public byte[] Buid {get; set;}
    public string Name {get; set;}
    public string Description {get; set;}
    public string Avatar {get; set;}
}