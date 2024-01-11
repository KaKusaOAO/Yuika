// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

public class ImGuiWindowClass
{
    public uint ClassId { get; set; }
    public uint ParentViewportId { get; set; } = uint.MaxValue;
    public ImGuiViewportFlags ViewportFlagsOverrideSet { get; set; }
    public ImGuiViewportFlags ViewportFlagsOverrideClear { get; set; }
    public ImGuiTabItemFlags TabItemFlagsOverrideSet { get; set; }
    public ImGuiDockNodeFlags DockNodeFlagsOverrideSet { get; set; }
    public bool DockingAlwaysTabBar { get; set; }
    public bool DockingAllowUnclassed { get; set; } = true;
}