namespace Yuika.YImGui;

[Flags]
public enum ImGuiTreeNodeFlags 
{
    None,

    Selected             = 1 << 0,
    Framed               = 1 << 1,
    AllowOverlap         = 1 << 2,
    NoTreePushOnOpen     = 1 << 3,
    NoAutoOpenOnLog      = 1 << 4,
    DefaultOpen          = 1 << 5,
    OpenOnDoubleClick    = 1 << 6,
    OpenOnArrow          = 1 << 7,
    Leaf                 = 1 << 8,
    Bullet               = 1 << 9,
    FramePadding         = 1 << 10,
    SpanAvailWidth       = 1 << 11,
    SpanFullWidth        = 1 << 12,
    SpanAllColumns       = 1 << 13,
    NavLeftJumpsBackHere = 1 << 14,
#if false
    NoScrollOnOpen       = 1 << 15,
#endif

    CollapsingHeader = Framed | NoTreePushOnOpen | NoAutoOpenOnLog
}
