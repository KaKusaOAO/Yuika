namespace Yuika.YImGui.Internal;

[Flags]
internal enum ImGuiActivateFlags
{
    None,
    
    PreferInput = 1 << 0,
    PreferTweak = 1 << 1,
    TryToPreserveState = 1 << 2,
    FromTabbing = 1 << 3
}