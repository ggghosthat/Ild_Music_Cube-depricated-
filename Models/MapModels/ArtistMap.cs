using System;

namespace Cube.Models.MapModels;
public record ArtistMap(byte[] Buid,
                        string Name,
                        string Description,
                        byte[] Avatar);