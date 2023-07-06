using System;
using System.Linq;
using System.Collections.Generic;

namespace Cube.Mapper.Entities;
public struct Store
{
    public int Tag {get; set;}
    public ICollection<Pair> Pairs {get; set;} = new List<Pair>();
    public int Count => Pairs.Count;
    
    public Store(int tag)
    {
        Tag = tag;
    }

    //set single first value for all pairs
    public void SetHolder(Guid holder)
    {
        Pairs.ToList().ForEach(i => i.First = holder);
    }
}