// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Numerics;

namespace Yuika.YImGui.Internal;

internal class ImGuiWindowTempData
{
    public Vector2 CursorPos;
    public Vector2 CursorPosPrevLine;
    public Vector2 CursorStartPos;
    public Vector2 CursorMaxPos;
    public Vector2 IdealMaxPos;
    public Vector2 CurrLineSize;
    public Vector2 PrevLineSize;
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
    public ImGuiOldColumns? CurrentColumns { get; set; }
    public int CurrentTableIdx { get; set; }
    
    public float ItemWidth { get; set; }
    public float TextWrapPos { get; set; }
    public List<float> ItemWidthStack { get; set; } = new List<float>();
    public List<float> TextWrapPosStack { get; set; } = new List<float>();
}