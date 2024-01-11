// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

internal class ImGuiTabBar
{
    public List<ImGuiTabItem> Tabs { get; set; } = new List<ImGuiTabItem>();
    public ImGuiTabBarFlags Flags { get; set; }
    public uint Id { get; set; }
    public uint SelectedTabId { get; set; }
    public uint NextSelectedTabId { get; set; }
    public uint VisibleTabId { get; set; }
    public int CurrFrameVisible { get; set; }
    public int PrevFrameVisible { get; set; }
    public float ItemSpacingY { get; set; }
}