namespace Cube.Mapper.Entities;
public record TagMap : IMappable
{
    public byte[] Buid { get; set; }    
    public string Name { get; set; }
}