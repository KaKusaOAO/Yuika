using System.Drawing;
using System.Numerics;

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

public class ImGuiViewport
{
    public ImGuiViewportFlags Flags { get; set; }
    public Vector2 Position { get; set; }
    public SizeF Size { get; set; }
    public Vector2 WorkPosition { get; set; }
    public SizeF WorkSize { get; set; }

    public object? PlatformHandleRaw { get; set; }

#if USE_DOCKING
    public uint Id { get; set; }
    public float DpiScale { get; set; }
    public uint ParentViewportId { get; set; }
    public ImDrawData? DrawData { get; set; }
    public object? RendererUserData { get; set; }
    public object? PlatformUserData { get; set; }
    public object? PlatformHandle { get; set; }
    public bool PlatformWindowCreated { get; set; }
    public bool PlatformRequestMove { get; set; }
    public bool PlatformRequestResize { get; set; }
    public bool PlatformRequestClose { get; set; }
#endif

    public Vector2 Center => new Vector2(Position.X + Size.Width / 2, Position.Y + Size.Height / 2);
    public Vector2 WorkCenter => new Vector2(WorkPosition.X + WorkSize.Width / 2, WorkPosition.Y + WorkSize.Height / 2);

}