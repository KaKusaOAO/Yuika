using System.Drawing;
using System.Numerics;

namespace Yuika.YImGui.Internal;

internal struct Vec2IH 
{
    public short X;
    public short Y;

    public Vec2IH(short x, short y) 
    {
        X = x;
        Y = y;
    }

    public static readonly Vec2IH Zero = new Vec2IH();
    public static readonly Vec2IH One = new Vec2IH(1, 1);
}

internal class ImGuiWindow : IImGuiWindow
{
    public ImGuiContext Ctx { get; set; }
    public string Name { get; set; }
    public uint Id { get; set; }
    public ImGuiWindowFlags Flags { get; set; }
    public ImGuiChildFlags ChildFlags { get; set; }
    public ImGuiViewportP Viewport { get; set; }
    public Vector2 Position { get; set; }
    public SizeF Size { get; set; }
    public SizeF SizeFull { get; set; }
    public SizeF ContentSize { get; set; }
    public SizeF ContentSizeIdeal { get; set; }
    public SizeF ContentSizeExplicit { get; set; }
    public Vector2 WindowPadding { get; set; }
    public float WindowRounding { get; set; }
    public float WindowBorderSize { get; set; }
    public Vector2 DecoOuterSize1 { get; set; }
    public Vector2 DecoOuterSize2 { get; set; }
    public Vector2 DecoInnerSize { get; set; }
    public uint MoveId { get; set; }
    public uint ChildId { get; set; }
    public Vector2 Scroll { get; set; }
    public Vector2 ScrollMax { get; set; }
    public Vector2 ScrollTarget { get; set; }
    public Vector2 ScrollTargetCenterRatio { get; set; }
    public Vector2 ScrollTargetEdgeSnapDist { get; set; }
    public Vector2 ScrollbarSizes { get; set; }
    public bool ScrollbarX { get; set; }
    public bool ScrollbarY { get; set; }
    public bool Active { get; set; }
    public bool WasActive { get; set; }
    public bool WriteAccessed { get; set; }
    public bool Collapsed { get; set; }
    public bool WantCollapseToggle { get; set; }
    public bool SkipItems { get; set; }
    public bool Appearing { get; set; }
    public bool Hidden { get; set; }
    public bool IsFallbackWindow { get; set; }
    public bool IsExplicitChild { get; set; }
    public bool HasCloseButton { get; set; }
    public sbyte ResizeBorderHovered { get; set; }
    public sbyte ResizeBorderHeld { get; set; }
    public short BeginCount { get; set; }
    public short BeginCountPreviousFrame { get; set; }
    public short BeginOrderWithinParent { get; set; }
    public short BeginOrderWithinContext { get; set; }
    public short FocusOrder { get; set; }
    public uint PopupId { get; set; }
    public sbyte AutoFitFramesX { get; set; }
    public sbyte AutoFitFramesY { get; set; }
    public bool AutoFitOnlyGrows { get; set; }
    public ImGuiDir AutoPosLastDirection { get; set; }
    public sbyte HiddenFramesCanSkipItems { get; set; }
    public sbyte HiddenFramesCannotSkipItems { get; set; }
    public sbyte HiddenFramesForRenderOnly { get; set; }
    public sbyte DisableInputFrames { get; set; }
    public ImGuiCond SetWindowPosAllowFlags { get; set; }
    public ImGuiCond SetWindowSizeAllowFlags { get; set; }
    public ImGuiCond SetWindowCollapsedAllowFlags { get; set; }
    public Vector2 SetWindowPosVal { get; set; }
    public Vector2 SetWindowPosPivot { get; set; }
    
    public List<uint> IdStack { get; set; }
    public ImGuiWindowTempData DC { get; set; }

    public RectangleF OuterRectClipped { get; set; }
    public RectangleF InnerRect { get; set; }
    public RectangleF InnerClipRect { get; set; }
    public RectangleF WorkRect { get; set; }
    public RectangleF ParentWorkRect { get; set; }
    public RectangleF ClipRect { get; set; }
    public RectangleF ContentRegionRect { get; set; }
    public Vec2IH HitTestHoleSize { get; set; }
    public Vec2IH HitTestHoleOffset { get; set; }

    public int LastFrameActive { get; set; }
    public int LastFrameJustFocused { get; set; }
    public float LastTimeActive { get; set; }

#if USE_DOCKING

    public bool DockIsActive { get; set; }
    public bool DockNodeIsVisible { get; set; }
    public bool DockTabIsVisible { get; set; }
    public bool DockTabWantClose { get; set; }
    public short DockOrder { get; set; }
    public ImGuiDockNode? DockNode { get; set; }
    public ImGuiDockNode? DockNodeAsHost { get; set; }
    public uint DockId { get; set; }

#endif

    public ImGuiWindow(ImGuiContext ctx, string name)
    {
        Ctx = ctx;
        Name = name;

        throw new NotImplementedException();
    }
    
    
}