﻿using System.Numerics;

namespace Yuika.YImGui.Internal;

internal class ImGuiWindowTempData
{
    public Vector2 CursorPos { get; set; }
    public Vector2 CursorPosPrevLine { get; set; }
    public Vector2 CursorStartPos { get; set; }
    public Vector2 CursorMaxPos { get; set; }
    public Vector2 IdealMaxPos { get; set; }
    public Vector2 CurrLineSize { get; set; }
    public Vector2 PrevLineSize { get; set; }
    public float CurrLineTextBaseOffset { get; set; }
    public float PrevLineTextBaseOffset { get; set; }
    public bool IsSameLine { get; set; }
    public bool IsSetPos { get; set; }
    public float Indent { get; set; }
    public float ColumnsOffset { get; set; }
    public float GroupOffset { get; set; }
    public Vector2 CursorStartPosLossyness { get; set; }
    
    public ImGuiNavLayer NavLayerCurrent { get; set; }
    public short NavLayersActiveMask { get; set; }
    public short NavLayersActiveMaskNext { get; set; }
    public bool NavIsScrollPushableX { get; set; }
    public bool NavHideHighlightOneFrame { get; set; }
    public bool NavWindowHasScrollY { get; set; }
    
    public bool MenuBarAppending { get; set; }
    public Vector2 MenuBarOffset { get; set; }
    public ImGuiMenuColumns MenuColumns { get; set; }
    public int TreeDepth { get; set; }
    public uint TreeJumpToParentOnPopMask { get; set; }
    public List<ImGuiWindow> ChildWindows { get; set; } = new List<ImGuiWindow>();
    public ImGuiStorage StateStorage { get; set; }
    public ImGuiOld
}