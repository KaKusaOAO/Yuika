namespace Yuika.YImGui;

[Flags]
public enum ImGuiViewportFlags 
{
    None,

    IsPlatformWindow    = 1 << 0,
    IsPlatformMonitor   = 1 << 1,
    OwnedByApp          = 1 << 2,

#if USE_DOCKING
    NoDecoration        = 1 << 3,
    NoTaskBarIcon       = 1 << 4,
    NoFocusOnAppearing  = 1 << 5,
    NoFocusOnClick      = 1 << 6,
    NoInputs            = 1 << 7,
    NoRendererClear     = 1 << 8,
    NoAutoMerge         = 1 << 9,
    TopMost             = 1 << 10,
    CanHostOtherWindows = 1 << 11,

    IsMinimized         = 1 << 12,
    IsFocused           = 1 << 13,
#endif
}