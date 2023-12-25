namespace Yuika.YImGui;

[Flags]
public enum ImDrawFlags
{
    None,
    
    Closed                   = 1 << 0,
    
    RoundCornersTopLeft      = 1 << 4,
    RoundCornersTopRight     = 1 << 5,
    RoundCornersBottomLeft   = 1 << 6,
    RoundCornersBottomRight  = 1 << 7,
    RoundCornersNone         = 1 << 8,
    
    RoundCornersTop = RoundCornersTopLeft | RoundCornersTopRight,
    RoundCornersBottom = RoundCornersBottomLeft | RoundCornersBottomRight,
    RoundCornersLeft = RoundCornersTopLeft | RoundCornersBottomLeft,
    RoundCornersRight = RoundCornersTopRight | RoundCornersBottomRight,
    RoundCornersAll = RoundCornersTopLeft | RoundCornersTopRight | RoundCornersBottomLeft | RoundCornersBottomRight,
    
    RoundCornersDefault = RoundCornersAll,
    RoundCornersMask = RoundCornersAll | RoundCornersNone
}