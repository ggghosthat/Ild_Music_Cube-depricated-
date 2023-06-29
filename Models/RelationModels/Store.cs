using System;
using System.Collections.Generic;

namespace Cube.Models.MapModels;
public struct Store
{
    public ReadOnlyMemory<char> Tag {get; set;}
    public ICollection<Pair> Pairs {get; set;} = new List<Pair>();

    public Store(ref string tag)
    {
        Tag = tag.AsMemory();
    }
}