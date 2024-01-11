// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

internal class ImGuiListClipperRange 
{
    public int Min { get; set; }
    public int Max { get; set; }
    public bool PosToIndexConvert { get; set; }
    public sbyte PosToIndexOffsetMin { get; set; }
    public sbyte PosToIndexOffsetMax { get; set; }

    public static ImGuiListClipperRange FromIndices(int min, int max) => new()
    {
        Min = min,
        Max = max,
        PosToIndexConvert = false,
        PosToIndexOffsetMin = 0,
        PosToIndexOffsetMax = 0
    };
    
    public static ImGuiListClipperRange FromPositions(float y1, float y2, int offMin, int offMax) => new()
    {
        Min = (int) y1,
        Max = (int) y2,
        PosToIndexConvert = true,
        PosToIndexOffsetMin = (sbyte) offMin,
        PosToIndexOffsetMax = (sbyte) offMax
    };
}