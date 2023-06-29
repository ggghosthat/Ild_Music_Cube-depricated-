using System;

namespace Cube.Models.MapModels;
public record TrackMap(byte[] Buid,
                       string Pathway,
                       string Name,
                       string Description,
                       byte[] avatar,
                       bool IsValid,
                       TimeSpan Duration);