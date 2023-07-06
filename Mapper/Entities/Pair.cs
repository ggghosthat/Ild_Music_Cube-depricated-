using System;

namespace Cube.Mapper.Entities;
public record Pair(Guid first, Guid second)
{
    public Guid First { get; set; } = first;
    public Guid Second { get; set; } = second;
}