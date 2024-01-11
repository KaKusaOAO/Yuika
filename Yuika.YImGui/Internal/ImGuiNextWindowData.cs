// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Drawing;
using System.Numerics;

namespace Yuika.YImGui.Internal;

internal class ImGuiNextWindowData
{
    public ImGuiNextWindowDataFlags Flags { get; set; }
    public ImGuiCond PosCond { get; set; }
    public ImGuiCond SizeCond { get; set; }
    public ImGuiCond CollapsedCond { get; set; }
    public ImGuiCond DockCond { get; set; }
    public Vector2 PosVal { get; set; }
    public Vector2 PosPivotVal { get; set; }
    public SizeF SizeVal { get; set; }
    public SizeF ContentSizeVal { get; set; }
    public Vector2 ScrollVal { get; set; }
    public ImGuiChildFlags ChildFlags { get; set; }
    public bool PosUndock { get; set; }
    public bool CollapsedVal { get; set; }
    public RectangleF SizeConstraintRect { get; set; }
    public ImGuiSizeCallback SizeCallback { get; set; }
    public object? SizeCallbackUserData { get; set; }
    public float BgAlphaVal { get; set; }
    public uint ViewportId { get; set; }
    public uint DockId { get; set; }
    public ImGuiWindowClass WindowClass { get; set; }
    public Vector2 MenuBarOffsetMinVal { get; set; }

    public void ClearFlags()
    {
        Flags = ImGuiNextWindowDataFlags.None;
    }
}