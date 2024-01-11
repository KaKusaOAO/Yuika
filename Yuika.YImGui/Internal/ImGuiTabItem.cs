// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

internal class ImGuiTabItem
{
    public uint Id { get; set; }
    public ImGuiTabItemFlags Flags { get; set; }
    public ImGuiWindow? Window { get; set; }
    public int LastFrameVisible { get; set; } = -1;
    public int LastFrameSelected { get; set; } = -1;
    public float Offset { get; set; }
    public float Width { get; set; }
    public float ContentWidth { get; set; }
    public float RequestedWidth { get; set; } = -1;
    public int NameOffset { get; set; } = -1;
    public short BeginOrder { get; set; } = -1;
    public short IndexDuringLayout { get; set; } = -1;
    public bool WantClose { get; set; }
}